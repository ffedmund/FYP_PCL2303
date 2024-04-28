using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Unity.Netcode;

namespace FYP
{
    public class InputHandler : PlayerSetupBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool a_Input;
        public bool b_Input;
        public bool x_Input;
        public bool y_Input;
        public bool rb_Input;
        public bool tap_rt_Input;
        public bool hold_rt_Input;
        public bool jump_Input;
        public bool inventory_Input;
        public bool notebook_Input;
        public bool lockOnInput;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        public bool rollFlag;
        public bool twoHandFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool inventoryFlag;
        public bool notebookFlag;
        public bool lockAbilityFlag;

        public float rollInputTimer;

        public bool shortcut_Input_1;
        public bool shortcut_Input_2;
        public bool shortcut_Input_3;

        [Header("Lock Non Local Player")]
        [SerializeField] bool inputLock;


        PlayerControls inputActions;
        PlayerCombatManager playerCombatManager;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        PlayerStats playerStats;
        WeaponSlotManager weaponSlotManager;
        CameraHandler cameraHandler;
        AnimatorHandler animatorHandler;
        UIController uiController;
        PlayerQuickSlots playerQuickSlots;

        Vector2 movementInput;
        Vector2 cameraInput;

        protected override void Awake()
        {
            base.Awake();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            uiController = FindObjectOfType<UIController>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerQuickSlots = GetComponent<PlayerQuickSlots>();
        }

        private float rtHoldTime = 0f;
        private float holdThreshold = 0.2f;

        private void Update()
        {
            if (inputLock)
            {
                return;
            }

            if (hold_rt_Input)
            {
                rtHoldTime += Time.deltaTime;
                if (rtHoldTime >= holdThreshold)
                {
                    // Handle hold input
                    Debug.Log("hold_rt_Input");
                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerCombatManager.HandleChargeAttack(playerInventory.rightWeapon);
                }
            }
            else if (rtHoldTime > 0)
            {
                if (rtHoldTime < holdThreshold)
                {
                    // Handle tap input
                    Debug.Log("tap_rt_Input");
                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerCombatManager.HandleHeavyAttack(playerInventory.rightWeapon);
                }
                rtHoldTime = 0f;
            }
        }

        void Start()
        {
            if (inputLock)
            {
                return;
            }

            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.RB.performed += i => rb_Input = true;

                inputActions.PlayerActions.HoldRT.performed += i => hold_rt_Input = true;
                inputActions.PlayerActions.HoldRT.canceled += i => hold_rt_Input = false;

                inputActions.PlayerActions.RT.performed += i => tap_rt_Input = true;

                inputActions.PlayerActions.DPadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerActions.DPadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.A.performed += i => a_Input = true;
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
                inputActions.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
                inputActions.PlayerActions.Y.performed += i => y_Input = true;
                inputActions.PlayerActions.X.performed += i => x_Input = true;
                inputActions.PlayerActions.Shortcut1.performed += i => shortcut_Input_1 = true;
                inputActions.PlayerActions.Shortcut2.performed += i => shortcut_Input_2 = true;
                inputActions.PlayerActions.Shortcut3.performed += i => shortcut_Input_3 = true;
                inputActions.PlayerActions.Notebook.performed += i => notebook_Input = true;
                inputActions.PlayerActions.LockAbility.performed += i => lockAbilityFlag = true;
            }

            inputActions.Enable();
        }

        public void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleRollInput(delta);
            HandleHoldRTInput();
            HandleAttackInput(delta);
            HandleQuickSlotsInput();
            HandleInventoryInput();
            HandleNotebookInput();
            HandleLockOnInput();
            HandleTwoHandInput();
            HandleShortcut();
            HandleSkillInput();
            HandleLockAbilityInput();
        }

        void HandleMoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            // b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            // b_Input = inputActions.PlayerActions.Roll.triggered;
            // b_Input = inputActions.PlayerActions.Roll.IsPressed();
            // sprintFlag = b_Input;

            if (b_Input)
            {
                rollInputTimer += delta;
                // sprintFlag = true;

                if (playerStats.currentStamina <= 0)
                {
                    b_Input = false;
                    sprintFlag = false;
                }

                if (moveAmount > 0.5f && playerStats.currentStamina > 0)
                {
                    sprintFlag = true;
                }
            }
            else
            {
                sprintFlag = false;
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        private void HandleAttackInput(float delta)
        {
            if(!playerManager.canAttack)
            {
                return;
            }

            if (rb_Input)
            {
                playerCombatManager.HandleRBAction();

                // playerCombatManager.HandleLightAttack(playerInventory.rightWeapon);
            }

            if (tap_rt_Input)
            {
                Debug.Log("tap_rt_Input");
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;

                Debug.Log("Testing");
                animatorHandler.anim.SetBool("isUsingRightHand", true);
                playerCombatManager.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }

        private void HandleQuickSlotsInput()
        {
            // inputActions.PlayerActions.DPadRight.performed += i => d_Pad_Right = true;
            // inputActions.PlayerActions.DPadLeft.performed += i => d_Pad_Left = true;

            if (d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }
            else if (d_Pad_Left)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInventoryInput()
        {
            if (inventory_Input && !notebookFlag)
            {
                if(uiController.CheckActivePopupWindow())
                {
                    //Close the Popup window
                    return;
                }
                inventoryFlag = !inventoryFlag;
                playerManager.canAttack = !inventoryFlag;

                if (inventoryFlag)
                {
                    uiController.OpenSelectWindow();
                    uiController.UpdateUI();
                }
                else
                {
                    uiController.CloseSelectWindow();
                    uiController.CloseAllInventoryWindows();
                }
            }
        }

        private void HandleNotebookInput()
        {
            if (notebook_Input && !inventoryFlag)
            {
                if(uiController.CheckActivePopupWindow())
                {
                    //Close the Popup window
                    return;
                }
                notebookFlag = !notebookFlag;
                playerManager.canAttack = !notebookFlag;

                if (notebookFlag)
                {
                    uiController.OpenNotebookWindow();
                }
                else
                {
                    uiController.CloseNotebookWindow();
                }
            }
        }

        private void HandleLockOnInput()
        {
            if (lockOnInput && lockOnFlag == false)
            {
                lockOnInput = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if (lockOnInput && lockOnFlag)
            {
                lockOnInput = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
            }

            if (lockOnFlag && right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (lockOnFlag && right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }

            cameraHandler.SetCameraHeight();
        }

        private void HandleTwoHandInput()
        {
            if (y_Input)
            {
                y_Input = false;

                twoHandFlag = !twoHandFlag;

                if (twoHandFlag)
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                }
                else
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
                }
            }
        }

        private void HandleShortcut()
        {
            if (shortcut_Input_1)
            {
                shortcut_Input_1 = false;
                playerQuickSlots.UseItemInSlot(0);
            }
            if (shortcut_Input_2)
            {
                shortcut_Input_2 = false;
                playerQuickSlots.UseItemInSlot(1);
            }
            if (shortcut_Input_3)
            {
                shortcut_Input_3 = false;
                playerQuickSlots.UseItemInSlot(2);
            }
        }

        private void HandleSkillInput()
        {
            if (x_Input)
            {
                Debug.Log("x_Input");
                x_Input = false;
                playerManager.playerAbilityManager.UseSkill(playerManager.playerAbilityManager.roleID);
            }
        }

        private void HandleHoldRTInput()
        {

            playerManager.anim.SetBool("isChargingAttack", hold_rt_Input);

            if (hold_rt_Input)
            {
                Debug.Log("hold_rt_Input");

                // playerManager.UpdateWhichHandCharacterIsUsing(true);
                // playerManager.playerInventory.currentItemBeingUsed = playerManager.playerInventory.rightWeapon;

                if (twoHandFlag)
                {
                    // if (playerManager.playerInventory.rightWeapon.th_hold_RT_Action != null)
                    // {
                    //     playerManager.playerInventory.rightWeapon.th_hold_RT_Action.PerformAction(playerManager);
                    // }
                }
                else
                {
                    // if (playerManager.playerInventory.rightWeapon.oh_hold_rt_Action != null)
                    // {
                    //     playerManager.playerInventory.rightWeapon.oh_hold_rt_Action.PerformAction(playerManager);
                    // }

                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerCombatManager.HandleChargeAttack(playerInventory.rightWeapon);
                }

            }
        }

        public void HandleLockAbilityInput()
        {
            if (lockAbilityFlag)
            {
                lockAbilityFlag = false;
                Debug.Log("lockAbilityFlag");
                if (playerManager.canUseSkill)
                {
                    playerManager.playerAbilityManager.LockAbility();
                    uiController.abilityLockPanel.SetActive(true);
                    uiController.abilityLockText.text = "Ability Locked";
                    uiController.abilityLockText.color = Color.red;
                }
                else
                {
                    playerManager.playerAbilityManager.UnlockAbility();
                    // uiController.abilityLockPanel.SetActive(false);
                    float powerUp = uiController.equipmentWindowUI.DestroyAllCurrentEquipment();
                    uiController.abilityLockText.text = "Ability Unlocked" + "\n" + "Power Up: " + powerUp + "%";
                    uiController.abilityLockText.color = Color.green;
                    playerManager.playerAbilityManager.HandleAbilityPowerUp(powerUp / 100);
                }

            }
        }

        public void EnableInput()
        {
            inputLock = false;
            Start();
        }

        public override void Setup(PlayerManager playerManager)
        {
            Awake();
        }
    }
}