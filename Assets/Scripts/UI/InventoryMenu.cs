using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    Inventory inventory;
    public Transform ItemParent;
    InventorySlot[] slots;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback = UpdateUI;
        slots = ItemParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
        
    }
    
    void UpdateUI()
    {
        Debug.Assert(inventory.items.Length == slots.Length, "Inventory initialization error.");
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ShowItem(inventory.items[i]);
        }
    }
}
