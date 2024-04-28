using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EyeOpenSceneTransition : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Image coverUp;
    [SerializeField] Image coverDown;
    [SerializeField] Image cover;
    [SerializeField] GoddessChatController goddessChatController;

    private void Start() {
        goddessChatController.Chat();
        // coverUp.DOFillAmount(0.8f,0.5f);
        // coverDown.DOFillAmount(0.8f,0.5f);
        // coverUp.DOFillAmount(1f,1f).SetDelay(0.5f).OnComplete(()=>{
        //     coverUp.DOFillAmount(0,1.5f);
        // });
        // coverDown.DOFillAmount(1f,1f).SetDelay(0.5f).OnComplete(()=>{
        //     coverDown.DOFillAmount(0,1.5f);
        //     goddessChatController.Chat();
        // });

        cover.DOFade(0.2f,5f).OnComplete(()=>{
            cover.gameObject.SetActive(false);
        });
    }

    public void CloseEye(Action callback){
        cover.gameObject.SetActive(true);
        cover.DOFade(1,2f).OnComplete(() => {
            callback();
            cover.gameObject.SetActive(false);
        });
    }
}
