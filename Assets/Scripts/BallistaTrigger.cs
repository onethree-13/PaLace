using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaTrigger : MonoBehaviour
{
    public Ballista BallistaPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Player"))
        {
            BallistaPrefab.Shoot();
        }
    }
}
