using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject platform;
    public Transform currentPoint;
    public float moveSpeed;
    public Transform[] points;
    public int pointSelection;

    // Start is called before the first frame update
    void Start()
    {
        currentPoint = points[pointSelection];
    }

    // Update is called once per frame
    void Update()
    {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentPoint.position, Time.deltaTime * moveSpeed);

        // If platform moves to the set current point
        if(platform.transform.position == currentPoint.position)
        {
            // Move to next point
            pointSelection = (pointSelection + 1) % points.Length;

            currentPoint = points[pointSelection];
        }
    }
}
