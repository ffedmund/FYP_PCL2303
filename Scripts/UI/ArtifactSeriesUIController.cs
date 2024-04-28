using TMPro;
using UnityEngine;
using FYP;

public class ArtifactSeriesUIController : MonoBehaviour {
    public GameObject blockPrefab;
    
    public void UpdateUI(ArtifactAbilityController artifactAbility){
        while(transform.childCount != artifactAbility.abilities.Count){
            if(transform.childCount < artifactAbility.abilities.Count){
                Instantiate(blockPrefab,transform);
            }else{
                Destroy(transform.GetChild(0));
            }
        }

        for(int i = 0; i < artifactAbility.abilities.Count; i++){
            TextMeshProUGUI text = transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            ArtifactSeries.SeriesAbility ability = artifactAbility.abilities[i];
            string effectText = "";
            string extraText = (ability.condition == TriggerCondition.HpPercentage || ability.condition == TriggerCondition.StaminaPercentage)?$"{ability.triggerValue}% below":"";
            switch(ability.effectType){
                case ArtifactSeries.EffectType.Damage:
                    effectText = ability.basicDamage.ToString();
                    break;
                case ArtifactSeries.EffectType.Buff:
                    effectText = ability.buff.name+$"{ability.duration}s";
                    break;
                case ArtifactSeries.EffectType.Healing:
                    effectText = ability.healingAmount.ToString();
                    break;
            }
            text.SetText(
                $"{ability.name}\n<size=80%>{ability.probability}% {ability.effectType} {ability.target} by {effectText}, when {ability.condition} {extraText}.({ability.cd}s)</size>."
            );
        }
    }
}