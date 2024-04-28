using System.Collections.Generic;
using UnityEngine;
using FYP;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items; // Assume Item is your ScriptableObject
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
    public static ItemDatabase instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        foreach (var item in items)
        {
            itemDictionary.Add(item.itemName, item);
        }
    }

    public Item GetItemById(string id)
    {
        if (itemDictionary.ContainsKey(id))
        {
            return itemDictionary[id];
        }
        else
        {
            Debug.LogWarning("Item with ID " + id + " not found!");
            return null;
        }
    }
}
