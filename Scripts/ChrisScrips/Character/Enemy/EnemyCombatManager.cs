using FYP;
using UnityEngine;

public class EnemyCombatManager : CharacterCombatManager 
{
    
    AnimatorHandler animatorHandler;
    EnemySpellSlots enemySpellSlots;
    EnemyManager enemyManager;
    EnemyWeaponSlotManager weaponSlotManager;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        enemySpellSlots = GetComponent<EnemySpellSlots>();
        enemyManager = GetComponent<EnemyManager>();
        weaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
    }


    public void SuccessfullyCastSpell(int index)
    {
        if(enemySpellSlots == null || index > enemySpellSlots.spellSlots.Length)
        {
            return;
        }
        enemySpellSlots.spellSlots[index].SuccessfullyCastSpell(animatorHandler, enemyManager, weaponSlotManager); 
        // animatorHandler.anim.SetBool("isFiringSpell", true);
    }
}