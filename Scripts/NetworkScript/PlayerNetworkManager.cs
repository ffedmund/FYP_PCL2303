using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public virtual void OnHeadEquipmentChange(int oldEquipmentID, int newEquipmentID)
        {
            if (player.IsOwner)
            {
                return;
            }

            EquipmentItem newEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(newEquipmentID);

            if (newEquipment != null)
            {
                player.characterInventory.currentHelmetEquipment = newEquipment as HelmetEquipment;
            }
            else
            {
                player.characterInventory.currentHelmetEquipment = null;
            }

            player.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }

        public virtual void OnBodyEquipmentChange(int oldEquipmentID, int newEquipmentID)
        {
            if (player.IsOwner)
            {
                return;
            }

            EquipmentItem newEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(newEquipmentID);

            if (newEquipment != null)
            {
                player.characterInventory.currentTorsoEquipment = newEquipment as TorsoEquipment;
            }
            else
            {
                player.characterInventory.currentTorsoEquipment = null;
            }

            player.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }

        public virtual void OnLegEquipmentChange(int oldEquipmentID, int newEquipmentID)
        {
            if (player.IsOwner)
            {
                return;
            }

            EquipmentItem newEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(newEquipmentID);

            if (newEquipment != null)
            {
                player.characterInventory.currentLegEquipment = newEquipment as LegEquipment;
            }
            else
            {
                player.characterInventory.currentLegEquipment = null;
            }

            player.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }

        public virtual void OnHandEquipmentChange(int oldEquipmentID, int newEquipmentID)
        {
            if (player.IsOwner)
            {
                return;
            }

            EquipmentItem newEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(newEquipmentID);

            if (newEquipment != null)
            {
                player.characterInventory.currentArmEquipment = newEquipment as ArmEquipment;
            }
            else
            {
                player.characterInventory.currentArmEquipment = null;
            }

            player.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }
    }

}
