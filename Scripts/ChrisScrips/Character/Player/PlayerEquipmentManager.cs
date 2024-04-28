using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        PlayerManager playerManager;
        InputHandler inputHandler;
        PlayerInventory playerInventory;
        PlayerStats playerStats;

        [Header("Equipment Model Changers")]
        HeadCoveringsBaseHairModelChanger headCoveringsBaseHairModelChanger;
        HeadCoveringsNoFacialHairModelChanger headCoveringsNoFacialHairModelChanger;
        HeadCoveringsNoHairModelChanger headCoveringsNoHairModelChanger;
        HelmetModelChanger helmetModelChanger;
        TorsoModelChanger torsoModelChanger;
        HipModelChanger hipModelChanger;
        // LegModelChanger legModelChanger;
        // ArmModelChanger armModelChanger;
        // CapeModelChanger capeModelChanger;
        RightUpperArmModelChanger rightUpperArmModelChanger;
        RightLowerArmModelChanger rightLowerArmModelChanger;
        RightHandModelChanger rightHandModelChanger;
        RightLegModelChanger rightLegModelChanger;

        LeftUpperArmModelChanger leftUpperArmModelChanger;
        LeftLowerArmModelChanger leftLowerArmModelChanger;
        LeftHandModelChanger leftHandModelChanger;
        LeftLegModelChanger leftLegModelChanger;


        [Header("Default Naked Models")]
        // public string nakedHeadModel;
        public string nakedTorsoModel;
        public string nakedHipModel;
        // public string nakedLegModel;
        // public string nakedArmModel;
        // public string nakedCapeModel;
        public string nakedRightUpperArmModel;
        public string nakedRightLowerArmModel;
        public string nakedRightHandModel;
        public string nakedRightLegModel;

        public string nakedLeftUpperArmModel;
        public string nakedLeftLowerArmModel;
        public string nakedLeftHandModel;
        public string nakedLeftLegModel;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerInventory = GetComponentInParent<PlayerInventory>();


            headCoveringsBaseHairModelChanger = GetComponentInChildren<HeadCoveringsBaseHairModelChanger>();
            headCoveringsNoFacialHairModelChanger = GetComponentInChildren<HeadCoveringsNoFacialHairModelChanger>();
            headCoveringsNoHairModelChanger = GetComponentInChildren<HeadCoveringsNoHairModelChanger>();

            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
            hipModelChanger = GetComponentInChildren<HipModelChanger>();
            // legModelChanger = GetComponentInChildren<LegModelChanger>();
            // armModelChanger = GetComponentInChildren<ArmModelChanger>();
            // capeModelChanger = GetComponentInChildren<CapeModelChanger>();
            rightUpperArmModelChanger = GetComponentInChildren<RightUpperArmModelChanger>();
            rightLowerArmModelChanger = GetComponentInChildren<RightLowerArmModelChanger>();
            rightHandModelChanger = GetComponentInChildren<RightHandModelChanger>();
            rightLegModelChanger = GetComponentInChildren<RightLegModelChanger>();

            leftUpperArmModelChanger = GetComponentInChildren<LeftUpperArmModelChanger>();
            leftLowerArmModelChanger = GetComponentInChildren<LeftLowerArmModelChanger>();
            leftHandModelChanger = GetComponentInChildren<LeftHandModelChanger>();
            leftLegModelChanger = GetComponentInChildren<LeftLegModelChanger>();

            playerStats = GetComponentInParent<PlayerStats>();
        }

        private void Start()
        {
            EquipAllEquipmentModelsOnStart();
        }

        public void EquipAllEquipmentModelsOnStart()
        {
            headCoveringsBaseHairModelChanger.UnEquipAllHeadCoveringsBaseHairModels();
            headCoveringsNoFacialHairModelChanger.UnEquipAllHeadCoveringsNoFacialHairModels();
            headCoveringsNoHairModelChanger.UnEquipAllHeadCoveringsNoHairModels();
            helmetModelChanger.UnEquipAllHelmetModels();
            
            if (playerInventory.currentHelmetEquipment != null)
            {
                if (playerInventory.currentHelmetEquipment.headCoveringsBaseHairModelName != "")
                {
                    headCoveringsBaseHairModelChanger.EquipHeadCoveringsBaseHairModelByName(playerInventory.currentHelmetEquipment.headCoveringsBaseHairModelName);
                }
                else if (playerInventory.currentHelmetEquipment.headCoveringsNoFacialHairModelName != "")
                {
                    headCoveringsNoFacialHairModelChanger.EquipHeadCoveringsNoFacialHairModelByName(playerInventory.currentHelmetEquipment.headCoveringsNoFacialHairModelName);
                }
                else if (playerInventory.currentHelmetEquipment.headCoveringsNoHairModelName != "")
                {
                    headCoveringsNoHairModelChanger.EquipHeadCoveringsNoHairModelByName(playerInventory.currentHelmetEquipment.headCoveringsNoHairModelName);
                }
                else if (playerInventory.currentHelmetEquipment.helmetModelName != "")
                {
                    helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
                }

                if (playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentHeadEquipmentID.Value = playerInventory.currentHelmetEquipment.itemID;
                }

                playerStats.physicalDamageAbsorptionHead = playerInventory.currentHelmetEquipment.physicalDefense;

                Debug.Log("Head Absorption: " + playerInventory.currentHelmetEquipment.physicalDefense + "%");
            }
            else
            {
                // helmetModelChanger.EquipHelmetModelByName(nakedHeadModel);
                playerStats.physicalDamageAbsorptionHead = 0;

                if (playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentHeadEquipmentID.Value = -1;
                }
            }

            torsoModelChanger.UnEquipAllTorsoModels();
            rightUpperArmModelChanger.UnEquipAllRightUpperArmModels();
            leftUpperArmModelChanger.UnEquipAllLeftUpperArmModels();

            if (playerInventory.currentTorsoEquipment != null)
            {
                torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
                rightUpperArmModelChanger.EquipRightUpperArmModelByName(playerInventory.currentTorsoEquipment.rightUpperArmModelName);
                leftUpperArmModelChanger.EquipLeftUpperArmModelByName(playerInventory.currentTorsoEquipment.leftUpperArmModelName);

                if (playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentBodyEquipmentID.Value = playerInventory.currentTorsoEquipment.itemID;
                }

                playerStats.physicalDamageAbsorptionBody = playerInventory.currentTorsoEquipment.physicalDefense;
                
                Debug.Log("Body Absorption: " + playerInventory.currentTorsoEquipment.physicalDefense + "%");
            }
            else
            {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                rightUpperArmModelChanger.EquipRightUpperArmModelByName(nakedRightUpperArmModel);
                leftUpperArmModelChanger.EquipLeftUpperArmModelByName(nakedLeftUpperArmModel);

                if (playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentBodyEquipmentID.Value = -1;
                }

                playerStats.physicalDamageAbsorptionBody = 0;
            }

            hipModelChanger.UnEquipAllHipModels();
            // legModelChanger.UnEquipAllLegModels();
            rightLegModelChanger.UnEquipAllRightLegModels();
            leftLegModelChanger.UnEquipAllLeftLegModels();

            if (playerInventory.currentLegEquipment != null)
            {
                hipModelChanger.EquipHipModelByName(playerInventory.currentLegEquipment.hipModelName);
                rightLegModelChanger.EquipRightLegModelByName(playerInventory.currentLegEquipment.rightLegModelName);
                leftLegModelChanger.EquipLeftLegModelByName(playerInventory.currentLegEquipment.leftLegModelName);
                // legModelChanger.EquipLegModelByName(playerInventory.currentLegEquipment.legName);

                if(playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentLegEquipmentID.Value = playerInventory.currentLegEquipment.itemID;
                }

                playerStats.physicalDamageAbsorptionLegs = playerInventory.currentLegEquipment.physicalDefense;

                Debug.Log("Legs Absorption: " + playerInventory.currentLegEquipment.physicalDefense + "%");
            }
            else
            {
                hipModelChanger.EquipHipModelByName(nakedHipModel);
                // legModelChanger.EquipLegModelByName(nakedLegModel);
                rightLegModelChanger.EquipRightLegModelByName(nakedRightLegModel);
                leftLegModelChanger.EquipLeftLegModelByName(nakedLeftLegModel);

                if(playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentLegEquipmentID.Value = -1;
                }

                playerStats.physicalDamageAbsorptionLegs = 0;
            }

            // armModelChanger.UnEquipAllArmModels();
            rightLowerArmModelChanger.UnEquipAllRightLowerArmModels();
            rightHandModelChanger.UnEquipAllRightHandModels();
            leftLowerArmModelChanger.UnEquipAllLeftLowerArmModels();
            leftHandModelChanger.UnEquipAllLeftHandModels();

            if (playerInventory.currentArmEquipment != null)
            {
                // armModelChanger.EquipArmModelByName(playerInventory.currentArmEquipment.armModelName);
                rightLowerArmModelChanger.EquipRightLowerArmModelByName(playerInventory.currentArmEquipment.rightLowerArmModelName);
                rightHandModelChanger.EquipRightHandModelByName(playerInventory.currentArmEquipment.rightHandModelName);
                leftLowerArmModelChanger.EquipLeftLowerArmModelByName(playerInventory.currentArmEquipment.leftLowerArmModelName);
                leftHandModelChanger.EquipLeftHandModelByName(playerInventory.currentArmEquipment.leftHandModelName);

                if(playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentHandEquipmentID.Value = playerInventory.currentArmEquipment.itemID;
                }

                playerStats.physicalDamageAbsorptionHands = playerInventory.currentArmEquipment.physicalDefense;

                Debug.Log("Hands Absorption: " + playerInventory.currentArmEquipment.physicalDefense + "%");
            }
            else
            {
                // armModelChanger.EquipArmModelByName(nakedArmModel);
                rightLowerArmModelChanger.EquipRightLowerArmModelByName(nakedRightLowerArmModel);
                rightHandModelChanger.EquipRightHandModelByName(nakedRightHandModel);
                leftLowerArmModelChanger.EquipLeftLowerArmModelByName(nakedLeftLowerArmModel);
                leftHandModelChanger.EquipLeftHandModelByName(nakedLeftHandModel);
                
                if(playerManager.IsOwner)
                {
                    playerManager.playerNetworkManager.currentHandEquipmentID.Value = -1;
                }

                playerStats.physicalDamageAbsorptionHands = 0;
            }

            // capeModelChanger.UnEquipAllCapeModels();

            // if (playerInventory.currentCapeEquipment != null)
            // {
            //     capeModelChanger.EquipCapeModelByName(playerInventory.currentCapeEquipment.capeModelName);
            // }
        }
    }
}