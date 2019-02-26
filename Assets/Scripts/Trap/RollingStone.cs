using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : MonoBehaviour
{
    public Rigidbody2D rb;
    public float gravityThreshold;

    public void Fall()
    {
        rb.gravityScale = gravityThreshold;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.name);
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(1);
            Invoke("Destructor", 0.5f);
        }
    }

    public void Destructor()
    {
        Destroy(gameObject);
    }
}