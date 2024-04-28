using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using FYP;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotebookUIController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] RectTransform bookPageTransform;
    public Transform leftFrameTransform;
    public Transform rightFrameTransform;
    public Transform leftTextBox;
    public Transform rightTextBox;
    public TextMeshProUGUI leftTitle;
    public TextMeshProUGUI rightTitle;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public NoteManager noteManager;

    PlayerRole playerRole;
    List<PlayerTarget> playerTargets;
    List<Quest> playerQuests;
    Note currentNote;
    int pageIndex;


    public void SetUp(PlayerRole playerRole, NoteManager noteManager){
        this.playerRole = playerRole;    
        this.noteManager = noteManager;
    }

    public void ShowPage(){
        SetPageIndex(pageIndex);
    }

    public void SetPageIndex(int index){
        rightText.SetText("");
        rightTitle.SetText("");
        switch(index){
            case 1:
                ShowTalentInfo();
                break;
            case 2:
                ShowQuestInfo();
                break;
            case 3:
                ShowHowToPlayInfo();
                break;
            default:
                ShowNoteInfo();
                break;
        }
        if(AudioSourceController.instance){
            AudioSourceController.instance.Play("TurnPage");
        }
        this.pageIndex = index;
        bookPageTransform.GetComponent<Image>().color = Color.white;
        bookPageTransform.anchoredPosition = new Vector2(-278,59);
        bookPageTransform.localScale = Vector3.one;
        bookPageTransform.DOAnchorPos(new Vector2(0,59),0.4f).OnComplete(()=>bookPageTransform.DOAnchorPos(new Vector2(278,59),0.4f));
        bookPageTransform.DOScaleX(0,0.4f).OnComplete(()=>{
            bookPageTransform.DOScaleX(-1,0.4f).OnComplete(()=>{
                bookPageTransform.GetComponent<Image>().DOFade(0,0.2f);
            });
        });
    }
    
    void ShowNoteInfo(){
        leftTitle.SetText("Notes");
        string noteHeaders = "";
        int i = 1;
        foreach(Note note in noteManager.noteDict.Values){
            string colorTag = note.readed?"black":"orange";
            noteHeaders += $"<color={colorTag}><line-indent=15%><link=\"note\">{note.title}</link>...{i++}</color></line-indent>\n";
        }
        leftText.SetText(noteHeaders);
    }

    void ShowTalentInfo(){
        leftTitle.SetText("Abilities");
        leftText.SetText(
            $"Skill\n\t<link=\"skill\">{playerRole.playerSkill.name}</link>...1\nTalent\n\t<link=\"talent\">{playerRole.playerTalent.name}</link>...2"
        );
        ShowRightContent("skill");
    }

    void ShowQuestInfo(){
        leftTitle.SetText("In Progress Quest");
        string questString = "";
        int i = 1;
        if(NetworkManager.Singleton.IsConnectedClient)
        {
            playerQuests = GameManager.instance.localPlayerManager.playerData.quests;
        }
        else
        {
            playerQuests = FindObjectOfType<PlayerManager>().playerData.quests;
        }
        foreach(Quest quest in playerQuests){
            questString += $"{i++}.<link=\"quest\">"+quest.title+"</link>\n";
        }
        leftText.SetText(questString);
        ShowRightContent("");
    }

    void ShowHowToPlayInfo(){
        leftTitle.SetText("Knowledge");
        leftText.SetText("1. <link=\"honor\">Honor</link>\n2. <link=\"quest_info\">Quest</link>\n3. <link=\"npc\">NPC</link>");
        ShowRightContent("");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(leftText, pos, null); 
        TMP_Text textComponent = leftText;
        if (linkIndex == -1)
        {
            linkIndex = TMP_TextUtilities.FindIntersectingLink(rightTitle, pos, null);
            textComponent = rightTitle;
        }
        if (linkIndex == -1)
        {
            linkIndex = TMP_TextUtilities.FindIntersectingLink(rightText, pos, null);
            textComponent = rightText;
        }

        if (linkIndex > -1)
        {
            TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            // Debug.Log(linkInfo.GetLinkText());
            // Debug.Log(linkInfo.GetLinkID());
            if(linkInfo.GetLinkID() == "track_quest"){
                Quest quest = playerQuests.Find(quest => quest.title == linkInfo.GetLinkText().Split("-")[0]);
                playerQuests.Remove(quest);
                playerQuests.Insert(0,quest);
                ShowQuestInfo();
            }else{
                ShowRightContent(linkInfo.GetLinkID(),linkInfo.GetLinkText());
            }
        }
    }

    void ShowRightContent(string id, string title = ""){
        switch(id){
            case "note":
                if(noteManager){
                    string keyword = Regex.Replace(title, @"\p{P}", "").Replace(" ", "_").ToLower();
                    Debug.Log("The keyword is : " + keyword);
                    Note note = noteManager.SearchNote(keyword);
                    string[] noteString = note.GetNote();
                    if(note){
                        currentNote = note;
                        note.readed = true;
                        rightTitle.SetText(noteString[0]);
                        rightText.SetText(noteString[1]);
                        ShowNoteInfo();
                    }
                }
                break;
            case "keyword":
                if(noteManager){
                    string keyword = Regex.Replace(title, @"\p{P}", "").Replace(" ", "_").ToLower();
                    noteManager.AddNote(keyword);
                    string[] noteString = currentNote.GetNote();
                    rightTitle.SetText(noteString[0]);
                    rightText.SetText(noteString[1]);
                    ShowNoteInfo();
                }
                break;
            case "target_0":
                rightTitle.SetText(playerTargets[0].title);
                rightText.SetText(playerTargets[0].shortDescription);
                rightText.SetText(playerTargets[0].detail);
                break;
            case "target_1":
                rightTitle.SetText(playerTargets[1].title);
                rightText.SetText(playerTargets[1].shortDescription);
                rightText.SetText(playerTargets[1].detail);
                break;
            case "target_2":
                rightTitle.SetText(playerTargets[2].title);
                rightText.SetText(playerTargets[2].shortDescription);
                rightText.SetText(playerTargets[2].detail);
                break;
            case "skill":
                rightTitle.SetText(playerRole.playerSkill.name);
                rightText.SetText(playerRole.playerSkill.description);
                break;
            case "talent":
                rightTitle.SetText(playerRole.playerTalent.name);
                rightText.SetText(playerRole.playerTalent.description);
                break;
            case "quest":
                Quest quest = playerQuests.Find(quest => quest.title == title);
                rightTitle.SetText($"<link=\"track_quest\">{title}---({(playerQuests.IndexOf(quest)==0?"Tracked":"")})</link>");
                rightText.SetText(quest.description+$"\nProgress:{quest.goalChecker.currentAmount}/{quest.goalChecker.targetAmount}");
                break;
            case "honor":
                rightTitle.SetText("Honor");
                rightText.SetText("Honor is the level of your adventurer rank. Higher the honor level, more difficult quest and better reward will get.\nYou can see your honor rank at the top left conor.");
                break;
            case "quest_info":
                rightTitle.SetText("Quest");
                rightText.SetText("You can get or report quest at quest board in town, which is nearby the fountain. It will refresh in a period of time. Also, It is the best way to get money and honor.");
                break;
            case "npc":
                rightTitle.SetText("NPC");
                rightText.SetText("You can find many firendly npc in town. Talk with them, increase the friendship between them, can help you get information about anything you want. Also some of them can trade! Go get some good stuff.");
                break;
            default:
                rightTitle.SetText("");
                rightText.SetText("");
                break;
        }
    }
}
