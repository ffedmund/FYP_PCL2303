using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FYP
{
    [CreateAssetMenu(fileName = "NetworkItemList", menuName = "Item/Network Item List")]
    public class NetworkItemList : ScriptableObject
    {
        [SerializeField]
        internal List<Item> List = new List<Item>();

        public IReadOnlyList<Item> ItemList => List;


        public int FindIndex(Item item)
        {
            int itemId = List.FindIndex(target => target == item);
            if(itemId == -1)
            {
                //Do Data Verify Search
                itemId = List.FindIndex(target => target.itemID == item.itemID && target.itemName == item.itemName);
            }
            return itemId;
        }
    }
}