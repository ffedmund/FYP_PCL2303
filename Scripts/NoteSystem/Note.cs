using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FYP{
    public enum NoteType{
        Clue,
        Story
    }

    [System.Serializable]
    public struct Paragraph{
        public bool unlocked;
        public string title;
        public bool enableInlineCommand;
        [TextArea(2,5)]
        public string content;
        public string[] containKeywords;
    }

    [CreateAssetMenu(menuName = "Notebook System/Note")]
    public class Note : ScriptableObject {
        public string keyword;
        public string title;
        public Sprite icon;
        [TextArea(2,5)]
        public string description;
        public string[] keywordsInDescription;
        public Paragraph[] paragraphs;
        public bool readed;

        string[] GetKeywordTextArray(string[] keywords){
            NoteManager noteManager = FindAnyObjectByType<NoteManager>();
            string[] keywordTextArray = new string[keywords.Length*2];
            if(noteManager != null && keywords.Length > 0){
                int j = 0;
                for(int i = 0; i < keywordTextArray.Length; i++){         
                    if(i%2 == 0){
                        keywordTextArray[i] = noteManager.noteDict.ContainsKey(keywords[j].Replace(" ","_"))?"black":"blue";
                    }else{
                        keywordTextArray[i] = keywords[j++];
                    }
                }
            }   
            return keywordTextArray;
        }

        string GetCommandData(string currentText){
            return Regex.Replace(currentText, @"\{(.+?)(?::(.+?))?\}", m => {
                string funcID = m.Groups[1].Value;
                string input = m.Groups[2].Value;
                return KeywordFunctionManager.instance.keywordFunctionDict[funcID](input);
            });
        }

        public bool UnlockParagraph(int index){
            if(paragraphs.Length > index && !paragraphs[index].unlocked){
                paragraphs[index].unlocked = true;
                readed = false;
                return true;
            }
            return false;
        }

        public string[] GetNote(){
            string[] keywordTextArray = GetKeywordTextArray(keywordsInDescription);
            string noteContent = "<line-indent=15%><size=80%>" + string.Format(description,keywordTextArray) + "</line-indent>\n";
            
            foreach(Paragraph paragraph in paragraphs){
                if(paragraph.unlocked){
                    keywordTextArray = GetKeywordTextArray(paragraph.containKeywords);
                    string content = (keywordTextArray.Length > 0)?string.Format(paragraph.content,keywordTextArray):paragraph.content;
                    if(paragraph.enableInlineCommand){
                        content = GetCommandData(content);
                    }
                    noteContent += string.Format
                    (
                        "\n<size=105%><b>{0}</b>\n" +
                        "<line-indent=15%><size=80%>{1}</line-indent>\n"
                    ,paragraph.title,content);
                }
            }

            string[] noteString = {
                title,noteContent
            };

            return noteString;
        }
    }
}