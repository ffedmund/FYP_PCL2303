using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using FYP;

public class MainMenuManger : MonoBehaviour
{
    [Header("Title Interface")]
    public RectTransform backgroundLTransform;
    public RectTransform backgroundRTransform;
    public RectTransform backgroundLStartPivot;
    public RectTransform backgroundRStartPivot;
    public RectTransform backgroundLEndPivot;
    public RectTransform backgroundREndPivot;
    public Image titleImage;
    public TextMeshProUGUI pressKeyTip;

    [Header("Main Menu Interface")]
    public GameObject menuUI;
    public GameObject sittingAdventurerImage;
    public RectTransform menuPivot;

    [Header("Transition Cover")]
    public Slider transitionCoverSlider;

    bool showingTitle;

    void Awake(){
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(MultiplayerGameManager.Singleton != null)
        {
            Destroy(MultiplayerGameManager.Singleton.gameObject);
        }
    }

    void Start() {
        showingTitle = true;
        menuUI.SetActive(false);
        backgroundLTransform.DOMove(backgroundLEndPivot.position,2);
        backgroundRTransform.DOMove(backgroundREndPivot.position,2);
        titleImage.DOFade(1,1).SetDelay(1).OnComplete(()=>{
            pressKeyTip.enabled = true;
            pressKeyTip.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown&&showingTitle){
            showingTitle = false;
            backgroundLTransform.DOMove(backgroundLStartPivot.position,1);
            backgroundRTransform.DOMove(backgroundRStartPivot.position,1);
            titleImage.DOFade(0,1).OnComplete(()=>{
                if(PlayerPrefs.HasKey("Launched")){
                    HideTitleInterface();
                    menuUI.SetActive(true);
                    sittingAdventurerImage.SetActive(true);
                    menuUI.transform.DOMove(menuPivot.position,1);
                    backgroundRTransform.DOMove(backgroundREndPivot.position,1);
                }else{
                    SceneManager.LoadScene(3);
                    PlayerPrefs.SetInt("Launched",1);
                    PlayerPrefs.Save();
                }
            });
            AudioSourceController.instance.Play("Button1",transform);
            AudioSourceController.instance.SetBGM("Main"+Random.Range(1,3),0.5f); 
            AudioSourceController.instance.currentBGMPlaylist = "Main";
        }
    }

    void HideTitleInterface(){
        backgroundLTransform.gameObject.SetActive(false);
        titleImage.gameObject.SetActive(false);
        pressKeyTip.DOKill();
        pressKeyTip.enabled = false;
    }

    public void SinglePlayerButtonClicked(){
        transitionCoverSlider.DOValue(1,0.5f).OnComplete(()=>{
            SceneManager.LoadScene(2);
        });
    }


    public void MultiPlayerButtonClicked(){
        transitionCoverSlider.DOValue(1,0.5f).OnComplete(()=>{
            SceneManager.LoadScene(5);
        });
    }

    public void CharacterCreatorButtonClicked(){
        transitionCoverSlider.DOValue(1,0.5f).OnComplete(()=>{
            SceneManager.LoadScene(3);
        });
    }

    public void QuitButtonClicked(){
        transitionCoverSlider.DOValue(1,0.5f).OnComplete(()=>{
            Application.Quit();
        });
    }
}
