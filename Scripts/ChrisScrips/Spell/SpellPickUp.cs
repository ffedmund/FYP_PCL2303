using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace FYP
{
    public class SpellPickUp : InteractableScript
    {
        public SpellItem spellItem;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;

            if(playerManager){
                playerInventory = playerManager.GetComponent<PlayerInventory>();
                playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
                animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

                playerLocomotion.rigidbody.velocity = Vector3.zero;
                // playerLocomotion.rigidbody.angularVelocity = Vector3.zero;

                animatorHandler.PlayTargetAnimation("Pick Up Item", true);

                playerInventory.spellInventory.Add(spellItem);
                playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(spellItem);

                foreach(Quest quest in playerManager.playerData.quests){
                    if(quest.goalChecker.ItemCollected(spellItem.itemName) && quest.goalChecker.goalType == GoalType.Gathering){
                        break;
                    }
                }


                if(NetworkManager.Singleton && TryGetComponent(out NetworkInteraction networkInteraction)){
                    networkInteraction.DespawnObjectRPC(NetworkManager.Singleton.LocalClientId);
                }else{
                    Destroy(gameObject);
                }
            }
        }
    }
}