using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LethalZone : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.name);
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            player.Damage(damage);
        }
    }
}
