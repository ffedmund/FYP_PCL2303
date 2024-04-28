using System.Collections;
using FYP;
using UnityEngine;

public class HitStopController : MonoBehaviour {
    public float sharpness = 50;
    AnimatorHandler animatorHandler;
    CharacterCombatManager characterCombatManager;

    void Start(){
        animatorHandler = transform.root.gameObject.GetComponentInChildren<AnimatorHandler>();
        characterCombatManager = transform.root.gameObject.GetComponent<CharacterCombatManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if((other.tag == "Enemy" || other.tag == "Player") && animatorHandler != null){
            float stopTime = 0.1f;
            float hitStrengthMultiplier = 1;
            HitBoxController enemyHitBox = other.GetComponentInChildren<HitBoxController>();
            if(characterCombatManager){
                hitStrengthMultiplier = characterCombatManager.currentAttackType == AttackType.heavy?2:1;
            }
            if(enemyHitBox){
                stopTime += (enemyHitBox.bodyPartWeakness-50)/50.0f*0.1f;
                enemyHitBox.DoShake(GetComponent<Collider>().ClosestPoint(other.transform.position),hitStrengthMultiplier);
            }
            StartCoroutine(animatorHandler.HitStop(stopTime));
        }
    }
}