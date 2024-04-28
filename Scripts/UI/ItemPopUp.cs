using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    Image background;
    Image icon;
    TextMeshProUGUI text;

    void Start(){
        background = GetComponent<Image>();
        icon = transform.GetChild(0).GetComponent<Image>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        Destroy(gameObject,3);
    }

    void Update(){
        background.CrossFadeAlpha(0, 3.0f, false);
        icon.CrossFadeAlpha(0, 3.0f, false);
        text.CrossFadeAlpha(0,3.0f,false);
    }
}
