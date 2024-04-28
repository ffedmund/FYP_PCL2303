using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using FYP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Linq;

namespace FYP
{
    public class UIController : MonoBehaviour
    {
        public PlayerInventory playerInventory;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerManager playerManager;
        public EquipmentWindowUI equipmentWindowUI;
        public ItemInteractManager itemInteractManager;
        public BuffSlotsUIController buffSlotsUIController;

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        // public GameObject equipmentScreenWindow;
        public GameObject weaponInventoryWindow;
        public GameObject blackCover;
        public GameObject interactUI;
        public GameObject uiBundleWindow;
        public GameObject questUI;
        public GameObject levelBarUI;
        public GameObject notebookUI;
        public GameObject chatBoxWindow;
        public GameObject tradeWindow;

        [Header("Equipment Window Slot Selected")]
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;
        public bool headEquipmentSlotSelected;
        public bool torsoEquipmentSlotSelected;
        public bool legsEquipmentSlotSelected;
        public bool armEquipmentSlotSelected;
        public bool spellSlotSelected;


        [Header("Weapon Inventory")]
        // public GameObject weaponInventorySlotPrefab;
        public Transform weaponInventorySlotParent;
        // WeaponInventorySlot[] weaponInventorySlots;

        // [Header("Head Equipment Inventory")]
        // public GameObject headEquipmentInventorySlotPrefab;
        // public Transform headEquipmentInventorySlotParent;
        // HeadEquipmentInventorySlot[] headEquipmentInventorySlots;

        // [Header("Torso Equipment Inventory")]
        // public GameObject torsoEquipmentInventorySlotPrefab;
        // public Transform torsoEquipmentInventorySlotParent;
        // TorsoEquipmentInventorySlot[] torsoEquipmentInventorySlots;


        [Header("Inventory")]
        public GameObject inventorySlotPrefab;
        public Transform inventorySlotParent;
        public InventorySlot[] inventorySlots;

        [Header("Progress Title")]
        public GameObject title;
        public TextMeshProUGUI titleText;
        List<string> progressContentList = new List<string>();//queue

        public static PlayerData playerData;
        int showingInventoryIndex = 1;
        int previousShowingInventoryIndex = 1;

        [Header("Died UI")]
        public GameObject diedHeader;
        public GameObject backToMainButton;
        public GameObject respawnButton;

        [Header("Level Up Effect")]
        public GameObject levelUpPopUp;
        public TextMeshProUGUI levelUpText;
        public Image levelUpUpperBackground;
        public Image levelUpLowerBackground;

        [Header("Notice UI")]
        public GameObject noteNotice;

        [Header("Cross Button")]
        public Button notebookCloseButton;

        [Header("Other")]
        public GameObject questBlock;
        public TextMeshProUGUI honorLevelText;
        public TextMeshProUGUI coordinateText;
        public Slider honorBar;
        public GameObject currentInteractWindow;
        public HealthBar healthBar;
        public StaminaBar staminaBar;
        public TextMeshProUGUI abilityLevelText;
        public QuickSlotsUI quickSlotsUI;

        [Header("Active UI Window")]
        public GameObject activePopupWindow;

        bool isSetup;
        public GameObject abilityLockPanel;
        public TextMeshProUGUI abilityLockText;

        InputHandler _inputHandler;

        public void Setup(GameObject localPlayer)
        {
            playerInventory = localPlayer.GetComponent<PlayerInventory>();
            playerManager = localPlayer.GetComponent<PlayerManager>();
            _inputHandler = localPlayer.GetComponent<InputHandler>();
            playerEquipmentManager = localPlayer.GetComponentInChildren<PlayerEquipmentManager>();
            playerData = playerManager.playerData;

            notebookCloseButton.onClick.AddListener(()=>_inputHandler.notebook_Input=true);
            equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            equipmentWindowUI.LoadArmorOnEquipmentScreen(playerInventory);
            equipmentWindowUI.LoadSpellOnEquipmentScreen(playerInventory);
            equipmentWindowUI.firstTimeLoading = false;
            blackCover.SetActive(false);
            UpdateHonorUI();
            isSetup = true;

            FindAnyObjectByType<MapController>(FindObjectsInactive.Include).SetViewer(localPlayer.transform);
        }

        async void Start()
        {
            await DataReader.ReadBackgroundDataBase();
            if(!NetworkManager.Singleton){
                Setup(FindAnyObjectByType<PlayerManager>().gameObject);
            }
        }


        private void Update()
        {   
            if(!isSetup){
                return;
            }
            if(playerData != null){
                questUI.gameObject.SetActive(playerData.quests.Count > 0);
            }
            if(playerManager != null)
            {
                coordinateText.SetText((int)playerManager.transform.position.x + "," + (int)playerManager.transform.position.z);
            }
            if(playerData != null){
                if(_inputHandler.moveAmount > 0){
                    DoFadeUI(0,questUI.transform);
                    DoFadeUI(0,levelBarUI.transform);
                }else{
                    ShowLevelUI();
                    ShowQuestUI();
                }
            }

            if (levelUpPopUp.activeSelf && !levelUpText.GetComponent<Animator>().GetBool("canLevelUp"))
            {
                levelUpPopUp.SetActive(false);
            }
        }

        private void AddEquipmentToList()
        {
            playerInventory.equipmentsInventory = new List<EquipmentItem>();
            for (int i = 0; i < playerInventory.helmetEquipmentsInventory.Count; i++)
            {
                playerInventory.equipmentsInventory.Add(playerInventory.helmetEquipmentsInventory[i]);
            }
            for (int i = 0; i < playerInventory.torsoEquipmentsInventory.Count; i++)
            {
                playerInventory.equipmentsInventory.Add(playerInventory.torsoEquipmentsInventory[i]);
            }
            for (int i = 0; i < playerInventory.armEquipmentsInventory.Count; i++)
            {
                playerInventory.equipmentsInventory.Add(playerInventory.armEquipmentsInventory[i]);
            }
            for (int i = 0; i < playerInventory.legEquipmentsInventory.Count; i++)
            {
                playerInventory.equipmentsInventory.Add(playerInventory.legEquipmentsInventory[i]);
            }
        }

        public void UpdateUI()
        {
            #region Weapon Inventory Slots

            // UpdateInventorySlot(playerInventory.weaponsInventory);

            #endregion

            #region Inventory Slots
            switch (showingInventoryIndex)
            {
                case 1:
                    // Debug.Log("Material Inventory: " + playerInventory.materialsInventory[0]);
                    UpdateInventorySlot(playerInventory.materialsInventory.Values.ToList());
                    break;
                case 2:
                    UpdateInventorySlot(playerInventory.artifactsInventory);
                    break;
                case 3:
                    AddEquipmentToList();
                    UpdateInventorySlot(playerInventory.equipmentsInventory);
                    break;
                case 4:
                    UpdateInventorySlot(playerInventory.spellInventory);
                    break;
                default:
                    UpdateInventorySlot(playerInventory.weaponsInventory);
                    break;
            }
            #endregion
            if(previousShowingInventoryIndex != showingInventoryIndex){
                itemInteractManager.ItemNotInteract();
                previousShowingInventoryIndex = showingInventoryIndex;
            }
            uiBundleWindow.GetComponent<StatusUIManager>().UpdateText();
            
            equipmentWindowUI.UpdateSlotUI();
        }

        public void BlackCoverFadeIn(Action callback){
            Image image = blackCover.GetComponent<Image>();
            Color color = image.color;
            color.a = 0;
            image.color = color;
            blackCover.SetActive(true);
            DOTween.Kill(image);
            image.DOFade(1,5).OnComplete(()=>callback());
        }

        public void BlackCoverFadeOut(Action callback){
            Image image = blackCover.GetComponent<Image>();
            Color color = image.color;
            color.a = 1;
            image.color = color;
            DOTween.Kill(image);
            image.DOFade(0,5).OnComplete(()=>callback());
        }

        void ShowQuestUI()
        {
            if (playerData.quests.Count > 0)
            {
                GameObject tempQuestBlock;
                if(questUI.transform.childCount == 0){
                    tempQuestBlock = Instantiate(questBlock, questUI.transform);
                    tempQuestBlock.SetActive(false);
                }else{
                    tempQuestBlock = questUI.transform.GetChild(0).gameObject;
                }
                Quest quest = playerData.quests[0];
                TextMeshProUGUI title = tempQuestBlock.transform.Find("Title").GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI content = tempQuestBlock.transform.Find("QuestContent").GetComponent<TextMeshProUGUI>();
                title.SetText(quest.title);
                if (quest.goalChecker.isReached())
                {
                    title.color = Color.green;
                    if (quest.targetNPC != "")
                    {
                        content.SetText($"Find {quest.targetNPC}");
                    }
                    else
                    {
                        content.SetText("Report at Quest Board");
                    }
                }
                else
                {
                    title.color = Color.white;
                    content.SetText(quest.description + $"\n{quest.goalChecker.currentAmount}/{quest.goalChecker.targetAmount}");
                }
            }

            if(playerData.quests.Count > 0){
                foreach(Transform child in questUI.transform){
                    child.gameObject.SetActive(true);
                }
                DoFadeUI(1,questUI.transform);
            }
        }

        void DoFadeUI(float endValue,Transform uiTransform){
            foreach(var textMesh in uiTransform.GetComponentsInChildren<TextMeshProUGUI>()){
                if(DOTween.IsTweening(textMesh) || textMesh.color.a == endValue){
                    continue;
                }
                textMesh.DOFade(endValue,0.5f);
            }
            foreach(var image in uiTransform.GetComponentsInChildren<Image>()){
                if(DOTween.IsTweening(image) || image.color.a == endValue){
                    continue;
                }
                image.DOFade(endValue,0.5f);
            }
        }

        void ShowLevelUI()
        {
            DoFadeUI(1, levelBarUI.transform);
            TextMeshProUGUI levelText = levelBarUI.GetComponentInChildren<TextMeshProUGUI>();
            Slider levelBar = levelBarUI.GetComponentInChildren<Slider>();
            int playerLevel = playerData.GetAttribute("level");
            int exp = playerData.GetAttribute("exp");
            int expToNextLevel = (int)Mathf.RoundToInt(100 * (float)System.Math.Pow(1.01f, playerLevel) - 1);

            levelText.SetText($"LV{playerLevel}");
            levelBar.maxValue = expToNextLevel;
            levelBar.value = exp;

            if (playerManager.canLevelUp)
            {
                levelUpPopUp.SetActive(true);
                levelUpText.GetComponent<Animator>().SetBool("canLevelUp", true);
                levelUpUpperBackground.GetComponent<Animator>().SetBool("canLevelUp", true);
                levelUpLowerBackground.GetComponent<Animator>().SetBool("canLevelUp", true);

                playerManager.canLevelUp = false;
            }

            abilityLevelText.SetText($"Skill and Talent Level: {playerManager.playerAbilityManager?.playerRole.playerSkill.level}");
        }

        void UpdateInventorySlot<T>(List<T> inventory)
        {
            if(inventory.Count > 0 && inventorySlots.Length == 0){
                Instantiate(inventorySlotPrefab, inventorySlotParent);
                inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>(true);
            }
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (i < inventory.Count)
                {
                    if (inventorySlots.Length < inventory.Count)
                    {
                        Instantiate(inventorySlotPrefab, inventorySlotParent);
                        inventorySlots = inventorySlotParent.GetComponentsInChildren<InventorySlot>(true);
                    }

                    if (inventory[i] is WeaponItem)
                    {
                        inventorySlots[i].ClearInventorySlot();
                        inventorySlots[i].AddItem(inventory[i]);
                    }
                    else if (inventory[i] is MaterialItem)
                    {
                        inventorySlots[i].ClearInventorySlot();
                        inventorySlots[i].AddItem(inventory[i]);
                    }
                    else if (inventory[i] is ArtifactItem)
                    {
                        inventorySlots[i].ClearInventorySlot();
                        inventorySlots[i].AddItem(inventory[i]);
                    }
                    else if (inventory[i] is EquipmentItem)
                    {
                        inventorySlots[i].ClearInventorySlot();
                        inventorySlots[i].AddItem(inventory[i]);
                    }
                    else if (inventory[i] is SpellItem)
                    {
                        inventorySlots[i].ClearInventorySlot();
                        inventorySlots[i].AddItem(inventory[i]);
                    }
                    else
                    {
                        inventorySlots[i].ClearInventorySlot();
                    }
                }
                else
                {
                    inventorySlots[i].ClearInventorySlot();
                }
            }
        }

        public void UpdateHonorUI()
        {
            string honorLevelSting = "D";
            switch (playerData.GetHonorLevel())
            {
                case 1:
                    honorLevelSting = "C";
                    break;
                case 2:
                    honorLevelSting = "B";
                    break;
                case 3:
                    honorLevelSting = "A";
                    break;
                case 4:
                    honorLevelSting = "S";
                    break;
                default:
                    break;
            }
            honorLevelText.SetText(honorLevelSting);
            int honorValue = playerData.GetAttribute("honor");
            // Debug.Log((honorValue-Mathf.Pow(200,playerData.GetHonorLevel()+1)+200)/(Mathf.Pow(200,playerData.GetHonorLevel()+1)*(200-1)));
            honorBar.value = (honorValue - 100 * Mathf.Pow(2, playerData.GetHonorLevel() + 1) + 200) / (Mathf.Pow(2, playerData.GetHonorLevel() + 1) * 100);
        }

        public void SetProgressTitle(string content){
            if(progressContentList.Count == 0){
                progressContentList.Add(content);
            }else if(content != progressContentList[progressContentList.Count-1]){
                progressContentList.Add(content);
            }
            StartCoroutine(ProgressTitleCoroutine());
        }

        IEnumerator ProgressTitleCoroutine(){
            titleText.alpha = 0;
            titleText.SetText(progressContentList[0]);
            titleText.DOFade(1,2);
            yield return new WaitForSeconds(5);
            titleText.DOFade(0,1).OnComplete(()=>{
                progressContentList.RemoveAt(0);
                titleText.SetText("");
                if(progressContentList.Count > 0){
                    StartCoroutine(ProgressTitleCoroutine());
                }
            });
            yield return null;
        }

        public void ChangeShowingInventory(int index)
        {
            showingInventoryIndex = index;
            UpdateUI();
        }

        public void OpenNotebookWindow(){
            notebookUI.gameObject.SetActive(true);
            notebookUI.transform.GetChild(1).GetComponent<NotebookUIController>().ShowPage();
            playerManager.lockCameraMovement = true;
        }

        public void OpenSelectWindow()
        {
            uiBundleWindow.SetActive(true);
            hudWindow.SetActive(false);
            playerManager.lockCameraMovement = true;
        }

        public void CloseNotebookWindow(){
            notebookUI.gameObject.SetActive(false);
            noteNotice.SetActive(notebookUI.GetComponentInChildren<NotebookUIController>().noteManager.HaveUnreadContent());
            playerManager.lockCameraMovement = false;
        }

        public void CloseSelectWindow()
        {
            uiBundleWindow.SetActive(false);
            hudWindow.SetActive(true);
            itemInteractManager.ItemNotInteract();
            playerManager.lockCameraMovement = false;
        }

        public void CloseAllInventoryWindows()
        {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            // equipmentScreenWindow.SetActive(false);
        }

        public void CloseActivePopupWindow()
        {
            activePopupWindow.SetActive(false);
            activePopupWindow = null;
        }

        public bool CheckActivePopupWindow()
        {
            if(activePopupWindow)
            {
                CloseActivePopupWindow();
                return true;
            }
            return false;
        }

        public void PlayerDied(){
            BlackCoverFadeIn(()=>{
                diedHeader.SetActive(true);
                backToMainButton.SetActive(true);
                respawnButton.SetActive(true);
            });
        }

        public void PlayerRespawn(){
            diedHeader.SetActive(false);
            backToMainButton.SetActive(false);
            respawnButton.SetActive(false);
            BlackCoverFadeOut(()=>{});

            playerManager.playerStats.Respawn();
        }

        public void BackToMainMenu(){
            if(MySceneManager.instance){
                MySceneManager.instance.LoadScene(0);
            }
        }

        public void ShowNoteNotice(){
            noteNotice.SetActive(true);
        }

        public void HideNoteNotice(){
            noteNotice.SetActive(false);
        }

        public bool IsEquipmentSlotsEnabled(){
            return  leftHandSlot01Selected || 
                    leftHandSlot02Selected || 
                    rightHandSlot01Selected || 
                    rightHandSlot02Selected || 
                    headEquipmentSlotSelected ||
                    torsoEquipmentSlotSelected ||
                    legsEquipmentSlotSelected ||
                    armEquipmentSlotSelected;

        }

        public void EnableAllSelectedSlots()
        {
            rightHandSlot01Selected = true;
            rightHandSlot02Selected = true;
            leftHandSlot01Selected = true;
            leftHandSlot02Selected = true;

            headEquipmentSlotSelected = true;
            torsoEquipmentSlotSelected = true;
            legsEquipmentSlotSelected = true;
            armEquipmentSlotSelected = true;

            spellSlotSelected = true;
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            leftHandSlot01Selected = false;
            leftHandSlot02Selected = false;

            headEquipmentSlotSelected = false;
            torsoEquipmentSlotSelected = false;
            legsEquipmentSlotSelected = false;
            armEquipmentSlotSelected = false;

            spellSlotSelected = false;

            equipmentWindowUI.UnhighlightAllButtons();
        }
    }
}