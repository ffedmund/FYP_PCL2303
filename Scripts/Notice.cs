
using TMPro;
using UnityEngine;

public class Notice : MonoBehaviour {
    public SpriteRenderer icon;
    public TextMeshPro text;

    [Header("MiniMap")]
    public SpriteRenderer minimapIcon;
    public Sprite[] iconArray;

    public bool setDefault;

    void Start() {
        if(setDefault){
            SetNotice(0,"",false);
        }
    }

    public void UpdateNoticeDirection(Vector3 direction){
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.eulerAngles = new Vector3(0,targetRotation.eulerAngles.y,0);
        transform.rotation = targetRotation;
    }

    public void SetNotice(Sprite image, string textContent = "", bool setMinimapIcon = true){
        icon.color = Color.white;
        icon.sprite = image;
        if(setMinimapIcon){
            minimapIcon.sprite = image;
        }
        SetText(textContent);
    }

    public void SetNotice(int iconIndex, string textContent = "", bool setMinimapIcon = true){
        icon.color = Color.white;
        icon.sprite = iconArray[iconIndex];
        if(setMinimapIcon){
            minimapIcon.sprite = iconArray[iconIndex];
        }
        SetText(textContent);
    }

    public void SetColor(Color color){
        icon.color = color;
    }

    public void ClearNotice(){
        icon.sprite = null;
        if(minimapIcon != null){
            minimapIcon.sprite = null;
        }
        text.SetText("");
    }

    void SetText(string textContent){
        if(text != null){
            text.enabled = true;
            text.SetText(textContent);
        }
    }
}