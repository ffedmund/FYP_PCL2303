using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FYP;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class QuestBoardUI : MonoBehaviour {
    
    public Transform questUIPrefabs;
    public UIController uiController;
    public QuestBoard questBoard;
    public int questRefreshFrequency;
    public float previousTime = 0;

    [Header("Quest Detail Window")]
    [SerializeField] GameObject questDetailWindow;
    [SerializeField] GameObject cover;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI rankRequirement;
    [SerializeField] Button confirmButton;
    [SerializeField] Transform rewardSlots;
    [SerializeField] GameObject rewardSlotPrefab;
    [SerializeField] TextMeshProUGUI honorAmountText;
    [SerializeField] TextMeshProUGUI monetAmountText;

    [Header("Quest Poster")]
    [SerializeField] Sprite[] posterSprites;

    Transform interactingQuestBoard;
    PlayerManager playerManager;

    async Task Setup(PlayerData playerData){
        if(!questBoard.inited){
            QuestList questList;
            QuestGiver questGiver;
            QuestReceiver questReceiver;
            TryGetComponent(out questList);
            questList.Init();
            if(!TryGetComponent(out questGiver) || !TryGetComponent(out questReceiver)){
                enabled = false;
                return;
            }
            questReceiver.SetupQuest(questList);
            questGiver.SetupQuest(questList);
            await Task.Run(() => {
                questBoard = new QuestBoard(questGiver,questReceiver,questList,playerData);
            });
        }
    }

    void OnDisable() {
        if(interactingQuestBoard){
            interactingQuestBoard.TryGetComponent(out Collider collider);
            collider.enabled = true;
            playerManager.canAttack = true;
            interactingQuestBoard = null;
            playerManager = null;
        }
    }

    void RefreshQuest(){
        if(IsTimeToRefresh()){
            questBoard.UpdateQuests();
            previousTime = Time.time;
        }
    }

    bool IsTimeToRefresh(){
        return Time.time - previousTime >= questRefreshFrequency || previousTime == 0;
    }

    void ShowQuestDetail(int index)
    {
        cover.SetActive(true);
        Quest quest = questBoard.currentQuestList[index];
        title.SetText(quest.title);
        rankRequirement.SetText("RANK\n"+quest.honorRank.ToString());
        description.SetText(quest.description);
        questDetailWindow.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => questBoard.questGiver.AcceptQuest(quest));
        confirmButton.onClick.AddListener(() => UpdateQuestWindow());
        confirmButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(quest.isActive?"ACCEPTED":"ACCEPT");
        confirmButton.onClick.AddListener(() => UpdateQuestWindow());
        confirmButton.onClick.AddListener(() => ShowQuestDetail(index));
        
        if(rewardSlots.childCount > 2)
        {
            Destroy(rewardSlots.GetChild(rewardSlots.childCount-1).gameObject);
        }
        monetAmountText.SetText(quest.moneyReward.ToString());
        honorAmountText.SetText(quest.honorReward.ToString());
        if(quest.itemReward != "" && ItemDatabase.instance)
        {
            GameObject awardUI = Instantiate(rewardSlotPrefab,rewardSlots);
            awardUI.GetComponentInChildren<Image>().sprite = ItemDatabase.instance.GetItemById(quest.itemReward).itemIcon;
            awardUI.GetComponentInChildren<TextMeshProUGUI>().SetText("x"+quest.itemRewardAmount);
        }
    }

    public void UpdateQuestWindow(){
        int questPostUINumber = transform.childCount;
        while(questPostUINumber!=questBoard.currentQuestList.Count){
            if(questPostUINumber<questBoard.currentQuestList.Count){
                Instantiate(questUIPrefabs,transform).GetComponent<Image>().sprite = posterSprites[UnityEngine.Random.Range(0,posterSprites.Length)];
                questPostUINumber++;
            }else{
                Destroy(transform.GetChild(0).gameObject);
                questPostUINumber--;
            }
        }

        for(int i = 0; i < questBoard.currentQuestList.Count; i++){
            Transform questUI = transform.GetChild(i);
            Transform completeText = questUI.Find("CompleteText");
            Quest quest = questBoard.currentQuestList[i];
            if(!quest.isFinished){
                questUI.Find("Title").GetComponent<TextMeshProUGUI>().SetText(quest.title);
                questUI.Find("RankText").GetComponent<TextMeshProUGUI>().SetText(quest.honorRank.ToString());
                questUI.Find("ScrollArea").Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().SetText(quest.description);
                int currentQuestIndex = i;
                if(questUI.TryGetComponent(out Button button))
                {
                    button.onClick.RemoveAllListeners();
                    if(questBoard.questList.CanReportQuest(quest) && quest.targetNPC == ""){
                        button.onClick.AddListener(()=>{
                            questBoard.questReceiver.ReportQuest(quest,playerManager);
                            UpdateQuestWindow();
                        });
                        completeText.gameObject.SetActive(true);
                        completeText.GetComponentInChildren<TextMeshProUGUI>().SetText("Complete");
                    }else if(quest.isActive){
                        completeText.gameObject.SetActive(true);
                        completeText.GetComponentInChildren<TextMeshProUGUI>().SetText("Accepted");
                    }else{
                        button.onClick.AddListener(()=>{ShowQuestDetail(currentQuestIndex);});
                        completeText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    questUI.AddComponent<Button>().onClick.AddListener(()=>{
                        ShowQuestDetail(currentQuestIndex);
                    });
                }
                questUI.gameObject.SetActive(true);
            }else{
                questUI.gameObject.SetActive(false);

            }
        }
    }

    public async void InteractingQuestBoard(Transform transform,PlayerManager playerManager){
        interactingQuestBoard = transform;
        transform.TryGetComponent(out Collider collider);
        collider.enabled = false;
        this.playerManager = playerManager;
        playerManager.canAttack = false;
        await Setup(playerManager.playerData);
        questDetailWindow.SetActive(false);
        cover.SetActive(false);
        RefreshQuest();
        UpdateQuestWindow();
    } 
}