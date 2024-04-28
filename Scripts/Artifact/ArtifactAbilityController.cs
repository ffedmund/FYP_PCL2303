using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FYP;
using UnityEngine;

public class ArtifactAbilityController : MonoBehaviour {
    [Header("Stats Bouns Amount")]
    public int healthLevel = 0;
    public int manaLevel = 0;
    public int staminaLevel = 0;
    public int strengthLevel = 0;
    public int intelligenceLevel = 0;
    public int dexterityLevel = 0;
    public int enduranceLevel = 0;
    public int luckLevel = 0;
    [Header("Stats Bouns Percentage")]
    public float healthPercentage = 1;
    public float manaPercentage = 1;
    public float staminaPercentage = 1;
    public float strengthPercentage = 1;
    public float intelligencePercentage = 1;
    public float dexterityPercentage = 1;
    public float endurancePercentage = 1;
    public float luckPercentage = 1;
    [Header("Active Ability")]
    public List<ArtifactSeries.SeriesAbility> abilities;

    Dictionary<ArtifactSeries.SeriesAbility,float> abilitiesCoolDownDict;
    Dictionary<int,ArtifactItem> ownedAritifactDict;
    PlayerInventory playerInventory;
    PlayerStats playerStats;
    UIController uiController;
    void Start(){
        uiController = FindAnyObjectByType<UIController>();
        ownedAritifactDict = new Dictionary<int, ArtifactItem>();
        abilitiesCoolDownDict = new Dictionary<ArtifactSeries.SeriesAbility, float>();
        abilities = new List<ArtifactSeries.SeriesAbility>();
        TryGetComponent(out playerInventory);
        TryGetComponent(out playerStats);
    }

    public void TriggerAbilities(TriggerCondition condition,CharacterStats targetStats ,float relateVariable = 0){
        foreach(var ability in abilities){
            if(!abilitiesCoolDownDict.ContainsKey(ability) || Time.time - abilitiesCoolDownDict[ability] >= ability.cd){
                bool abilityTriggered = false;
                switch(condition){
                    case TriggerCondition.EnemyDied:
                        break;
                    case TriggerCondition.Attack:
                        abilityTriggered = ArtifactSeries.TriggerAbility(ability,playerStats,targetStats);
                        break;
                    case TriggerCondition.HpPercentage:
                        if(relateVariable<ability.triggerValue){
                            abilityTriggered = ArtifactSeries.TriggerAbility(ability,playerStats,targetStats);
                        }
                        break;
                    case TriggerCondition.StaminaPercentage:
                        break;
                    case TriggerCondition.Stats:
                        break;
                    default:
                        break;
                }
                if(abilityTriggered){
                    if(abilitiesCoolDownDict.ContainsKey(ability)){
                        abilitiesCoolDownDict[ability] = Time.time;
                    }else{
                        abilitiesCoolDownDict.Add(ability,Time.time);
                    }
                }
            }
        }
    }

    public void UpdateArtifacts(){
        abilities = new List<ArtifactSeries.SeriesAbility>();
        List<int> currentArtifactIds = new List<int>(ownedAritifactDict.Keys);
        Dictionary<ArtifactSeries,int> seriesAmountDict = new Dictionary<ArtifactSeries, int>();

        foreach(ArtifactItem artifact in playerInventory.artifactsInventory){
            if(!ownedAritifactDict.ContainsKey(artifact.id)){
                ownedAritifactDict.Add(artifact.id,artifact);
                UpdateAritfactAbility(artifact);//Add Aritfact Ability
            }else{
                currentArtifactIds.Remove(artifact.id);
            }
            foreach(var series in artifact.series){
                if(seriesAmountDict.ContainsKey(series)){
                    seriesAmountDict[series]++;
                }else{
                    seriesAmountDict.Add(series,1);
                }   
            }
        }
        foreach(int lostArtifactId in currentArtifactIds){
            UpdateAritfactAbility(ownedAritifactDict[lostArtifactId],-1);//Remove Aritfact Ability
            ownedAritifactDict.Remove(lostArtifactId);
        }
        foreach(ArtifactSeries artifactSeries in seriesAmountDict.Keys){
            foreach(var ability in artifactSeries.GetAbility()){
                if(seriesAmountDict[artifactSeries] >= ability.triggerArtifactAmount){
                    abilities.Add(ability);
                    uiController.SetProgressTitle("Series Active");
                }
            }
        }
        playerStats.UpdatePlayerStats();
    }

    void UpdateAritfactAbility(ArtifactItem artifact, int multiplier = 1){
        foreach(ArtifactItem.AbilityStats abilityStats in artifact.abilities){
            int value = abilityStats.value * multiplier;
            switch(abilityStats.statType){
                case StatType.Health:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        healthLevel += value;
                    }else{
                        healthPercentage += value/100.0f;
                    }
                    break;
                case StatType.Mana:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        manaLevel += value;
                    }else{
                        manaPercentage += value/100.0f;
                    }
                    break;
                case StatType.Stamina:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        staminaLevel += value;
                    }else{
                        staminaPercentage += value/100.0f;
                    }
                    break;
                case StatType.Strength:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        strengthLevel += value;
                    }else{
                        strengthPercentage += value/100.0f;
                    }
                    break;
                case StatType.Intelligence:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        intelligenceLevel += value;
                    }else{
                        intelligencePercentage += value/100.0f;
                    }
                    break;
                case StatType.Dexterity:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        dexterityLevel += value;
                    }else{
                        dexterityPercentage += value/100.0f;
                    }
                    break;
                case StatType.Endurance:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        enduranceLevel += value;
                    }else{
                        endurancePercentage += value/100.0f;
                    }
                    break;
                case StatType.Luck:
                    if(abilityStats.valueType == ArtifactItem.ValueType.Amount){
                        luckLevel += value;
                    }else{
                        luckPercentage += value/100.0f;
                    }
                    break;
            }
        }
    } 
}