using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour {
    public bool enableControl = true;                                           // Enable player input.
    public float runSpeed = 32f;                                                // setting the rate of horizontal move
    public float inertia = 0.9f;                                                // setting the decreasing rate of horizontal speed when disabling controller
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    
    [SerializeField] private float m_JumpSpeed = 20f;                           // Amount of vertical velocity added when the player jumps.
    [SerializeField] private float m_JumpCancelSpeed = 10f;                     // Amount of vertical velocity added when the player jumps.
    
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [Range(0, 1)] [SerializeField] private float m_airborneSpeed = .8f;         // Amount of maxSpeed applied to airborne movement. 1 = 100%

    public bool detectable = true;     // Whether player could be detected by enemies.
    public bool invincible = false;    // Whether player could get damage from enemies's attack.

    public Transform sceneInitialSpawnPoint;        // Spawn point for respawning player when player die 
    public Transform lastSpawnPoint;                // (Deprecated) Nearest Spawn point for some trap
    public float spawnDelay = 2.0f;                 // How long between player die and player respawn
    public float spawnInvincibleDuration = 2.0f;    // How long player would be invincible after 
    public float blinkInterval = 0.2f;              // Blink interval when being invincible

    public float attackDamage = 1f;                                 // How much damage normal attack will cause.
    public float sneeakMultiplier = 2f;                             // Damage Multiplier which applys when player attacks without being detected by target enemy
    public float airborneMultiplier = 2f;                           // Damage Multiplier which applys when player attacks during jumping
    [SerializeField] private Collider2D m_DamageArea;               // An area where object will get damage when player press attack
    [SerializeField] private float m_AttackPreparePeriod = .02f;    // How long the attacking animation plays before the player can really cause damage
    [SerializeField] private float m_AttackPeriod = .04f;           // How long the player attack last
    [SerializeField] private float m_AttackColdDown = .02f;         // How long the player cannot cast another attack after finish one

    [System.Serializable]
	public class PlayerStats {
        private float _curHealth;
        
        public float maxHealth = 1.0f;

		public float curHealth
		{
			get { return _curHealth; }
            // Ensure the range of _curHealth is [0, maxHealth]
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); } 
        }

		public void Init()
		{
			curHealth = maxHealth;
		}
    }

	public PlayerStats stats = new PlayerStats();
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    private CharacterController2D characterController2d;
    private GameController gameController;

    

    // Player input related, passing from update() to fixUpdate()
    private bool jump = false;          // Whether the player is pressing jump
    private bool jumpCancel = false;    // Whether the player is releasing jump
    private bool crouch = false;        // Whether the player is crouching (sneaking)

    // Move related parameters, passing from update() to fixUpdate()
    private bool isGrounded;            // Whether or not the player is grounded. (Update from CharacterController2D)
    private float horizontalMove = 0f;  // the length of horizontal move with direction, 
                                        // (Warning) final movement may be less than horizontalMove because of crouching or air movement
    private Vector2 velocity = Vector2.zero;    // current speed of player movement, will be modified by Move()

    // Attack related paramaters
    private bool attack = false;        // Whether the player is attacking

    // Animation Related
    protected Animator m_Animator;   // Animation controller of player
    protected SpriteRenderer m_renderer; // Sprite renderer of player
    public void PlayAttackAnimation() => m_Animator.SetTrigger("Attacking");
    public void PlayHitAnimation() => m_Animator.SetTrigger("NormalHit");
    public void PlayDeathAnimation() => m_Animator.SetTrigger("DeadlyHit");
    public void PlayRespawnAnimation() => m_Animator.SetTrigger("Respawning");

    //public int fallBoundary = -20;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
        characterController2d = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
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

        // reset DamageArea physic state in game engine.
        m_DamageArea.enabled = true;
        m_DamageArea.enabled = false;
    }

	void Update () {
        horizontalMove = inertia * horizontalMove;

        if (enableControl)
        {
            horizontalMove = SimpleInput.GetAxisRaw("Horizontal") * runSpeed;

            if (SimpleInput.GetButtonDown("Crouch"))
                crouch = true;
            else if (Input.GetButtonUp("Crouch"))
                crouch = false;

            if (SimpleInput.GetButtonDown("Jump") && isGrounded && jump == false)
            {
                // The player starts to jump from any ground
                jump = true;
            }
            else if (SimpleInput.GetButtonUp("Jump") && !isGrounded && jumpCancel == false)
            {
                // The player starts to cancel jumping in the air
                jumpCancel = true;
            }

            // Player begins to attack 
            if (SimpleInput.GetButtonDown("Fire1") && !attack)
                StartCoroutine(AttackCoroutine());
        }
        
        // Update animation
        m_Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        m_Animator.SetBool("IsGrounded", isGrounded);
        m_Animator.SetBool("Sneaking", crouch);
    }

    public void ReleaseJump()
    {
        if (!enableControl || stats.curHealth <= 0.0f)
            return;

        if (!isGrounded && jump == false && jumpCancel == false)
        {
            // The player starts to cancel jumping in the air
            jumpCancel = true;
        }
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

    private void UpdateFacing(float facing)
    {
        // Ensure the sprite faces right when facing is larger than zero
        // And it faces left when facing is smaller than zero
        if (facing > 0.0f && !m_FacingRight)
        {
            m_FacingRight = true;
            characterController2d.Flip();
        }
        else if (facing < 0.0f && m_FacingRight)
        {
            m_FacingRight = false;
            characterController2d.Flip();
        }
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
            
            UpdateFacing(move);

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10, currentVelocity.y);
            // And then smoothing it out and applying it to the character
            characterController2d.SetVelocity(Vector2.SmoothDamp(currentVelocity, targetVelocity, ref velocity, m_MovementSmoothing));
        }

        // If the player is pressing jump...
        if (shouldJump)
        {
            // Add a vertical speed to the player.
            isGrounded = false;
            characterController2d.AddVelocity(0.0f, m_JumpSpeed);
        }
        // If the player is releasing jumping...
        else if (shouldCancelJump)
        {
            // If player vertical speed is higher than m_JumpCancelSpeed, limit it ot m_JumpCancelSpeed
            if (currentVelocity.y > m_JumpCancelSpeed)
                characterController2d.SetVeticalSpeed(m_JumpCancelSpeed);
        }
    }

    public void Damage (int damage, bool respawn = false) {
        // when the player is dead or invincible, player should not recieve any damage.
        if (stats.curHealth <= 0.0f || invincible)
            return;

        Debug.Log(string.Format("Player get {0} damage", damage));
        stats.curHealth -= damage;

        if (stats.curHealth <= 0.0f)
        {
            StartCoroutine(RespawnPlayer(sceneInitialSpawnPoint));
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
        // Unfixed Bug: Player flip for no reason before respawn
        enableControl = false;
        characterController2d.SetVelocity(Vector2.zero);
        characterController2d.SetRagdoll(false);

        PlayDeathAnimation();

        yield return new WaitForSeconds(spawnDelay);
        
        // hide player before teleporting 
        m_renderer.enabled = false;

        characterController2d.Teleport(spawnPoint.position);

        // Relaod Scence
        gameController.ResetScene();

        //PlayRespawnAnimation();
        //m_renderer.enabled = true;
        //enableControl = true;

        // Recover initial status.
        //stats.Init();
        //yield return StartCoroutine(InvincibleCoroutine(spawnInvincibleDuration));
    }

    public IEnumerator InvincibleCoroutine(float duration)
    {
        float timer = duration;
        invincible = true;

        // blinking effect
        m_renderer.enabled = false;
        while (timer > 0)
        {
            yield return new WaitForSeconds(blinkInterval);
            timer -= blinkInterval;
            m_renderer.enabled = !m_renderer.enabled;
        }
        m_renderer.enabled = true;

        invincible = false;
    }

    public IEnumerator AttackCoroutine()
    {
        attack = true;

        enableControl = false;

        // Avoid dead player attack enemy.
        if (stats.curHealth <= 0.0f)
            yield break;
        PlayAttackAnimation();

        yield return new WaitForSeconds(m_AttackPreparePeriod);

        // Avoid dead player attack enemy.
        if (stats.curHealth <= 0.0f)
            yield break;

        m_DamageArea.enabled = true;

        // Force game engine to calculate physic
        m_DamageArea.transform.position = m_DamageArea.transform.position + Vector3.zero;

        yield return new WaitForSeconds(m_AttackPeriod);

        // Calculate final damage
        float damage = attackDamage;
        if (!isGrounded)
            damage *= airborneMultiplier;

        // Cast damage to enemy in m_DamageArea
        m_DamageArea.GetComponent<DamageArea>().CommitDamage(damage, sneeakMultiplier);

        m_DamageArea.enabled = false;
        enableControl = true;

        yield return new WaitForSeconds(m_AttackColdDown);

        attack = false;
    }
}
