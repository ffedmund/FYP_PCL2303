using UnityEngine;
using FYP;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BuffManager : MonoBehaviour {
    static Dictionary<CharacterStats,Dictionary<string,Buff>> buffCharacterDict;
    
    private void Start() {
        buffCharacterDict = new Dictionary<CharacterStats, Dictionary<string,Buff>>();
    }

    private void Update() {
        if(buffCharacterDict.Count > 0){
            List<CharacterStats> noBuffCharacterList = new List<CharacterStats>();
            foreach(CharacterStats character in buffCharacterDict.Keys){
                List<string> completeBuffIdList = new List<string>();
                foreach(Buff buff in buffCharacterDict[character].Values){
                    if(!buff.UpdateBuff(Time.deltaTime,character)){
                        completeBuffIdList.Add(buff.uuid);
                        if(character is PlayerStats){
                            ((PlayerStats)character).UpdateSprintSpeed();
                        }
                    }
                }
                foreach(string id in completeBuffIdList){
                    Destroy(buffCharacterDict[character][id]);
                    buffCharacterDict[character].Remove(id);
                }
                if(buffCharacterDict[character].Count == 0){
                    noBuffCharacterList.Add(character);
                }
            }
            foreach(CharacterStats character in noBuffCharacterList){
                buffCharacterDict.Remove(character);
            }
        }
    }

    static Buff SetNewBuff(Buff buff, float duration){
        Buff instance = Instantiate(buff);
        instance.duration = duration;
        return instance;
    }

    public static void SetBuff(Buff buff, CharacterStats target, float duration){
        if(!buffCharacterDict.ContainsKey(target)){
            buffCharacterDict.Add(target,new Dictionary<string, Buff>());
        }
        if(buffCharacterDict[target].ContainsKey(buff.uuid)){
            buffCharacterDict[target][buff.uuid].duration = duration;
        }else{
            Buff instance = SetNewBuff(buff,duration);
            buffCharacterDict[target].Add(instance.uuid,instance);
            Debug.Log(instance);
            instance.Active(target);
            if(target is PlayerStats){
                ((PlayerStats)target).UpdateSprintSpeed();
            }
        }
    }

    public static bool HaveBuff(CharacterStats character){
        return buffCharacterDict.ContainsKey(character);
    }
}