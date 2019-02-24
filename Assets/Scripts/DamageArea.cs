using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (Warning) This class is only for casting damage from player.
public class DamageArea : MonoBehaviour
{
    public int overlapCacheSize = 10;
    [SerializeField] private LayerMask m_DamageableArea;
    List<GameObject> nearDamageable = new List<GameObject>();

    // Record all emeny entered Damage Area into nearEnemy.
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Damageable" || col.gameObject.tag == "Enemy")
        {
            //Debug.Log(string.Format("Attack list add {0}", col.name));
            nearDamageable.Add(col.gameObject);
        }
            
    }

    // Remove emeny who is in nearEnemy but leaves Damage Area.
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Damageable" || col.gameObject.tag == "Enemy")
        {
            //Debug.Log(string.Format("Attack list remove {0}", col.name));
            nearDamageable.Remove(col.gameObject);
        }
    }

    // Cast damage to enemy in nearEnemy.
    public void CommitDamage(float damage, float sneakingMultiplier)
    {
        // Get current overlapping objects
        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        Collider2D[] colliders = new Collider2D[overlapCacheSize];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(m_DamageableArea);
        int colliderCount = myCollider.OverlapCollider(contactFilter, colliders);
        //Debug.Log(string.Format("Get {0} overlapped colliders", colliderCount));

        for (int i = 0; i < colliderCount; i++)
        {
            if (colliders[i].gameObject.tag == "Damageable" || colliders[i].gameObject.tag == "Enemy")
            {
                if (!nearDamageable.Contains(colliders[i].gameObject)) {
                    //Debug.Log(string.Format("Attack list add {0}", colliders[i].name));
                    nearDamageable.Add(colliders[i].gameObject);
                }
            }
        }

        //Debug.Log(string.Format("Attack list count: {0}", nearDamageable.Count));
        foreach(GameObject gb in nearDamageable)
        {
            //Debug.Log(string.Format("Attacking {0}", gb.name));
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
