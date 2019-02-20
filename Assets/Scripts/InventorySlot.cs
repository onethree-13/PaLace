using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    Item item;
    public Transform ItemParent;

    public void ShowItem(Item newItem)
    {
        if (newItem != null)
        {
            item = newItem;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}
