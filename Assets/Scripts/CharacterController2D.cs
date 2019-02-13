using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public bool enableControl = true;                                           // Enable player input. 
    [SerializeField] private int m_MaxJump = 1;                                 // Max time that player can jump before touching ground.
    [SerializeField] private float m_JumpSpeed = 20f;                           // Amount of vertical velocity added when the player jumps.
    [SerializeField] private float m_JumpCancelSpeed = 16f;                     // Amount of vertical velocity added when the player jumps.
    [SerializeField] private float m_JumpInterval = .5f;                        // Minimum interval between two jumps. 
    [SerializeField] private float runSpeed = 40f;                              // setting the rate of horizontal move
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, 1)] [SerializeField] private float m_airborneSpeed = .8f;         // Amount of maxSpeed applied to airborne movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Animator m_Animator;                               // Animation controller of player
    [SerializeField] private Collider2D m_DamageArea;                           // An area where object will get damage when player press attack
    [SerializeField] private float m_AttackPeriod = .7f;                        // How long the player cannot cast another attack

    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;  // the Rigidbody2D of player
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;    // current speed of player movement, will be modified by Move()

    // Move related parameters, passing from update() to fixUpdate()
    // Should not be used in other field
    private float horizontalMove = 0f;  // the length of horizontal move with direction, 
                                        // (Warning) final movement may be less than horizontalMove because of crouching or air movement
    private bool jump = false;          // Whether the player is pressing jump
    private bool jumpCancel = false;    // Whether the player is releasing jump
    private bool crouch = false;        // Whether the player is crouching (sneaking)
    private bool attack = false;        // Whether the player is attacking

    // Jump related parameters, maintain jumping status in Move()
    // Should not be used in other field
    private int remainJump = 0;         // How many times players can jump after one jumping
    private float jumpTimer = 0f;       // How long player cannot jump

    // Attack related paramaters
    private float atkTimer = 0f;

    public void PlayAttackAnimation() => m_Animator.SetTrigger("Attacking");
    public void PlayHitAnimation() => m_Animator.SetTrigger("NormalHit");
    public void PlayDeathAnimation() => m_Animator.SetTrigger("DeadlyHit");

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_DamageArea.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetButtonDown("Jump"))
        {
            // The player starts to jump from any ground
            if (m_Grounded)
            {
                jump = true;
                // Update parameter from jumping in the air
                remainJump = m_MaxJump - 1;
                jumpTimer = m_JumpInterval;
            }
            // The player pressing jumping and have reach the max duration
            else if (jumpTimer > 0.0 && remainJump > 0)
            {
                jump = true;
                // Update parameter from jumping in the air
                remainJump = remainJump - 1;
                jumpTimer = m_JumpInterval;
            }
        }
        else if (Input.GetButtonUp("Jump") && !m_Grounded)
        {
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
        m_Animator.SetBool("IsGrounded", m_Grounded);
        m_Animator.SetBool("Sneaking", crouch);

        // Contoll Damage Area based on attack status
        m_DamageArea.enabled = attack;
    }

    // Update is called once per tick
    // modified in project stting -> time -> fix timestamp
    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && colliders[i].gameObject.name != "Water")
                m_Grounded = true;
        }

        // Move our character
        Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, jumpCancel);
        jump = false;
        jumpCancel = false;

        // Update jumpTimer
        if (jumpTimer > 0.0)
        {
            jumpTimer = jumpTimer - Time.fixedDeltaTime;
        }
    }

    // Move the player, should only be used in FixUpdate()
    public void Move(float move, bool crouch, bool shouldJump, bool shouldCancelJump)
    {
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;
            }

            // if not grounded
            if (!m_Grounded && m_AirControl)
            {
                // Reduce the speed by the airborneSpeed multiplier
                move *= m_airborneSpeed;
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player is pressing jumping and can jump...
        if (shouldJump)
        {
            // Add a vertical speed to the player.
            m_Grounded = false;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y + m_JumpSpeed);
        }
        else if (shouldCancelJump)
        {
            if (m_Rigidbody2D.velocity.y > m_JumpCancelSpeed)
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpSpeed);
        }
    }

    // This moves the character without any implied velocity.
    public void Teleport(Vector2 position)
    {
        m_Rigidbody2D.MovePosition(position);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
