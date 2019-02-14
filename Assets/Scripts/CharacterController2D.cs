using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    public bool isGrounded;             // Whether or not the player is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    private Rigidbody2D m_Rigidbody2D;  // the Rigidbody2D of player

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.


private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Update is called once per tick
    // modified in project stting -> time -> fix timestamp
    private void FixedUpdate()
    {
        isGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                isGrounded = true;
        }
    }

    public Vector2 GetVelocity()
    {
        return m_Rigidbody2D.velocity;
    }

    public bool GetFacingRight()
    {
        return m_FacingRight;
    }

    public void AddVelocity(float x, float y)
    {
        m_Rigidbody2D.velocity += new Vector2(x, y);

        FixFacingDirection();
    }

    public void SetVelocity(Vector2 newSpeed)
    {
        m_Rigidbody2D.velocity = newSpeed;

        FixFacingDirection();
    }

    public void SetHorizontalSpeed(float horizontalDeltaSpeed)
    {
        m_Rigidbody2D.velocity = new Vector2(horizontalDeltaSpeed, m_Rigidbody2D.velocity.y);

        FixFacingDirection();
    }

    public void SetVeticalSpeed(float veticalDeltaSpeed)
    {
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, veticalDeltaSpeed);
    }

    // This moves the character without any implied velocity.
    public void Teleport(Vector2 position)
    {
        m_Rigidbody2D.MovePosition(position);
    }

    private void FixFacingDirection()
    {
        if (m_Rigidbody2D.velocity.x > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (m_Rigidbody2D.velocity.x < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
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
