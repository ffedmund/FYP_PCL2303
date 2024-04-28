using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class EquipmentItem : Item
    {
        [Header("Defense Bonus")]
        public float physicalDefense;

        [Header("Equipment Rarity")]
        public Rarity rarity;

        [Header("Equipment Level")]
        public int level;
    }
}

