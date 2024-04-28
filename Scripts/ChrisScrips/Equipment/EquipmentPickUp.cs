using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace FYP
{
    public class EquipmentPickUp : InteractableScript
    {
        public EquipmentItem equipment;

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

                // playerInventory.equipmentsInventory.Add(equipment);

                if (equipment is HelmetEquipment)
                {
                    playerInventory.helmetEquipmentsInventory.Add((HelmetEquipment)equipment);
                }
                else if (equipment is ArmEquipment)
                {
                    playerInventory.armEquipmentsInventory.Add((ArmEquipment)equipment);
                }
                else if (equipment is TorsoEquipment)
                {
                    playerInventory.torsoEquipmentsInventory.Add((TorsoEquipment)equipment);
                }
                else if (equipment is LegEquipment)
                {
                    playerInventory.legEquipmentsInventory.Add((LegEquipment)equipment);
                }

                playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(equipment);

                foreach (Quest quest in playerManager.playerData.quests)
                {
                    if (quest.goalChecker.ItemCollected(equipment.itemName) && quest.goalChecker.goalType == GoalType.Gathering)
                    {
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