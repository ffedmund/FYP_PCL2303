using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace FYP
{
    public class ArtifactPickUp : InteractableScript
    {
        public ArtifactItem artifact;

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
            ArtifactAbilityController artifactAbilityController;

            if(playerManager){
                playerInventory = playerManager.GetComponent<PlayerInventory>();
                playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
                animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();
                artifactAbilityController = playerManager.GetComponent<ArtifactAbilityController>();

                playerLocomotion.rigidbody.velocity = Vector3.zero;

                animatorHandler.PlayTargetAnimation("Pick Up Item", true);

                playerInventory.artifactsInventory.Add(artifact);
                artifactAbilityController.UpdateArtifacts();
                playerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(artifact);
                FindAnyObjectByType<UIController>().SetProgressTitle("Artifact Collect");


                if(NetworkManager.Singleton && TryGetComponent(out NetworkInteraction networkInteraction)){
                    networkInteraction.DespawnObjectRPC(NetworkManager.Singleton.LocalClientId);
                }else{
                    Destroy(gameObject);
                }
            }
        }
    }
}
