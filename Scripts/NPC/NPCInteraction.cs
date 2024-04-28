using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;

public class NPCInteraction : InteractableScript
{
    public TextMeshProUGUI chatText;
    public NPC npc;
    public bool isInteracting;

    ChatBoxUIController chatBoxUIController;
    NPCQuestManager npcQuestManager;
    NPCItemGiver npcItemGiver;
    NPCDialogueController npcDialogueController;
    Animator _animator;
    PlayerManager interactingPlayerManager;
    NPCService[] services;
    Dictionary<string, int> ansAcceptQuestIdDict;
    new Camera camera;
    Firendship firendship;//Maybe change to dictionary later

    NPCHint nPCHint;
    NoteManager noteManager;
    bool hasUnlockHint = false;
    int hintCount = 0;

    public virtual void Awake()
    {
        canvasUIController = FindAnyObjectByType<UIController>();
        // Debug.Log($"{gameObject.name} Awake");
    }

    public virtual void Setup(NPC npc, Animator animator, Firendship firendship)
    {
        this.npc = npc;
        services = npc.services;
        _animator = animator;
        camera = GetComponentInChildren<Camera>();
        this.firendship = firendship;
        nPCHint = GetComponent<NPCHint>();
        noteManager = FindObjectOfType<NoteManager>();
    }

    void Start()
    {
        TryGetComponent(out npcQuestManager);
        TryGetComponent(out npcItemGiver);
        TryGetComponent(out npcDialogueController);
        targetUIWindow = canvasUIController.chatBoxWindow;
        canvasUIController.chatBoxWindow.TryGetComponent(out chatBoxUIController);
        ansAcceptQuestIdDict = new Dictionary<string, int>();
        isUITrigger = true;
        hasUnlockHint = false;
        hintCount = 0;
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        string chatContent = null;
        int originalFirendshipValue = firendship.firendshipValue;
        interactingPlayerManager = playerManager;
        interactingPlayerManager.lockCameraMovement = true;
        interactingPlayerManager.canAttack = false;
        isInteracting = true;
        GetComponent<Collider>().enabled = false;
        camera.enabled = true;
        _animator.Play("Talking");
        firendship.IncreaseFirendship();

        chatContent = npcItemGiver.TriggerEvents(playerManager);//Check whether have itme to give first
       
        #region Quest Response
        if (chatContent == null && npcQuestManager.HaveValidQuestEvent(playerManager))
        {
            NPCQuestInteraction npcQuestInteraction = npcQuestManager.InteractQuest(playerManager);
            chatContent = npcQuestInteraction.reponseDialogue;
            if (npcQuestInteraction.giveQuest)
            {
                chatBoxUIController.SetAnswerButton(0, npcQuestInteraction.playerChoiceDialogue, () =>
                {
                    npcQuestInteraction.acceptQuestCallback();
                    Init();
                });
            }
            else if (npcQuestInteraction.receivedQuest)
            {
                firendship.IncreaseFirendship(10);
            }
        }
        foreach (Quest quest in playerManager.playerData.quests)
        {
            if (quest.goalChecker.goalType == GoalType.Relationship)
            {
                quest.goalChecker.UpdateCurrentAmount(firendship.firendshipValue, npc.npcName);
            }
        }
        #endregion

        chatBoxUIController.SetFirendshipSlider(originalFirendshipValue, firendship.firendshipValue);
        if (chatContent == null)
        {
            Chat();
        }
        else
        {
            chatBoxUIController.SetDefaultAnswerListener(Init);
            chatBoxUIController.SetCloseButton(Init);
            chatBoxUIController.SetChatbox(npc.npcName, chatContent);
        }
    }

    public void Init()
    {
        npcDialogueController.TriggerDialogue(firendship);
        Debug.Log($"Interaction Inited, PlayerManager: {interactingPlayerManager}");
        interactingPlayerManager.lockCameraMovement = false;
        interactingPlayerManager.canAttack = true;
        camera.enabled = false;
        isInteracting = false;
        interactingPlayerManager = null;
        ansAcceptQuestIdDict = new Dictionary<string, int>();
        chatBoxUIController.HideChatbox();
        GetComponent<Collider>().enabled = true;
    }

    protected void Chat(string playerAnswer = "")
    {
        if (ServiceTriggered(playerAnswer))
        {
            return;
        }

        if (ansAcceptQuestIdDict.ContainsKey(playerAnswer))
        {
            npcQuestManager.GiveQuest(ansAcceptQuestIdDict[playerAnswer]);
        }
        string content = npcDialogueController.Talk(playerAnswer);
        chatBoxUIController.UpdateFirendshipSlider(firendship.firendshipValue);
        Debug.Log(content);
        if (content != null && content != "")
        {
            chatBoxUIController.SetChatbox(npc.npcName, content);
            Choice[] answeringChoice = npcDialogueController.GetAnsweringChoices();
            for (int i = 0; i < answeringChoice.Length; i++)
            {
                Choice choice = answeringChoice[i];
                if (choice.questAcceptTrigger)
                {
                    ansAcceptQuestIdDict.Add(choice.content, choice.questID);
                }
            }

            SetPlayerChoiceButton(answeringChoice);
        }
        else
        {
            Init();
        }
    }

    void SetPlayerChoiceButton(Choice[] answeringChoice)
    {
        List<Choice> choices = new List<Choice>(answeringChoice);
        foreach (NPCService service in services)
        {
            choices.Add(new Choice
            {
                content = service.triggerChat
            });
        }
        chatBoxUIController.SetResponse((answer, extraFirendshipValue) =>
        {
            this.Chat(answer);
            if (extraFirendshipValue >= 0)
            {
                this.firendship.IncreaseFirendship(extraFirendshipValue);
            }
            else
            {
                this.firendship.DecreaseFirendship(-extraFirendshipValue);
            }
        }, choices.ToArray());
        if (answeringChoice.Length == 0)
        {
            chatBoxUIController.SetDefaultAnswerListener(() => {Chat();});
        }
    }

    bool ServiceTriggered(string playerAnswer)
    {
        foreach (NPCService service in services)
        {
            if (service.triggerChat == playerAnswer && interactingPlayerManager.playerData.GetAttribute("money") >= service.cost)
            {
                switch (service.type)
                {
                    case NPCServiceType.Trade:
                        TradeManager.TradeParties host = new TradeManager.TradeParties
                        {
                            type = TradeManager.PartiesType.NPC,
                            npcInventory = GetComponent<NPCInventory>()
                        };

                        TradeManager.TradeParties client = new TradeManager.TradeParties
                        {
                            type = TradeManager.PartiesType.Player,
                            playerInventory = interactingPlayerManager.playerInventory
                        };
                        TradeManager.instance.StartTrade(host, client);
                        break;
                    case NPCServiceType.Effect:
                        FindAnyObjectByType<UIController>().buffSlotsUIController.CreateBuffIcon(service.buff, service.duration);
                        BuffManager.SetBuff(service.buff, interactingPlayerManager.playerStats, service.duration);
                        break;
                    case NPCServiceType.Heal:
                        interactingPlayerManager.playerStats.Heal(service.healingAmount);
                        break;
                    case NPCServiceType.Ask:
                        if (!interactingPlayerManager.playerInventory.materialsInventory.ContainsKey(service.targetItem.itemName))
                        {
                            chatBoxUIController.SetChatbox(npc.npcName, "You don't have any " + service.targetItem.itemName + ".");
                            return true;
                        }
                        else
                        {
                            if (!nPCHint.isHintListEmpty())
                            {
                                string hint = nPCHint.getNewHint();
                                hintCount++;
                                if (!hasUnlockHint)
                                {
                                    hasUnlockHint = true;
                                    noteManager.AddNote("prophet_hints");
                                }
                                noteManager.AddContentToParagraph("prophet_hints", 0, hintCount + ". " + hint);

                                chatBoxUIController.SetChatbox(npc.npcName, hint);
                                break;
                            }
                            else
                            {
                                chatBoxUIController.SetChatbox(npc.npcName, "You have discovered all the hints!");
                                return true;
                            }
                        }
                    case NPCServiceType.Upgrade:
                        WeaponItem currentRightHandWeapon = interactingPlayerManager.playerInventory.weaponsInRightHandSlots[interactingPlayerManager.playerInventory.currentRightWeaponIndex];
                        chatBoxUIController.SetChatbox(npc.npcName, "Are you sure you want to upgrade " + currentRightHandWeapon.itemName + " to level " + (currentRightHandWeapon.level + 1) + " for " + service.resourcesCost * currentRightHandWeapon.level + " resources and $" + service.cost * currentRightHandWeapon.level + "?");
                        
                        chatBoxUIController.SetAnswerButton(0, "Yes", () =>
                        {
                            chatBoxUIController.HideChatbox();

                            if (!interactingPlayerManager.playerInventory.materialsInventory.ContainsKey(service.resources.itemName))
                            {
                                chatBoxUIController.SetChatbox(npc.npcName, "You don't have any " + service.resources.itemName + ".");
                                return;
                            }

                            if (interactingPlayerManager.playerInventory.materialsInventory[service.resources.itemName].itemAmount < service.resourcesCost * currentRightHandWeapon.level)
                            {
                                chatBoxUIController.SetChatbox(npc.npcName, "You don't have enought " + service.resources.itemName + ".");
                                return;
                            }

                            if (interactingPlayerManager.playerData.GetAttribute("money") < service.cost * currentRightHandWeapon.level)
                            {
                                chatBoxUIController.SetChatbox(npc.npcName, "You don't have enought money.");
                                return;
                            }

                            // interactingPlayerManager.playerInventory.UpgradeItem(currentRightHandWeapon);
                            

                            interactingPlayerManager.playerInventory.RemoveItem(service.resources, service.resourcesCost * currentRightHandWeapon.level);
                            interactingPlayerManager.playerData.AddPlayerData("money", -service.cost * currentRightHandWeapon.level);

                            interactingPlayerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Resources, -service.resourcesCost * currentRightHandWeapon.level);
                            interactingPlayerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money, -service.cost * currentRightHandWeapon.level);

                            currentRightHandWeapon.level++;
                            currentRightHandWeapon.lightAttackDamageModifier += 0.1f;
                            currentRightHandWeapon.heavyAttackDamageModifier += 0.1f;
                            currentRightHandWeapon.chargeAttackDamageModifier += 0.1f;

                            Init();
                        });
                        chatBoxUIController.SetAnswerButton(1, "No", () =>
                        {
                            chatBoxUIController.HideChatbox();
                            chatBoxUIController.SetChatbox(npc.npcName, "Goodbye!");

                            Init();
                        });

                        return true;
                    default:
                        break;
                }
                if (service.type != NPCServiceType.Ask)
                {
                    interactingPlayerManager.playerData.AddPlayerData("money", -service.cost);
                    interactingPlayerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Money, -service.cost);
                }
                else
                {
                    interactingPlayerManager.itemInteractableGameObject.GetComponent<ItemPopUpController>().NewPopUp(ItemPopUpController.PopUpIcon.Ticket, -service.cost);
                    interactingPlayerManager.playerInventory.RemoveItem(service.targetItem, service.cost);
                }

                return true;
            }
        }
        return false;
    }
}