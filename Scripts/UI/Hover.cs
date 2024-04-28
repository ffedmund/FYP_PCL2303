using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]public int id;
    public bool onHover;
    public Button button;
    public Color defaultColor;
    public Color darkColor;

    private static GameObject _selectedButton;
    public static GameObject SelectedButton
    {
        get { return _selectedButton; }
        set
        {
            if (_selectedButton != value)
            {
                _selectedButton = value;
                OnSelectedButtonChanged?.Invoke(_selectedButton);
            }
        }
    }

    public delegate void SelectedButtonChanged(GameObject newButton);
    public static event SelectedButtonChanged OnSelectedButtonChanged;

    void Start()
    {
        button = GetComponent<Button>();
        defaultColor = button.colors.normalColor;
        darkColor = new Color(0.2f, 0.2f, 0.2f); // Set this to your desired dark color
        button.onClick.AddListener(() => SelectedButton = gameObject);
        OnSelectedButtonChanged += HandleSelectedButtonChanged;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover = true;
        SelectedButton = gameObject;
        transform.DOScale(1.1f,1);
        transform.DOMoveY(Screen.height/2+30,1);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHover = false;
        SelectedButton = null;
        transform.DOScale(1,1);
        transform.DOMoveY(Screen.height/2,1);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    void HandleSelectedButtonChanged(GameObject newButton)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = (newButton == gameObject || newButton == null) ? defaultColor : darkColor;
        button.colors = colors;
        foreach(Transform child in transform){
            if(child.TryGetComponent(out Image image)){
                image.color = colors.normalColor;
            }
            if(child.TryGetComponent(out TextMeshProUGUI text)){
                text.alpha = (newButton == gameObject || newButton == null) ?1:0.1f;
            }
        }
    }

    private void OnDestroy() {
        OnSelectedButtonChanged = null;
        DOTween.Complete(transform);
    }
}