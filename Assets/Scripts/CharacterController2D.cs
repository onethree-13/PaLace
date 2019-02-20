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

private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (m_CeilingCheck == null)
        {
            m_CeilingCheck = transform.Find("CeilingCheck").transform;
            if (m_CeilingCheck == null)
                Debug.LogError("CeilingCheck not found");
        }
        if (m_GroundCheck == null)
        {
            m_GroundCheck = transform.Find("GroundCheck").transform;
            if (m_GroundCheck == null)
                Debug.LogError("GroundCheck not found");
        }
    }
    
    // Update is called once per tick
    // modified in project stting -> time -> fix timestamp
    private void FixedUpdate()
    {
        Debug.Log(m_Rigidbody2D.velocity);
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

    public void AddVelocity(float x, float y)
    {
        m_Rigidbody2D.velocity += new Vector2(x, y);
    }

    public void SetVelocity(Vector2 newSpeed)
    {
        m_Rigidbody2D.velocity = newSpeed;
    }

    public void SetHorizontalSpeed(float horizontalDeltaSpeed)
    {
        m_Rigidbody2D.velocity = new Vector2(horizontalDeltaSpeed, m_Rigidbody2D.velocity.y);
    }

    public void SetVerticalSpeed(float verticalDeltaSpeed)
    {
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, verticalDeltaSpeed);
    }

    // This moves the character without any implied velocity.
    public void Teleport(Vector2 position)
    {
        transform.position = position;
    }

    public void SetRagdoll(bool enable)
    {
        m_Rigidbody2D.isKinematic = enable;
    }

    public void Flip()
    {
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
