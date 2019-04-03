using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public AudioClip pickUpSFXClip;

    private void Start()
    {
        if (Inventory.instance.checkItemStatus(item.id))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Pickup(collision.gameObject);
            gameObject.SetActive(false);
        }
    }

    void Pickup(GameObject playerObj)
    {
        Debug.Log("Picking up " + item.name);
        Inventory.instance.Add(item);
        playerObj.GetComponent<AudioSource>().PlayOneShot(pickUpSFXClip);
    }
}
