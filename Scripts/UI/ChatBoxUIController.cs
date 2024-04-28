using System;
using FYP;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Text.RegularExpressions;

public class ChatBoxUIController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI chatContent;
    [SerializeField] GameObject navigationBar;
    [SerializeField] Transform answerButtonSlot;
    [SerializeField] GameObject answerButtonPrefab;
    [SerializeField] Button closeButton;
    [SerializeField] Slider friendshipSlider;
    UIController uiController;
    Button defaultAnswerButton;
    Coroutine chatCoroutine = null;
    string currentText = "";

    void Awake(){
        TryGetComponent(out defaultAnswerButton);
        uiController = FindAnyObjectByType<UIController>();
    }

    public string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        string nameTag = $"<b>Unkown:</b>\n";
        return nameTag+new string(stringChars);
    }

    IEnumerator GlitchText(){
        string glitchString = GenerateRandomString(50);
        for (int i = 0; i <= glitchString.Length; i++) {
            chatContent.text = glitchString;
            chatContent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.04f); // adjust the delay as needed
        }
    }
    
    public void SetChatbox(string speakerName ,string content,bool showChatbox = true,bool glitchText = false ,string colorHexCode = ""){
        string nameTag = $"<b>{speakerName}:</b>\n";
        currentText = nameTag+content;
        if(colorHexCode != ""){
            currentText = $"<color={colorHexCode}>{currentText}</color={colorHexCode}>";
        }
        chatContent.maxVisibleCharacters = 0;
        currentText = Regex.Replace(currentText, @"\{(.+?)(?::(.+?))?\}", m => {
            string funcID = m.Groups[1].Value;
            string input = m.Groups[2].Value;
            return KeywordFunctionManager.instance.keywordFunctionDict[funcID](input);
        });
        gameObject.SetActive(showChatbox);
        chatCoroutine = StartCoroutine(ShowText(currentText,nameTag.Length));
        if(glitchText){
           StartCoroutine(GlitchText());
        }
    }

    public void SetCloseButton(Action callback){
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => {
            if(chatCoroutine != null){
                CompleteChat();
            }
            callback();
        });
    }

    public void CompleteChat() {
    if (chatCoroutine != null) {
            StopCoroutine(chatCoroutine);
            chatContent.text = currentText;
            chatContent.maxVisibleCharacters = currentText.Length;
            chatCoroutine = null;
        }   
    }

    IEnumerator ShowText(string fullText,int startPos = 0) {
        for (int i = startPos; i <= fullText.Length; i++) {
            chatContent.text = fullText;
            chatContent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.05f); // adjust the delay as needed
        }
        chatCoroutine = null;
    }

    public void SetDefaultAnswerListener(Action callback, bool showFullContent = true){
        if(!defaultAnswerButton){
            TryGetComponent(out defaultAnswerButton);
        }
        Debug.Log("Coroutine: " + chatCoroutine);
        defaultAnswerButton.onClick.RemoveAllListeners();
        defaultAnswerButton.onClick.AddListener(()=>{
            if(chatCoroutine != null && showFullContent){
                CompleteChat();
            }else{
                callback();
            }
        });
    }

    public GameObject SetAnswerButton(int index, string answeringChoice, Action responseAction){
        GameObject answerButtonObject;
        if(answerButtonSlot.childCount <= index){
            answerButtonObject = Instantiate(answerButtonPrefab,answerButtonSlot);
            answerButtonObject.GetComponentInChildren<TextMeshProUGUI>().SetText(answeringChoice);
        }else{
            answerButtonObject = answerButtonSlot.GetChild(index).gameObject;
            answerButtonObject.SetActive(true);
            answerButtonObject.GetComponentInChildren<TextMeshProUGUI>().SetText(answeringChoice);
        }
        answerButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
        answerButtonObject.GetComponent<Button>().onClick.AddListener(()=>responseAction());
        return answerButtonObject;
    }

    public void SetResponse(Action<string,int> responseAction, Choice[] answeringChoices = null){
        ClearAnswerListener();
        bool hasChoices = answeringChoices != null && answeringChoices.Length>0;
        SetDefaultAnswerListener(() => {
            if(!hasChoices){
                responseAction("",0);
            }
        });
        SetCloseButton(() => {
            if(!hasChoices){
                responseAction("",0);
            }
        });
        if(answeringChoices != null && answeringChoices.Length > 0){
            for(int i = 0; i < answeringChoices.Length; i++){
                Choice choice = answeringChoices[i];
                string ans = choice.content;
                SetAnswerButton(i,ans,()=>{
                    CompleteChat();
                    responseAction(ans,choice.extraFirendshipValue);
                });
            }
            for(int i = answerButtonSlot.childCount; i > answeringChoices.Length; i--){
                answerButtonSlot.GetChild(i-1).gameObject.SetActive(false);
            }
        }else{
            ClearAnswerListener();
        }
    }

    public void SetFirendshipSlider(int orignalValue, int newValue){
        friendshipSlider.value = orignalValue;
        friendshipSlider.DOValue(newValue,1);
    }

    public void UpdateFirendshipSlider(int newValue){
        friendshipSlider.DOValue(newValue,1);
    }

    public void ClearChatboxListener(){
        ClearDefaultAnswerListener();
        ClearAnswerListener();
        ClearCloseButtonListener();
    }

    public void HideChatbox(){
        gameObject.SetActive(false);
        ClearChatboxListener();
        ClearChatbox();
    }

    protected void ClearChatbox(){
        chatContent.SetText("");
    }

    protected void ClearDefaultAnswerListener(){
        Debug.Log("Default Answer Button :" + defaultAnswerButton);
        defaultAnswerButton.onClick.RemoveAllListeners();
    }

    protected void ClearAnswerListener(){
        foreach(Transform answerButton in answerButtonSlot){
            if(answerButton.gameObject.activeSelf){
                answerButton.gameObject.SetActive(false);
                answerButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    protected void ClearCloseButtonListener(){
        closeButton.onClick.RemoveAllListeners();
    }

}   