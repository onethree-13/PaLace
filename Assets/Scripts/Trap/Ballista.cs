using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject ArrowPrefab;
    public int ArrowNumber = 3;
    public float ArrowInterval = 0.2f;

    public void Shoot()
    {
        for(int i = 0; i < ArrowNumber; i++)
        {
            Invoke("ShootOnce", ArrowInterval * i);
        }
    }

    void ShootOnce()
    {
        Instantiate(ArrowPrefab, FirePoint.position, FirePoint.rotation);
    }
}
