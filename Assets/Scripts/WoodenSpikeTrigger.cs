using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenSpikeTrigger : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;

    public float gravityScaleThreshold;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            rigidbody2D.gravityScale = gravityScaleThreshold;
        }
    }
}
