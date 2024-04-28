using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
  public enum StatType{
    Health,
    Mana,
    Stamina,
    Strength,
    Intelligence,
    Dexterity,
    Endurance,
    Luck
  }
  [CreateAssetMenu(menuName = "Items/Artifact/Artifact Item")]
  public class ArtifactItem : Item
  {
    public enum ValueType{
      Percentage,
      Amount
    }

    [System.Serializable]
    public struct AbilityStats{
      public StatType statType;
      public int value;
      public ValueType valueType;
    }
    public ArtifactSeries[] series;
    public int id;
    public AbilityStats[] abilities;
  }
}