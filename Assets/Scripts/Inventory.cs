using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
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
        if(items == null)
        {
            items = new Item[space];
        }
        for (int i=0; i < space ; i++)
        {
            items[i] = null;
        }
        //if (onItemChangedCallback != null)
        //    onItemChangedCallback.Invoke();
    }

    public bool checkItemStatus(int id)
    {
        Debug.Assert(id < space);
        return (items[id] != null);
    }

    public int getItemNumber()
    {
        int count = 0;

        for(int i = 0; i < items.Length; i++)
        {
            if(items[i] != null)
            {
                count = count + 1;
            }
        }

        return count;
    }
}