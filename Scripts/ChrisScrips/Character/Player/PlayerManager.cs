using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace FYP
{
    public class PlayerManager : CharacterManager
    {
        public PlayerEquipmentManager playerEquipmentManager;

        public bool canLevelUp = false;
        public bool canUseSkill = true;
        public bool canAttack = true;
        
        public ParticleSystem levelUpEffect;
        public ParticleSystem dodgeEffect;

        public PlayerInventory playerInventory;
        public PlayerCombatManager playerCombatManager;

        [Header("Lock Non Local Player")]
        [SerializeField] bool isLocalPlayer = true;

        public CharacterController characterController;

        public PlayerNetworkManager playerNetworkManager;

        public PlayerMinimapIconController minimapIconController;

        void Start()
        {
            playerEquipmentManager = GetComponentInChildren<PlayerEquipmentManager>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerStats = GetComponent<PlayerStats>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerInventory = GetComponent<PlayerInventory>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            characterController = GetComponent<CharacterController>();
            playerAbilityManager = GetComponent<PlayerAbilityManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            minimapIconController = GetComponentInChildren<PlayerMinimapIconController>();
        }

        //[Warning] This function will never be called
        private void OnClientConnectedCallback(ulong clientId)
        {
            Debug.Log("Client connected with id: " + clientId);

            GameSessionManager.instance.AddPlayerToActivePlayerList(this);

            if (!IsServer && IsOwner)
            {
                foreach(var player in GameSessionManager.instance.players)
                {
                    if(player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningOnline(player);
                    }
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            playerNetworkManager = GetComponent<PlayerNetworkManager>();

            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnRightWeaponChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnLeftWeaponChange;
            playerNetworkManager.currentHeadEquipmentID.OnValueChanged += playerNetworkManager.OnHeadEquipmentChange;
            playerNetworkManager.currentBodyEquipmentID.OnValueChanged += playerNetworkManager.OnBodyEquipmentChange;
            playerNetworkManager.currentLegEquipmentID.OnValueChanged += playerNetworkManager.OnLegEquipmentChange;
            playerNetworkManager.currentHandEquipmentID.OnValueChanged += playerNetworkManager.OnHandEquipmentChange;

            if(playerEquipmentManager == null)
            {
                playerEquipmentManager = GetComponentInChildren<PlayerEquipmentManager>();                
            }
            playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }

        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            isInvulnerable = anim.GetBool("isInvulnerable");
            isPerformingFullyChargedAttack = anim.GetBool("isPerformingFullyChargedAttack");
            isFiringSpell = anim.GetBool("isFiringSpell");
            // anim.SetBool("isInAir", isInAir);
            isInAir = anim.GetBool("isInAir");
            isJumping = anim.GetBool("isJumping");
            isGrounded = anim.GetBool("isGrounded");

            if (!isLocalPlayer)
            {
                return;
            }

            inputHandler.TickInput(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();
            // playerLocomotion.ApplyForwardJumpForceOverTime();
            playerStats.RegenerateStamina();

            CheckForInteractableObject();
            playerLocomotion.HandleAllMovement();
        }

        private void LateUpdate()
        {
            if(!isLocalPlayer){
                return;
            }
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.tap_rt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.inventory_Input = false;
            inputHandler.notebook_Input = false;

            float delta = Time.deltaTime;

            if (cameraHandler != null && !lockCameraMovement)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir)
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }

            Shader.SetGlobalVector("_Player",transform.position);
        }

        public void CheckForInteractableObject()
        {
            Vector3 rayOrigin = transform.position;
            rayOrigin.y += 2f;

            // Debug.Log("Checking for interactable object" + rayOrigin);

            RaycastHit hit;

            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers) ||
                Physics.SphereCast(rayOrigin, 0.3f, Vector3.down, out hit, 2.5f, cameraHandler.ignoreLayers))
            {
                // Debug.Log("Hit with " + hit.transform.name);
                if (hit.collider.tag == "Interactable")
                {
                    InteractableScript interactableObject = hit.collider.GetComponent<InteractableScript>();
                    if (interactableObject != null)
                    {
                        // Debug.Log("Interacting with " + interactableObject.name);
                        string interactableText = interactableObject.interactableText;
                        // inputHandler.interactableObject = interactableObject;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);

                        if (inputHandler.a_Input)
                        {
                            hit.collider.GetComponent<InteractableScript>().Interact(this);
                        }
                    }
                }
                else
                {
                    // Debug.Log("No Interacting Object");
                    if (interactableUIGameObject != null)
                    {
                        interactableUIGameObject.SetActive(false);
                    }

                    // if (itemInteractableGameObject != null && inputHandler.a_Input)
                    // {
                    //     itemInteractableGameObject.SetActive(false);
                    // }
                }
            }
        }

        public void SetLocalPlayer(bool isLocalPlayer){
            this.isLocalPlayer = isLocalPlayer;
        }

        private void LoadOtherPlayerCharacterWhenJoiningOnline(PlayerManager player)
        {
            player.playerNetworkManager.OnRightWeaponChange(0, player.playerNetworkManager.currentRightHandWeaponID.Value);
            player.playerNetworkManager.OnLeftWeaponChange(0, player.playerNetworkManager.currentLeftHandWeaponID.Value);
            player.playerNetworkManager.OnHeadEquipmentChange(0, player.playerNetworkManager.currentHeadEquipmentID.Value);
            player.playerNetworkManager.OnBodyEquipmentChange(0, player.playerNetworkManager.currentBodyEquipmentID.Value);
            player.playerNetworkManager.OnLegEquipmentChange(0, player.playerNetworkManager.currentLegEquipmentID.Value);
            player.playerNetworkManager.OnHandEquipmentChange(0, player.playerNetworkManager.currentHandEquipmentID.Value);
        }
    }
}
