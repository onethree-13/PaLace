using UnityEngine;

public class CharacterController2D_1 : MonoBehaviour
{

    [SerializeField] private int m_MaxJump = 2;                                 // Max time that player can jump before touching ground.
    [SerializeField] private float m_JumpForce = 450f;                          // Amount of force added when the player jumps.
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

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;  // the Rigidbody2D of player
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;    // current speed of player movement, will be modified by Move()

    // Move related parameters, passing from update() to fixUpdate()
    // Should not be used in other field
    private float horizontalMove = 0f;  // the length of horizontal move with direction, 
                                        // (Warning) final movement may be less than horizontalMove because of crouching or air movement
    private bool jump = false;          // Whether the player is pressing jump
    private bool jumpCancel = false;    // Whether the player is canceling jump
    private bool crouch = false;        // Whether the player is crouching (sneaking)
    private bool attack = false;        // Whether the player is attacking

    // Jump related parameters, maintain jumping status in Move()
    // Should not be used in other field
    private int remainJump = 0;          // How many times players can jump after one jumping
    private float nextTimeCanJump = 0f;  // When player can jump after one jumping
    public float jumpShortSpeed = 3f;    // Velocity for the lowest jump
    public float jumpSpeed = 6f;         // Velocity for the highest jump

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump") && m_Grounded)   // Player starts pressing the button
            jump = true;

        if (Input.GetButtonUp("Jump") && !m_Grounded)     // Player stops pressing the button
            jumpCancel = true;

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            attack = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            attack = false;
        }

        // Update animation
        m_Animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        m_Animator.SetBool("IsGrounded", m_Grounded);
        m_Animator.SetBool("Sneaking", crouch);
        m_Animator.SetBool("Attacking", attack);

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
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                break;
            }
        }

        // Move our character
        Move(horizontalMove * Time.fixedDeltaTime, crouch);
    }


    public void Move(float move, bool crouch)
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
        // If the player is pressing jumping...
        if (jump)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, jumpSpeed);
            jump = false;
        }
        // Cancel the jump when the button is no longer pressed
        if (jumpCancel)
        {
            if (m_Rigidbody2D.velocity.y > jumpShortSpeed)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, jumpShortSpeed);
            }
            jumpCancel = false;
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
