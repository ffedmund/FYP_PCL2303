using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    // [CreateAssetMenu(menuName = "Item Actions/Charge Attack Action")]
    // public class ChargeAttackAction : ItemAction
    public class ChargeAttackAction : MonoBehaviour
    {
    //     public override void PerformAction(PlayerManager player)
    //     {
    //         if (player.playerStats.currentStamina <= 0) return;

    //         if (player.canDoCombo)
    //         {
    //             player.inputHandler.comboFlag = true;
    //             HandleChargewWeaponCombo(player);
    //             player.inputHandler.comboFlag = false;
    //         }
    //         else
    //         {
    //             if (player.isInteracting) return;
    //             if (player.canDoCombo) return;

    //             HandleChargeAttack(player);
    //         }
            
    //     }

    //     private void HandleChargeAttack(PlayerManager player)
    //     {
    //         if (player.isUsingLeftHand)
    //         {
    //             player.anim.PlayerTargetAnimation(player.playerCombatManager.oh_charge_attack_01, true, false, true);
    //             player.playerCombatManager.lastAttack = player.playerCombatManager.oh_charge_attack_01;
    //         }
    //         else if (player.isUsingRightHand)
    //         {
    //             if (player.inputHandler.twoHandFlag)
    //             {
    //                 player.anim.PlayerTargetAnimation(player.playerCombatManager.th_charge_attack_01, true, false, true);
    //                 player.playerCombatManager.lastAttack = player.playerCombatManager.th_charge_attack_01;
    //             }
    //             else
    //             {
    //                 player.anim.PlayerTargetAnimation(player.playerCombatManager.oh_charge_attack_01, true, false, true);
    //                 player.playerCombatManager.lastAttack = player.playerCombatManager.oh_charge_attack_01;
    //             }
    //         }
    //     }

    //     private void HandleChargewWeaponCombo(PlayerManager player)
    //     {
    //         if (player.inputHandler.comboFlag)
    //         {
    //             player.anim.SetBool("canDoCombo", false);

    //             if (player.isUsingLeftHand)
    //             {
    //                 if (player.playerCombatManager.lastAttack == player.playerCombatManager.oh_charge_attack_01)
    //                 {
    //                     player.anim.PlayerTargetAnimation(player.playerCombatManager.oh_charge_attack_02, true, false, true);
    //                     player.playerCombatManager.lastAttack = player.playerCombatManager.oh_charge_attack_02;
    //                 }
    //                 else
    //                 {
    //                     player.anim.PlayerTargetAnimation(player.playerCombatManager.oh_charge_attack_01, true, false, true);
    //                     player.playerCombatManager.lastAttack = player.playerCombatManager.oh_charge_attack_01;
    //                 }
    //             }
    //             else if (player.isUsingRightHand)
    //             {
    //                 if (player.isTwoHandingWeapon)
    //                 {
    //                     if (player.playerCombatManager.lastAttack == player.playerCombatManager.th_charge_attack_01)
    //                     {
    //                         player.anim.PlayerTargetAnimation(player.playerCombatManager.th_charge_attack_02, true, false, true);
    //                         player.playerCombatManager.lastAttack = player.playerCombatManager.th_charge_attack_02;
    //                     }
    //                     else
    //                     {
    //                         player.anim.PlayerTargetAnimation(player.playerCombatManager.th_charge_attack_01, true, false, true);
    //                         player.playerCombatManager.lastAttack = player.playerCombatManager.th_charge_attack_01;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    }
}