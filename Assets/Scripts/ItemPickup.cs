using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Pickup();
            gameObject.SetActive(false);
        }
    }

    void Pickup()
    {
        Debug.Log("Picking up " + item.name);
        Inventory.instance.Add(item);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
    }
}
