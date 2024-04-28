using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace FYP
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager characterManager;

        [Header("Equipment")]
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<int> currentHeadEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentBodyEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLegEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentHandEquipmentID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public virtual void OnRightWeaponChange(int oldWeaponID, int newWeaponID)
        {
            if(characterManager.IsOwner)
            {
                return;
            }

            WeaponItem newWeapon = WorldItemDataBase.Instance.GetWeaponItemByID(newWeaponID);

            if (newWeapon != null)
            {
                characterManager.characterInventory.rightWeapon = newWeapon;
                characterManager.weaponSlotManager.LoadWeaponOnSlot(newWeapon, false);
            }
        }

        public virtual void OnLeftWeaponChange(int oldWeaponID, int newWeaponID)
        {
            if (characterManager.IsOwner)
            {
                return;
            }

            WeaponItem newWeapon = WorldItemDataBase.Instance.GetWeaponItemByID(newWeaponID);

            if (newWeapon != null)
            {
                characterManager.characterInventory.leftWeapon = newWeapon;
                characterManager.weaponSlotManager.LoadWeaponOnSlot(newWeapon, true);
            }
        }
    }
}
