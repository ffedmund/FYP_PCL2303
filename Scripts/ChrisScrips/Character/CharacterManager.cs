using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace FYP
{
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterCombatManager characterCombatManager;
        public CharacterInventory characterInventory;
        public InputHandler inputHandler;
        public Animator anim;
        public CameraHandler cameraHandler;
        public PlayerStats playerStats;
        public PlayerLocomotion playerLocomotion;
        public PlayerData playerData = new PlayerData();
        public PlayerAbilityManager playerAbilityManager;
        public WeaponSlotManager weaponSlotManager;

        public CharacterNetworkManager characterNetworkManager;

        public InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemInteractableGameObject;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isJumping;
        public bool isUsingSkill;
        public bool isInAir;
        public bool isGrounded;
        public bool isClimbing;
        public bool canDoCombo;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isInvulnerable;
        public bool isInvisible;
        public bool isPerformingFullyChargedAttack;
        public bool lockCameraMovement;
        public bool canRoll = true;

        [Header("Spells")]
        public bool isFiringSpell;

        public Transform lockOnTransform;

        protected virtual void Awake()
        {
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterInventory = GetComponent<CharacterInventory>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
    }
}

