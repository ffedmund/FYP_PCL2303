using System.Collections.Generic;
using UnityEngine;
using FYP;

public class NetworkItemReferences : MonoBehaviour {
    public static NetworkItemReferences Singleton { get; internal set; }
    [SerializeField]
    NetworkItemList networkItemList;
    Dictionary<int,Item> networkItemDict = new Dictionary<int, Item>();

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        int i = 0;
        foreach(Item item in networkItemList.ItemList){
            networkItemDict[i++] = item;
        }
    }

    public int GetId(Item item){
        return networkItemList.FindIndex(item);
    }

    public Item GetItem(int id){
        return networkItemDict.ContainsKey(id)?networkItemDict[id]:null;
    }
}