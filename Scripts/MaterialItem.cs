using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{   
    [System.Serializable]
    public struct ItemEffect{
        public ItemEffectType itemEffectType;
        [Header("Item Effect")]
        [HideInInspector] public Buff buff;
        [HideInInspector] public float duration;
        [HideInInspector] public int amount;
    }

    public enum ItemEffectType{
        None,
        Heal,
        Buff,
        Projectile,
        Read,
        Open
    }

    [CreateAssetMenu(menuName = "Items/Material Item")]
    public class MaterialItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;
        public int itemAmount; //contain number
        
        [Header("Idle Animations")]
        public string right_hand_idle;
        public string left_hand_idle;

        [Header("Item Effect")]
        public ItemEffect itemEffect;
    }
}