using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace FYP
{
    public class MaterialPickUp : InteractableScript
    {
        public MaterialItem material;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            PickUpItem(playerManager);
        }

        protected void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;
            PlayerStats playerStats;

            if(playerManager){

                playerInventory = playerManager.GetComponent<PlayerInventory>();
                playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
                animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();
                playerStats = playerManager.GetComponent<PlayerStats>();

                playerLocomotion.rigidbody.velocity = Vector3.zero;

                animatorHandler.PlayTargetAnimation("Pick Up Item", true);

                playerInventory.AddItem(material);

                // Debug.Log("Luck Level: " + playerStats.luckLevel);
                // Debug.Log("Luck Chance: " + playerStats.luckLevel / 200.0f);
                // if (Random.value < playerStats.luckLevel / 200.0f)
                // {
                //     playerInventory.AddItem(material);
                // }


                playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(material);

                foreach (Quest quest in playerManager.playerData.quests)
                {
                    if(quest.goalChecker.ItemCollected(material.itemName) && quest.goalChecker.goalType == GoalType.Gathering){
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
