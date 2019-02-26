using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingObject : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    public float leftPushRange;        // Push zone for left direction
    public float rightPushRange;       // Push zone for right direction
    public float velocityThreshold;    // Threshold for velocity, applied when rigibody in push zone
    public bool startLeft;             // Whether start with pushing to left

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        if(startLeft)
        {
            rigidbody2D.angularVelocity = velocityThreshold * -1;
        }
        else
        {
            rigidbody2D.angularVelocity = velocityThreshold;
        }
    }

    private void Update()
    {
        Push();
    }

    private void Push()
    {
        if(transform.rotation.z > 0 && transform.rotation.z < rightPushRange && rigidbody2D.angularVelocity > 0 && rigidbody2D.angularVelocity < velocityThreshold)
        {
            rigidbody2D.angularVelocity = velocityThreshold;
            Debug.Log("right push");
        }
        else if(transform.rotation.z < 0 && transform.rotation.z > leftPushRange && rigidbody2D.angularVelocity < 0 && rigidbody2D.angularVelocity > velocityThreshold * -1)
        {
            rigidbody2D.angularVelocity = velocityThreshold * -1;
            Debug.Log("left push");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.ToString());
    }
}
