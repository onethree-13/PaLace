using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaTrigger : MonoBehaviour
{
    public Ballista BallistaPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            BallistaPrefab.Shoot();
        }
    }
}
