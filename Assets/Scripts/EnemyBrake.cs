using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrake : MonoBehaviour
{
    

    public Directions brakeDirection = Directions.disable;

    [HideInInspector]
    public enum Directions { disable, left, right }
    private float BrakeDirect = .0f;

    private void Awake()
    {
        if (brakeDirection == Directions.left)
            BrakeDirect = -1f;
        else if (brakeDirection == Directions.right)
            BrakeDirect = 1f;
    }

    // Set brake for all the enemy enter this area
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
            col.gameObject.GetComponent<Enemy>().SetBrake(BrakeDirect);
    }

    // Release brake for all the enemy leave this area
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
            col.gameObject.GetComponent<Enemy>().SetBrake(0);
    }
}
