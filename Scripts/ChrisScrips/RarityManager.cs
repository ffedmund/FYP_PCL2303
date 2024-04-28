using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public class RarityManager : MonoBehaviour
    {
        public Color[] rarityColor = { new Color(0.7578f, 0.7305f, 0.6953f), new Color(0.1992f, 0.4180f, 0.2422f), new Color(0.3359f, 0.4922f, 0.6133f), new Color(0.3086f, 0.2109f, 0.3867f), new Color(0.8008f, 0.6758f, 0.2109f) };

    }

}

