using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public bool enableControl = true;                                           // Enable player input.
    [SerializeField] private float runSpeed = 32f;                              // setting the rate of horizontal move
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    
    [SerializeField] private float m_JumpSpeed = 20f;                           // Amount of vertical velocity added when the player jumps.
    [SerializeField] private float m_JumpCancelSpeed = 10f;                     // Amount of vertical velocity added when the player jumps.
    
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [Range(0, 1)] [SerializeField] private float m_airborneSpeed = .8f;         // Amount of maxSpeed applied to airborne movement. 1 = 100%

    [SerializeField] private Collider2D m_DamageArea;                           // An area where object will get damage when player press attack
    [SerializeField] private float m_AttackPeriod = .7f;                        // How long the player cannot cast another attack

    [System.Serializable]
	public class PlayerStats {
        private int _curHealth;
        
        public int maxHealth = 1;

		public int curHealth
		{
			get { return _curHealth; }
			set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
		}

		public void Init()
		{
			curHealth = maxHealth;
		}
    }

	public PlayerStats stats = new PlayerStats();

    private CharacterController2D characterController2d;
    private GameController gameController;

    public Transform sceneInitialSpawnPoint;
    public Transform lastSpawnPoint;
    public float spawnDelay = 2f;

    // Player input related, passing from update() to fixUpdate()
    private bool jump = false;          // Whether the player is pressing jump
    private bool jumpCancel = false;    // Whether the player is releasing jump
    private bool crouch = false;        // Whether the player is crouching (sneaking)
    private bool attack = false;        // Whether the player is attacking

    // Move related parameters, passing from update() to fixUpdate()
    private bool isGrounded;            // Whether or not the player is grounded. (Update from CharacterController2D)
    private float horizontalMove = 0f;  // the length of horizontal move with direction, 
                                        // (Warning) final movement may be less than horizontalMove because of crouching or air movement
    private Vector2 velocity = Vector2.zero;    // current speed of player movement, will be modified by Move()

    // Attack related paramaters
    private float atkTimer = 0f;

    // Animation Related
    protected Animator m_Animator;   // Animation controller of player
    public void PlayAttackAnimation() => m_Animator.SetTrigger("Attacking");
    public void PlayHitAnimation() => m_Animator.SetTrigger("NormalHit");
    public void PlayDeathAnimation() => m_Animator.SetTrigger("DeadlyHit");

    //public int fallBoundary = -20;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
        characterController2d = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_DamageArea.enabled = false;
    }

    void Start()
	{
		stats.Init();

        // Change player's pos and respawn point based on entrance if lastLoadedScene is not null. 
        if(!string.IsNullOrEmpty(gameController.lastLoadedScene))
        {
            Debug.Log(gameController.lastLoadedScene);
            sceneInitialSpawnPoint = GameObject.Find(gameController.lastLoadedScene + "_sp").transform;
            transform.position = sceneInitialSpawnPoint.position;
        }
        
        if(sceneInitialSpawnPoint == null)
        {
            GameObject copyInitialTransform = new GameObject();
            copyInitialTransform.transform.position = transform.position;
            sceneInitialSpawnPoint = copyInitialTransform.transform;
        }

        lastSpawnPoint = sceneInitialSpawnPoint;
    }

	void Update () {
        // Warning!!!
        // Please put operation not related to player input above this line,
        // Or it may not work when input is disabled. 
        if (!enableControl)
        {
            horizontalMove = 0f;
            return;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && jump == false)
        {
            // The player starts to jump from any ground
            jump = true;
        }
        else if (Input.GetButtonUp("Jump") && !isGrounded && jumpCancel == false)
        {
            // The player starts to cancel jumping in the air
            jumpCancel = true;
        }

        // Player begins to attack 
        if (Input.GetButtonDown("Fire1") && !attack)
        {
            attack = true;
            atkTimer = m_AttackPeriod;
            PlayAttackAnimation();
        }
        // Maintain attack status
        if (attack)
        {
            if (atkTimer > 0.0)
            {
                atkTimer = atkTimer - Time.deltaTime;
            }
            else
            {
                // stop attack if passed AttackPeriod(finished attack animation)
                attack = false;
            }
        }

        // Update animation
        m_Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        m_Animator.SetBool("IsGrounded", isGrounded);
        m_Animator.SetBool("Sneaking", crouch);

        // Contoll Damage Area based on attack status
        m_DamageArea.enabled = attack;
    }

    private void FixedUpdate()
    {
        // Update isGrounded form characterController2d
        isGrounded = characterController2d.isGrounded;

        // Move our character
        Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, jumpCancel);
        jump = false;
        jumpCancel = false;
    }

    // Move the player, should only be used in FixUpdate()
    public void Move(float move, bool crouch, bool shouldJump, bool shouldCancelJump)
    {
        // Current velocity of player rigidbody
        Vector2 currentVelocity = characterController2d.GetVelocity();

        //only control the player if grounded or airControl is turned on
        if (isGrounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;
            }

            // if not grounded
            if (!isGrounded && m_AirControl)
            {
                // Reduce the speed by the airborneSpeed multiplier
                move *= m_airborneSpeed;
            }
            
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10, currentVelocity.y);
            // And then smoothing it out and applying it to the character
            characterController2d.SetVelocity(Vector2.SmoothDamp(currentVelocity, targetVelocity, ref velocity, m_MovementSmoothing));
        }

        // If the player is pressing jumping and can jump...
        if (shouldJump)
        {
            // Add a vertical speed to the player.
            isGrounded = false;
            characterController2d.AddVelocity(0.0f, m_JumpSpeed);
        }
        else if (shouldCancelJump)
        {
            if (currentVelocity.y > m_JumpCancelSpeed)
                characterController2d.SetVeticalSpeed(m_JumpCancelSpeed);
        }
    }

    public void Damage (int damage, bool respawn = false) {
        Debug.Log(string.Format("Player get {0} damage", damage));
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            // ToDo Let player choose which reset the level
            PlayDeathAnimation();
            StartCoroutine(RespawnPlayer(sceneInitialSpawnPoint));

            // ToDo: reset the level
            // Maybe calling gameMaster.resetLevel() ?

            // Reset current health of the player.
            // ToDo: obtain a copy of Player.stats from last scene.
            stats.Init();
        }
        else
        {
            PlayHitAnimation();
            if (respawn)
                StartCoroutine(RespawnPlayer(lastSpawnPoint));
        }
    }

    public IEnumerator RespawnPlayer (Transform spawnPoint)
    {
        enableControl = false;

        yield return new WaitForSeconds(spawnDelay);

        transform.position = spawnPoint.position;

        enableControl = true;
    }
}
