using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class ProjectileParticle : MonoBehaviour
{
    CharacterManager characterManager;
    CharacterStats characterStats;
    int basicDamage;

    public void Emit(int basicDamage,CharacterStats stats){
        this.basicDamage = basicDamage;
        this.characterStats = stats;
        this.characterManager = stats.GetComponent<CharacterManager>();
        GetComponent<ParticleSystem>().Play();
    }

    private void OnParticleCollision(GameObject other) {
        Debug.Log("Collide with " + other.name);
        if (other.tag == "Enemy")
            {
                EnemyStats enemyStats = other.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    if (characterStats != null)
                    {

                        int characterStrengthMultiplier = (int)System.Math.Round((characterStats.strengthLevel + characterStats.extraStrength - 10) * 0.5 * characterStats.strengthMultiplier);
                        int physicalDamage = (int)(basicDamage * characterStats.strengthMultiplier + characterStrengthMultiplier);
                        Debug.Log("Ability Damage: " + physicalDamage);
                        if(characterStats.TryGetComponent(out ArtifactAbilityController artifactAbility)){
                            physicalDamage = Mathf.FloorToInt((physicalDamage + artifactAbility.strengthLevel * 0.5f) * artifactAbility.strengthPercentage);
                        }

                        float finalPhysicalDamage = physicalDamage;

                        enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), characterManager);

                        DamageCollider enemyDamageCollider = other.GetComponentInChildren<DamageCollider>();
                        if (enemyDamageCollider != null)
                        {
                            enemyDamageCollider.DisableDamageCollider();
                        }
                    }
                }

                if (characterStats != null)
                {
                    EnemyManager enemyManager = other.GetComponent<EnemyManager>();
                    if (enemyManager != null)
                    {
                        enemyManager.currentTarget = characterStats;
                    }
                }
            }
    }
}
