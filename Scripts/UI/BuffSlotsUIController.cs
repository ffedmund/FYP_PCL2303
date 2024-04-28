using System.Collections.Generic;
using DG.Tweening;
using FYP;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlotsUIController : MonoBehaviour
{
    public PlayerStats playerStats;
    public Dictionary<string,GameObject> buffIconDict;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        buffIconDict = new Dictionary<string, GameObject>();
    }

    public void CreateBuffIcon(Buff buff, float duration){
        Debug.Log(buff.uuid);
        if(buffIconDict.ContainsKey(buff.uuid)){
            DOTween.Kill(buffIconDict[buff.uuid].GetComponent<Image>());
            Destroy(buffIconDict[buff.uuid]);
            buffIconDict.Remove(buff.uuid);
        }
        Sprite sprite = buff.icon;
        GameObject imageObject = new GameObject("Image");
        imageObject.transform.SetParent(transform);
        Image image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        buffIconDict.Add(buff.uuid,imageObject);
        Tween tween = image.DOFade(0,duration)
        .OnComplete(()=>{
            buffIconDict.Remove(buff.uuid);
            Destroy(imageObject);
        });
    }

}
