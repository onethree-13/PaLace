using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBlade : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public float leftPushRange;
    public float rightPushRange;
    public float velocityThreshold;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.angularVelocity = velocityThreshold;
    }

    private void Update()
    {
        Push();
    }

    private void Push()
    {
        if(transform.rotation.z > 0 && transform.position.z < rightPushRange && rigidbody2D.angularVelocity > 0 && rigidbody2D.angularVelocity < velocityThreshold)
        {
            rigidbody2D.angularVelocity = velocityThreshold;
        }
        else if(transform.rotation.z < 0 && transform.rotation.z > leftPushRange && rigidbody2D.angularVelocity < 0 && rigidbody2D.angularVelocity > velocityThreshold * -1)
        {
            rigidbody2D.angularVelocity = velocityThreshold * -1;
        }
    }
}
