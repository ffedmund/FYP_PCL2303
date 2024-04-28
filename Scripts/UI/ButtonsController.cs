using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ButtonsController : MonoBehaviour{
    GameObject acitveButton;

    public void OnSelectButton(GameObject clickedButton){
        if(clickedButton != acitveButton){
            acitveButton = clickedButton;
            clickedButton.GetComponent<Image>().color = clickedButton.GetComponent<Button>().colors.selectedColor;
            foreach(Transform child in transform){
                if(child.gameObject != acitveButton && child.TryGetComponent(out Image image) && child.TryGetComponent(out Button button)){
                    image.color = button.colors.normalColor;
                }
            }
        }
    }
}