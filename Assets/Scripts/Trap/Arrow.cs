using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f; // arrow speed
    public Rigidbody2D rb;
    public float autoDestroy = 1f;  // in seconds

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        Destroy(gameObject, autoDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.name);
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            player.Damage(1);
        }
        Destroy(gameObject);
    }
}
