using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LethalZone : MonoBehaviour
{
    public float damage = 1f;
    public float enemyDamage = 100f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            player.Damage(damage);
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.Damage(enemyDamage);
        }
    }
}
