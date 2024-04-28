using UnityEngine;

namespace FYP{
    [CreateAssetMenu(menuName = "Buffs/Buff")]
    public class Buff : ScriptableObject {
        public int id;
        public string buffName;
        public Sprite icon;
        public float duration;
        [Header("Extra Stats")]
        public int extraStrength;
        public int extraIntelligence;
        public int extraDexterity;
        public int extraEndurance;
        public int extraLuck;
        public float extraStaminaRegenration;

        [Header("Unique Id")]
        public string uuid;

        public void Active(CharacterStats characterStats){
            characterStats.extraStrength += extraStrength; 
            characterStats.extraIntelligence += extraIntelligence;
            characterStats.extraDexterity += extraDexterity;
            characterStats.extraEndurance += extraEndurance;
            characterStats.extraLuck += extraLuck;
            characterStats.extraStaminaRegenration += extraStaminaRegenration;
        }

        public bool UpdateBuff(float deltaTime,CharacterStats characterStats){
            duration -= deltaTime;
            if(duration <= 0){
                characterStats.extraStrength -= extraStrength; 
                characterStats.extraIntelligence -= extraIntelligence;
                characterStats.extraDexterity -= extraDexterity;
                characterStats.extraEndurance -= extraEndurance;
                characterStats.extraLuck -= extraLuck;
                characterStats.extraStaminaRegenration -= extraStaminaRegenration;
                return false;
            }
            return true;
        }
    }
}