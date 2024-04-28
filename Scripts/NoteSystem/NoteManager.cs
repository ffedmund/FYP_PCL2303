using System.Collections.Generic;
using System.Linq;
using FYP;
using UnityEngine;
using Unity.Netcode;

public class NoteManager : PlayerSetupBehaviour {
    public Note playerNote;
    [Header("Default Notes")]
    public List<Note> defaultActiveNotes;
    [Header("All Notes Reference")]
    public Note[] notes;

    public Dictionary<string, Note> noteDict = new Dictionary<string, Note>();

    UIController uiController;

    void Start(){
        if(!NetworkManager.Singleton){
            Setup(FindAnyObjectByType<PlayerManager>());
        }
    }

    public override void Setup(PlayerManager playerManager){
        PlayerRole playerRole = playerManager.playerData.GetInfo();
        uiController = FindAnyObjectByType<UIController>();
        NotebookUIController notebookUIController = uiController.notebookUI.GetComponentInChildren<NotebookUIController>(true);
        notebookUIController.SetUp(playerRole,this);
        foreach(Note note in defaultActiveNotes){
            AddNote(note);
        }
        if(playerRole.playerTargets != null){
            CreatePlayerBackgroundNote(playerRole.playerBackground);
        }
        if(KeywordFunctionManager.instance != null){
            KeywordFunctionManager.instance.keywordFunctionDict.Add("unlock_note",(object value)=>{
                try{
                    if(value is string){
                        this.AddNote((string)value);
                    }
                    return "";
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
            KeywordFunctionManager.instance.keywordFunctionDict.Add("unlock_paragraph",(object value)=>{
                try{
                    if(value is string){
                        string[] input = ((string)value).Split(":");
                        if(input.Length<2){
                            return "";
                        }
                        if(!this.noteDict.ContainsKey(input[0])){
                            this.AddNote(input[0]);
                        }
                        if(this.noteDict[input[0]].UnlockParagraph(int.Parse(input[1]))){
                            this.uiController.SetProgressTitle("Paragraph Found");
                        }
                    }
                    return "";
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
        }
    }

    void CreatePlayerBackgroundNote(PlayerBackground playerBackground){
        Note note = new Note
        {
            paragraphs = new Paragraph[3],
            description = playerBackground.description,
            title = "Start of Journey",
            keyword = "start_of_journey"
        };
        playerNote = note;
        if(playerNote.title != null){
            AddNote(note);
        }
    }

    public void AddNote(Note note){
        if(!noteDict.ContainsKey(note.keyword)){
            Debug.Log("Add new Note");
            Note noteCopy = Instantiate(note);
            noteDict.Add(noteCopy.keyword,noteCopy);
            uiController.ShowNoteNotice();
            uiController.SetProgressTitle("Note Found");
        }
    }

    public void AddNote(string keyword){
        for(int i = 0; i < notes.Length; i ++){
            if(notes[i].keyword == keyword){
                AddNote(notes[i]);
            }
        }
    }

    public Note SearchNote(string keyword){
        if(noteDict.ContainsKey(keyword)){
            return noteDict[keyword];
        }
        return null;
    }

    public bool HaveUnreadContent(){
        return noteDict.Values.ToList().Find(note => !note.readed);
    }

    public void AddContentToParagraph(string keyword, int index, string content){
        if(noteDict.ContainsKey(keyword)){
            noteDict[keyword].paragraphs[index].content += content + "\n";
        }
    }
}