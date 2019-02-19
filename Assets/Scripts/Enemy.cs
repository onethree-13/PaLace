using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Assuming all our enemy initially face right.

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour {

    public bool enable = true;  // Enable AI of this enemy.

    [System.Serializable]
	public class EnemyStats {
		public float maxHealth = 8f;

		private float _curHealth;
		public float curHealth
		{
			get { return _curHealth; }
			set { _curHealth = Mathf.Clamp (value, 0, maxHealth); }
		}

		public void Init()
		{
			curHealth = maxHealth;
		}
	}
    [SerializeField] private StatusIndicator statusIndicator;

    public EnemyStats stats = new EnemyStats();
    [SerializeField]
    [Tooltip("If the sprite face left initially, enable this. Otherwise, leave disabled")]
    private bool spriteFaceRight = true;  // For determining which way this enemy is initially facing.
    

    [Header("Move settings")]
    [SerializeField] private float runSpeed = 32f;                              // setting the rate of horizontal move
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement

    [Header("Attack settings")]
    public int touchDamage = 1;
    public int meleeDamage = 1;
    public float meleeRange = .0f;
    public float meleeAngle = 60.0f;
    [SerializeField] private float m_AttackPreparePeriod = .9f; // How long the attacking animation plays before the enemy can really cause damage
    [SerializeField] private float m_AttackPeriod = 1f;         // How long the enemy freezes after an attack

    [Header("Scan settings")]
    [Tooltip("The angle of the forward of the view cone. 0 is forward of the sprite, 90 is up, 180 behind etc.")]
    [Range(0.0f, 360.0f)]
    public float viewDirection = 0.0f;
    [Range(0.0f, 360.0f)]
    public float viewFov = 30f;
    public float viewDistance = 6.0f;
    public float traceViewDistance = 6.0f;
    [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
    public float timeBeforeTargetLost = 6.0f;
    public bool isChasingPlayer = false;

    // Sprite Facing related parameters
    protected Vector2 m_SpriteForward;         // The forward vector of this enemy.

    // Enemey move related parameters
    private Vector2 velocity = Vector2.zero;    // current speed of player movement, will be modified by Move()

    // Enemey view related
    protected float m_TimeSinceLastTargetView;

    // Animation Related
    protected Animator m_Animator;   // Animation controller of player
    public void PlayAttackAnimation() => m_Animator.SetTrigger("Attacking");
    public void PlayHitAnimation() => m_Animator.SetTrigger("NormalHit");
    public void PlayDeathAnimation() => m_Animator.SetTrigger("DeadlyHit");

    // Other Controller and entity
    private CharacterController2D characterController2d;
    private GameController gameController;
    private Player player;

    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
        characterController2d = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (spriteFaceRight)
            m_SpriteForward = Vector2.right;
        else
            m_SpriteForward = Vector2.left;
    }

    void Start()
	{
		stats.Init();

		if (statusIndicator != null)
		{
			statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
		}
	}

    private void FixedUpdate()
    {
        if (!enable)
            return;

        // Try to find player if this enemy is not chasing player
        if (!isChasingPlayer && player.detectable)
            ScanForPlayer();
        
        if (!isChasingPlayer)
        {
            //Patrol(); //todo
        }
        else
        {
            CheckTargetStillVisible();
            OrientToTarget();
            MoveToTarget();
            TryMeleeAttack();
        }

        // Update timer
        if (m_TimeSinceLastTargetView > 0.0f)
            m_TimeSinceLastTargetView -= Time.deltaTime;
    }

    public void UpdateFacing(float facing)
    {
        if (facing > 0 && !spriteFaceRight)
        {
            spriteFaceRight = true;
            m_SpriteForward = Vector2.right;
            characterController2d.Flip();
        }
        else if (facing < 0 && spriteFaceRight)
        {
            spriteFaceRight = false;
            m_SpriteForward = Vector2.left;
            characterController2d.Flip();
        }
    }

    public void MemoryTarget() => isChasingPlayer = true;

    private bool TestPlayerinArc(Vector3 position, float distance, float fov)
    {
        Vector3 dir = player.transform.position - position;

        // Check if distance to player is close enough for detection 
        if (dir.sqrMagnitude > distance * distance)
            return false;

        // Check if the player is in the enemy's sight
        Vector3 testForward = Quaternion.Euler(0, 0, spriteFaceRight ? Mathf.Sign(m_SpriteForward.x) * viewDirection : Mathf.Sign(m_SpriteForward.x) * -viewDirection) * m_SpriteForward;
        float angle = Vector3.Angle(testForward, dir);
        if (angle > fov * 0.5f)
            return false;

        // Check if there are obstacle between player and this enemy
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        if (hit && hit.transform.gameObject.tag == "Player")
            return true;
        else
            return false;
    }

    public void ScanForPlayer()
    {
        isChasingPlayer = false;

        // Do not chase invisible player or dead player
        if (!player.detectable || player.stats.curHealth <= 0)
            return;

        if (TestPlayerinArc(transform.position, viewDistance, viewFov))
            isChasingPlayer = true;
    }

    public void CheckTargetStillVisible()
    {
        // Stand still if haven't detect player
        if (!isChasingPlayer)
            return;

        if (TestPlayerinArc(transform.position, viewDistance, viewFov))
        {
            // Player is still in sight, so reset the timer
            m_TimeSinceLastTargetView = timeBeforeTargetLost;
        }

        // Player is out of sight for some time, so stop chasing it.
        if (m_TimeSinceLastTargetView <= 0.0f)
            isChasingPlayer = false;
    }

    public void OrientToTarget()
    {
        if (!isChasingPlayer)
            return;

        Vector3 toTarget = player.transform.position - transform.position;

        if (Vector2.Dot(toTarget, m_SpriteForward) < 0)
        {
            UpdateFacing(-m_SpriteForward.x);
        }
    }

    private void SetSpeedWithSmoothing(Vector2 targetVelocity)
    {
        //Smoothing character velocity from current speed to target speed 
        characterController2d.SetVelocity(Vector2.SmoothDamp(characterController2d.GetVelocity(), targetVelocity, ref velocity, m_MovementSmoothing));
    }

    public void MoveToTarget()
    {
        // Stand still if haven't detect player
        if (!isChasingPlayer)
            return;

        // If close enough for melee attack, stop moving
        float distance = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(distance) < 0.5 * meleeRange)
        {
            SetSpeedWithSmoothing(Vector2.zero);
            return;
        }

        // Player is at the right if distance is positive, vice versa.
        float move = Mathf.Sign(distance) * runSpeed * Time.fixedDeltaTime * 10;

        UpdateFacing(move);

        // Move enemy to player based on move 
        Vector2 currentVelocity = characterController2d.GetVelocity();
        SetSpeedWithSmoothing(new Vector2(move, currentVelocity.y));
    }

    public void TryMeleeAttack()
    {
        // Stand still if haven't detect player
        if (!isChasingPlayer)
            return;

        // Check if the player is in the enemy's melee range and angle
        if (!TestPlayerinArc(transform.position, meleeRange, meleeAngle))
            return;

        // when player is dead, this enemy should not attack
        if (player.stats.curHealth <= 0)
            return;

        // Attaack the player
        StartCoroutine(AttackCoroutine());
    }

    public IEnumerator AttackCoroutine()
    {
        enable = false;
        PlayAttackAnimation();

        yield return new WaitForSeconds(m_AttackPreparePeriod);

        // Check if the player is in the enemy's melee range and angle
        // If yes, cast damage to player
        if (!TestPlayerinArc(transform.position, meleeRange, meleeAngle))
            player.Damage(meleeDamage);

        yield return new WaitForSeconds(m_AttackPeriod - m_AttackPreparePeriod);

        enable = true;
    }

    // Damage player if player's collider touch enemy's collider
    void OnCollisionEnter2D(Collision2D _colInfo)
    {
        // Dead object cannot attack
        if (stats.curHealth <= 0.0f)
            return;

        Player _player = _colInfo.collider.GetComponent<Player>();
        if (_player != null)
        {
            _player.Damage(touchDamage);

            if (_player.stats.curHealth <= 0)
            {
                // Stop chasing player if player is dead
                isChasingPlayer = false;
            }
        }
    }

    public void Damage(float damage)
    {
        // when being dead, this enemy should not recieve any damage.
        if (stats.curHealth <= 0.0f)
            return;

        Debug.Log(string.Format("An enemy get {0} damage", damage));
        stats.curHealth -= damage;

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        if (stats.curHealth <= 0)
        {
            StartCoroutine(DeathCoroutine(this));
        }
    }

    IEnumerator DeathCoroutine(Enemy enemy)
    {
        PlayDeathAnimation();
        yield return new WaitForSeconds(5); //todo 
        gameController.KillObject(enemy.gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Draw the cone of view
        Vector3 forward = spriteFaceRight ? Vector2.right : Vector2.left;
        forward = Quaternion.Euler(0, 0, spriteFaceRight ?  viewDirection : -viewDirection) * forward;

        if (GetComponent<SpriteRenderer>().flipX) forward.x = -forward.x;

        Vector3 viewEndpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (viewEndpoint - transform.position).normalized, viewFov, viewDistance);

        //Draw the cone of melee attack range
        Vector3 meleeEndpoint = transform.position + (Quaternion.Euler(0, 0, meleeAngle * 0.5f) * forward);
        Handles.color = new Color(1.0f, 0, 0, 0.1f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (meleeEndpoint - transform.position).normalized, meleeAngle, meleeRange);
    }
#endif
}
