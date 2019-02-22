using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Singleton
    public static Inventory instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory!");
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public static int space = 9;  // Amount of item spaces
    public Item[] items;

    void Start()
    {
        Reset();
    }

    public void Add(Item item)
    {
        items[item.id] = item;
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public void Reset()
    {
        items = new Item[9];
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}