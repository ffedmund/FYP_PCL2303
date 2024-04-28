using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public int itemID;
        public Sprite itemIcon;
        public string itemName;
        public string itemDescription; 
    }
}

