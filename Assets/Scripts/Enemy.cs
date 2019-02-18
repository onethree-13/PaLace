using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Assuming all our enemy initially face right.

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour {

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
	
	public EnemyStats stats = new EnemyStats();
    [Tooltip("If the sprite face left initially, enable this. Otherwise, leave disabled")]
    private bool m_InitialFacingRight = true;  // For determining which way this enemy is initially facing.
    protected Vector2 m_SpriteForward;         // The forward vector of this enemy.

    public int touchDamage = 1;
    public int attackDamage = 1;
    public float meleeRange = .0f;
    public float meleeAngle = 60.0f;
    [SerializeField] private Collider2D m_DamageArea;   // An area where object will get damage when player press attack
    [SerializeField] private StatusIndicator statusIndicator;

    [SerializeField] private float runSpeed = 32f;                              // setting the rate of horizontal move
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement

    [Header("Scanning settings")]
    [Tooltip("The angle of the forward of the view cone. 0 is forward of the sprite, 90 is up, 180 behind etc.")]
    [Range(0.0f, 360.0f)]
    public float viewDirection = 0.0f;
    [Range(0.0f, 360.0f)]
    public float viewFov = 30f;
    public float viewDistance = 6.0f;
    public float traceViewDistance = 6.0f;
    [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
    public float timeBeforeTargetLost = 6.0f;

    // Move related parameters
    private Vector2 velocity = Vector2.zero;    // current speed of player movement, will be modified by Move()

    // View related
    public Transform target;
    protected float m_TimeSinceLastTargetView;

    // Attack related
    // todo

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        characterController2d = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();

        if (m_InitialFacingRight)
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
        

        if (target == null && player.detectable)
            ScanForPlayer();
        
        if (target == null)
        {
            //Patrol(); //todo
        }
        else
        {
            CheckTargetStillVisible();
            OrientToTarget();
            MoveToTarget();
        }

        // Update timer
        if (m_TimeSinceLastTargetView > 0.0f)
            m_TimeSinceLastTargetView -= Time.deltaTime;

    }

    public void UpdateFacing(float facing)
    {
        if (facing > 0 && m_SpriteForward == Vector2.left)
        {
            m_SpriteForward = Vector2.right;
            characterController2d.Flip();
        }
        else if (facing < 0 && m_SpriteForward == Vector2.right)
        {
            m_SpriteForward = Vector2.left;
            characterController2d.Flip();
        }
    }

    private void SetSpeedWithSmoothing(Vector2 targetVelocity)
    {
        //Smoothing character velocity from current speed to target speed 
        characterController2d.SetVelocity(Vector2.SmoothDamp(characterController2d.GetVelocity(), targetVelocity, ref velocity, m_MovementSmoothing));
    }

    public void ScanForPlayer()
    {
        if (!player.detectable)
            return;
        
        // If the player don't have control, they can't react, so do not pursue them
        if (!player.enableControl)
            return;

        Vector3 dir = player.transform.position - transform.position;

        // Check if distance to player is close enough for detection 
        if (dir.sqrMagnitude > viewDistance * viewDistance)
            return;
        
        // Check if the player is in the enemy's sight
        Vector3 testForward = Quaternion.Euler(0, 0, Mathf.Sign(m_SpriteForward.x) * viewDirection) * m_SpriteForward;
        float angle = Vector3.Angle(testForward, dir);
        if (angle > viewFov * 0.5f)
            return;

        // Check if there are obstacle between player and this enemy
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        Debug.Log(hit.transform.gameObject.name);
        if (hit)
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                // Set player detected
                target = player.transform;
                m_TimeSinceLastTargetView = timeBeforeTargetLost;
            }
        }        
    }

    public void OrientToTarget()
    {
        if (target == null)
            return;

        Vector3 toTarget = target.position - transform.position;

        if (Vector2.Dot(toTarget, m_SpriteForward) < 0)
        {
            UpdateFacing(-m_SpriteForward.x);
        }
    }

    public void MoveToTarget()
    {

        Debug.Log(target.name);
        // Stand still if haven't detect player
        if (target == null)
            return;

        float distance = target.position.x - transform.position.x;
        if (Mathf.Abs(distance) < meleeRange)
        {
            // Close enough for melee attack, stop moving
            SetSpeedWithSmoothing(Vector2.zero);
            return;
        }

        // Player is at the right if distance is positive, vice versa.
        float move = Mathf.Sign(distance) * runSpeed * Time.fixedDeltaTime * 10;

        Debug.Log(move);

        UpdateFacing(move);

        // Move enemy to player based on move 
        Vector2 currentVelocity = characterController2d.GetVelocity();
        SetSpeedWithSmoothing(new Vector2(move, currentVelocity.y));
    }

    public void CheckTargetStillVisible()
    {
        // Stand still if haven't detect player
        if (target == null)
            return;
        Vector3 toTarget = target.position - transform.position;

        if (toTarget.sqrMagnitude < traceViewDistance * traceViewDistance)
        {
            // Check if there are obstacle between player and this enemy
            RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget, viewDistance);

            if (hit)
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    //we reset the timer if the target is at trace viewing distance.
                    m_TimeSinceLastTargetView = timeBeforeTargetLost;
                }
            }
        }

        if (m_TimeSinceLastTargetView <= 0.0f)
            ForgetTarget();
    }

    public void MeleeAttack()
    {
        // Stand still if haven't detect player
        if (target == null)
            return;

        // Check if the player is in the enemy's melee range
        Vector3 toTarget = target.position - transform.position;
        if (toTarget.sqrMagnitude >= meleeRange * meleeRange)
            return;
        
        // Check if the player is in the enemy's melee angle
        Vector3 testForward = Quaternion.Euler(0, 0, Mathf.Sign(m_SpriteForward.x) * viewDirection) * m_SpriteForward;
        float angle = Vector3.Angle(testForward, toTarget);
        if (angle > meleeAngle * 0.5f)
            return;

        // Attaack the player
        player.Damage(1);
    }

    // Reset target if player escape from enemy's sight for timeBeforeTargetLost
    public void ForgetTarget()
    {
        target = null;
    }

    public void Damage(float damage)
    {
        // when being dead, this enemy should not recieve any damage.
        if (stats.curHealth <= 0)
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

    // Damage player if player's collider touch enemy's collider
    void OnCollisionEnter2D(Collision2D _colInfo)
	{
        // Dead object cannot attack
        if (stats.curHealth <= 0)
            return;

		Player _player = _colInfo.collider.GetComponent<Player>();
		if (_player != null)
		{
			_player.Damage(touchDamage);

            if (_player.stats.curHealth <= 0)
            {
                // Forget target if target is dead
                ForgetTarget();
            }
		}
	}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Draw the cone of view
        Vector3 forward =  Vector2.right;
        forward = Quaternion.Euler(0, 0, viewDirection) * forward;

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
