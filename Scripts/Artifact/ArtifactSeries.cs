using System.Collections;
using Breeze.Core;
using FYP;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace FYP{
    public enum TriggerCondition{
        EnemyDied,
        Attack,
        HpPercentage,
        StaminaPercentage,
        Stats,
        Hurt
    }
    [CreateAssetMenu(menuName = "Items/Artifact/Artifact Series")]
    public class ArtifactSeries : ScriptableObject {
        public enum EmitTarget{
            Self,
            Others,
            Surrounding
        }
        public enum EffectType{
            Damage,
            Buff,
            Healing,
            Reduction
        }
        [System.Serializable]
        public struct SeriesAbility{
            public string name;
            public int triggerArtifactAmount;
            public float cd;
            public EmitTarget target;
            public EffectType effectType;        

            public GameObject effectPrefab;
            [Header("Damage Type")]
            public float basicDamage;
            [Header("Healing Type")]
            public float healingAmount;
            [Header("Buff/Debuff Type")]
            public Buff buff;
            public float duration;
            [Header("Condition")]
            public TriggerCondition condition;
            [Range(1,100)]
            public int probability;
            public int triggerValue;
        }    
        public string description;
        [SerializeField] SeriesAbility[] abilities;
        
        public SeriesAbility[] GetAbility(){
            return abilities;
        }

        public static bool TriggerAbility(SeriesAbility ability, CharacterStats self, CharacterStats others){
            Debug.Log("Ability Trigger");
            if(Random.Range(0,100)>=ability.probability){
                return false;
            }
            CharacterStats target;
            switch (ability.target){
                case EmitTarget.Self:
                    target = self;
                    break;
                case EmitTarget.Others:
                    if(others == null){
                        return false;
                    }
                    target = others;
                    break;
                case EmitTarget.Surrounding:
                    if(others != null){
                        return false;
                    }
                    target = null;
                    break;
                default:
                    return false;
            }
            switch(ability.effectType){
                case EffectType.Damage:
                    if(target != null){
                        if(target is EnemyStats){
                            ((EnemyStats)target).TakeDamage((int)ability.basicDamage, others.GetComponent<CharacterManager>());
                        }else{
                            target.TakeDamage((int)ability.basicDamage, others.GetComponent<CharacterManager>());
                        }
                        Instantiate(ability.effectPrefab,target.transform.position,Quaternion.identity);
                    }else{
                        GameObject attackEffect = Instantiate(ability.effectPrefab,self.transform.position + self.transform.forward,self.transform.rotation);
                        if(attackEffect.TryGetComponent(out ProjectileParticle projectileParticle)){
                            projectileParticle.Emit((int)ability.basicDamage,self);
                        }
                    }
                    break;
                case EffectType.Buff:
                    BuffManager.SetBuff(ability.buff,target,ability.duration);
                    break;
                case EffectType.Healing:
                    target.Heal((int)ability.healingAmount);
                    break;
                case EffectType.Reduction:
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}