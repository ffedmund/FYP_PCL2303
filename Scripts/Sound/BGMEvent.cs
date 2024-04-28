using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGMEvent : MonoBehaviour {
    static Dictionary<string,BGMEvent> eventDict;
    public string eventName;
    public string bgmName;
    List<Transform> triggerTransforms = new List<Transform>();
    void Awake(){
        if(eventDict == null){
            eventDict = new Dictionary<string, BGMEvent>();
        }
        if(!eventDict.ContainsKey(eventName)){
            eventDict.Add(eventName,this);
        }
    }

    public static void TriggerEvent(string eventName,Transform triggerTransform){
        if(eventDict.ContainsKey(eventName)){
            BGMEvent bgmEvent = eventDict[eventName];
            if(!bgmEvent.triggerTransforms.Contains(triggerTransform)){
                bgmEvent.triggerTransforms.Add(triggerTransform);
            }
            if(bgmEvent.triggerTransforms.Count > 0){
                AudioSourceController.instance.SetBGM(bgmEvent.bgmName,0);
            }
        }
    }

    public static void RemoveTrigger(string eventName,Transform triggerTransform){
        if(eventDict.ContainsKey(eventName)){
            BGMEvent bgmEvent = eventDict[eventName];
            bgmEvent.triggerTransforms.Remove(triggerTransform);
            if(bgmEvent.triggerTransforms.Count < 1){
                AudioSourceController.instance.FadeOutBGM(bgmEvent.bgmName,2);
            }
        }
    }
}