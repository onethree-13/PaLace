using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (Warning) This class is only for casting damage from player.
public class DamageArea : MonoBehaviour
{
    List<GameObject> nearDamageable = new List<GameObject>();

    // Record all emeny entered Damage Area into nearEnemy.
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Damageable") nearDamageable.Add(col.gameObject);
    }

    // Remove emeny who is in nearEnemy but leaves Damage Area.
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Damageable") nearDamageable.Remove(col.gameObject);
    }

    // Cast damage to enemy in nearEnemy.
    public void CommitDamage(float damage, float sneakingMultiplier)
    {
        foreach(GameObject gb in nearDamageable)
        {
            if (gb.GetComponent<Enemy>() != null)
            {
                Enemy enemy = gb.GetComponent<Enemy>();
                if (enemy.isChasingPlayer)
                {
                    // Detected by this enemy
                    enemy.Damage(damage);
                }
                else
                {
                    // Not detected by this enemy
                    enemy.Damage(damage * sneakingMultiplier);
                }
            }
            
            //todo deal with other kinds of damageable object
        }
            
    }
}
