using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStoneTrigger : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    public GameObject rollingStone;

    private void Start()
    {
        rigidbody2D = rollingStone.GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rigidbody2D.gravityScale = 2;
    }
}