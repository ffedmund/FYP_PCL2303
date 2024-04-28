using System.Collections.Generic;
using FYP;
using UnityEngine;

namespace FYP{
    [CreateAssetMenu(menuName = "LootList", order = 0)]
    public class LootList : ScriptableObject {
        [System.Serializable]
        public struct Loot{
            public Item lootItem;
            public int weight;
        }

        public List<Loot> list;
    }
}