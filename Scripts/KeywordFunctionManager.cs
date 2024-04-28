using System;
using System.Collections.Generic;
using UnityEngine;

public class KeywordFunctionManager : MonoBehaviour {
    
    public static KeywordFunctionManager instance;
    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    public Dictionary<string,Func<object,string>> keywordFunctionDict = new Dictionary<string, System.Func<object,string>>();
}