using System.Collections;
using System.Collections.Generic;
using FYP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopUpController : MonoBehaviour
{
    public enum PopUpIcon{
        Money,
        Honor,
        Ticket,
        Resources
    }

    [SerializeField] GameObject itemPopUpPrefab;
    [SerializeField] Sprite[] popUpIconSprites;

    public void NewPopUp(Item item, int amount = 1){
        GameObject newPopUp = Instantiate(itemPopUpPrefab,transform);
        newPopUp.GetComponentInChildren<TextMeshProUGUI>().SetText(item.name + ((amount>1)?$" x{amount}":""));
        newPopUp.transform.GetChild(0).GetComponent<Image>().sprite = item.itemIcon;
    }

    public void NewPopUp(PopUpIcon popUpIcon, int amount = 1){
        NewPopUp(popUpIconSprites[(int)popUpIcon], amount);
    }

    public void NewPopUp(Sprite iconSprite, int amount = 1){
        GameObject newPopUp = Instantiate(itemPopUpPrefab,transform);
        newPopUp.GetComponentInChildren<TextMeshProUGUI>().SetText((amount != 0)?((amount>1?" +":" ")+$"{amount}"):"");
        newPopUp.transform.GetChild(0).GetComponent<Image>().sprite = iconSprite;
    }

}
