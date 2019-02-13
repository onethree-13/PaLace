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
		public int maxHealth = 5;

		private int _curHealth;
		public int curHealth
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

    public int touchDamage = 1;
    public int attackDamage = 1;
    public float meleeRange = 1.0f;
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
    [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
    public float timeBeforeTargetLost = 3.0f;

    // Move related parameters
    private bool isGrounded;            // Whether or not the player is grounded. (Update from CharacterController2D)
    private Vector2 velocity = Vector2.zero;    // current speed of player movement, will be modified by Move()

    // View related
    protected Transform m_Target;
    protected float m_TimeSinceLastTargetView;

    // Attack related
    // todo

    // Animation Related
    protected Animator m_Animator;   // Animation controller of player

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
        if (m_DamageArea != null)
            m_DamageArea.enabled = false;
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
        // Update isGrounded form characterController2d
        isGrounded = characterController2d.isGrounded;

        if (m_Target == null && player.detectable)
            ScanForPlayer();
        else
        {
            CheckTargetStillVisible();
            MoveToTarget();
        }

        // Update timer
        if (m_TimeSinceLastTargetView > 0.0f)
            m_TimeSinceLastTargetView -= Time.deltaTime;
    }

    public void ScanForPlayer()
    {
        if (!player.detectable)
            return;

        bool spriteFacingRight = characterController2d.GetFacingRight();
        Vector2 spriteForward;
        if (spriteFacingRight)
            spriteForward = Vector2.right;
        else
            spriteForward = Vector2.left;

        // If the player don't have control, they can't react, so do not pursue them
        if (!player.enableControl)
            return;

        Vector3 dir = player.transform.position - transform.position;

        // Check if distance to player is close enough for detection 
        if (dir.sqrMagnitude > viewDistance * viewDistance)
        {
            return;
        }
        
        // Check if the player is in the enemy's sight
        Vector3 testForward = Quaternion.Euler(0, 0, Mathf.Sign(spriteForward.x) * viewDirection) * spriteForward;

        float angle = Vector3.Angle(testForward, dir);

        if (angle > viewFov * 0.5f)
        {
            return;
        }

        // Set player detected
        m_Target = player.transform;
        m_TimeSinceLastTargetView = timeBeforeTargetLost;
    }

    public void MoveToTarget()
    {
        // Stand still if haven't detect player
        if (m_Target == null)
            return;

        float distance = m_Target.position.x - transform.position.x;
        if (distance < meleeRange)
        {
            // Close enough for melee attack, stop moving
            return;
        }

        // Player is at the right if distance is positive, vice versa。
        float move = Mathf.Sign(distance) * runSpeed * Time.fixedDeltaTime * 10;

        // Current velocity of player rigidbody
        Vector2 currentVelocity = characterController2d.GetVelocity();
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move, currentVelocity.y);
        // And then smoothing it out and applying it to the character
        characterController2d.SetVelocity(Vector2.SmoothDamp(currentVelocity, targetVelocity, ref velocity, m_MovementSmoothing));
    }

    public void CheckTargetStillVisible()
    {
        if (!player.detectable)
        {
            ForgetTarget();
            return;
        }

        if (m_Target == null)
            return;

        

        bool spriteFacingRight = characterController2d.GetFacingRight();
        Vector2 spriteForward;
        if (spriteFacingRight)
            spriteForward = Vector2.right;
        else
            spriteForward = Vector2.left;

        Vector3 toTarget = m_Target.position - transform.position;

        if (toTarget.sqrMagnitude < viewDistance * viewDistance)
        {
            Vector3 testForward = Quaternion.Euler(0, 0, viewDirection) * spriteForward;

            float angle = Vector3.Angle(testForward, toTarget);

            if (angle <= viewFov * 0.5f)
            {
                //we reset the timer if the target is at viewing distance.
                m_TimeSinceLastTargetView = timeBeforeTargetLost;
            }
        }

        if (m_TimeSinceLastTargetView <= 0.0f)
        {
            ForgetTarget();
        }
    }

    // Reset target if player escape from enemy's sight for timeBeforeTargetLost
    public void ForgetTarget()
    {
        m_Target = null;
    }

    public void Damage(int damage)
    {
        Debug.Log(string.Format("An enemy get {0} damage", damage));
        stats.curHealth -= damage;

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        if (stats.curHealth <= 0)
        {
            gameController.KillEnemy(this);
        }
    }

    // Damage player if player's collider touch enemy's collider
    void OnCollisionEnter2D(Collision2D _colInfo)
	{
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
        //draw the cone of view
        Vector3 forward =  Vector2.right;
        forward = Quaternion.Euler(0, 0, viewDirection) * forward;

        if (GetComponent<SpriteRenderer>().flipX) forward.x = -forward.x;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, viewFov, viewDistance);

        //Draw attack range
        //Handles.color = new Color(1.0f, 0, 0, 0.1f);
        //Handles.DrawSolidDisc(transform.position, Vector3.back, meleeRange);
    }
#endif
}
