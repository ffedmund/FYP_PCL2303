using FYP;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowController : MonoBehaviour {
    public UIController uiController;
    public CameraHandler cameraHandler;
    public GameObject content;
    public GameObject leftFrame;
    public GameObject rightFrame;
    public GameObject infoFrame;
    public TextMeshProUGUI leftFrameTitle;
    public TextMeshProUGUI rightFrameTitle;
    public GameObject rightFrameTitleObject;
    [Header("Inventory UI Elements")]
    public GameObject inventoryUI;
    public GameObject money;
    public GameObject inventoryNavigation;

    [Header("Player's UI Elements")]
    public GameObject statusUI;
    public GameObject statsUI;
    public GameObject equipmentUI;

    [Header("Artifact UI Elements")]
    public GameObject artifactUI;
    public ArtifactSeriesUIController artifactSeriesUI;

    [Header("Map UI Elements")]
    public GameObject mapUI;

    [Header("Options UI Elements")]
    public GameObject optionList;
    public GameObject settingsWindow;
    public GameObject hintsWindow;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider mouseSensitivitySlider;


    private void Start() {
        uiController = FindAnyObjectByType<UIController>();
        cameraHandler = FindAnyObjectByType<CameraHandler>();
        bgmSlider.onValueChanged.AddListener(delegate { SaveSetting("BGMVolume",bgmSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SaveSetting("SFXVolume",sfxSlider.value); });
        mouseSensitivitySlider.onValueChanged.AddListener(delegate { SaveSetting("MouseSensitivity",mouseSensitivitySlider.value); });
        InventoryWindow();
    }

    private void OnEnable() {
        if(uiController){
            uiController.UpdateUI();
        }
    }

    public void InventoryWindow(){
        ResetAllElement();
        uiController.ChangeShowingInventory(1);
        uiController.UpdateUI();
        inventoryUI.SetActive(true);
        inventoryNavigation.SetActive(true);
        statusUI.SetActive(true);
        statsUI.SetActive(true);
        equipmentUI.SetActive(true);
        money.SetActive(true);
        infoFrame.SetActive(true);
        leftFrameTitle.SetText("");
        rightFrameTitle.SetText("Status");
        ButtonSound();
    }

    public void ArtifactWindow(){
        ResetAllElement();
        uiController.ChangeShowingInventory(2);
        uiController.UpdateUI();
        inventoryUI.SetActive(true);
        artifactUI.SetActive(true);
        statusUI.SetActive(true);
        statsUI.SetActive(true);
        infoFrame.SetActive(true);
        leftFrameTitle.SetText("Artifacts");
        rightFrameTitle.SetText("Artifact Series");
        artifactSeriesUI.UpdateUI(uiController.playerInventory.GetComponent<ArtifactAbilityController>());
        ButtonSound();
    }

    public void MapWindow(){
        ResetAllElement();
        leftFrame.SetActive(false);
        mapUI.SetActive(true);
        rightFrameTitle.SetText("Map");
        ButtonSound();
    }

    public void OptionsWindow(){
        ResetAllElement();
        leftFrameTitle.SetText("Options");
        rightFrameTitleObject.SetActive(false);
        optionList.SetActive(true);
        ButtonSound();
    }

    public void SettingsWindow(){
        OptionsWindow();
        rightFrameTitle.SetText("Settings");
        settingsWindow.SetActive(true);
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        ButtonSound();
    }

    public void HintsWindow(){
        OptionsWindow();
        rightFrameTitle.SetText("Settings");
        hintsWindow.SetActive(true);
        ButtonSound();
    }

    public void ExitGameButtonClicker(){
        if(MySceneManager.instance != null){
            MySceneManager.instance.QuitGameHandler();
        }
    }

    public void BackToMainButtonClicker()
    {
        if(MySceneManager.instance != null)
        {
            NetworkManager.Singleton?.Shutdown();
            MySceneManager.instance.LoadScene(0);
        }
    }

    public void SaveSetting(string key,float value){
        if(PlayerPrefs.HasKey(key)){
            PlayerPrefs.SetFloat(key,value);
        }
        PlayerPrefs.Save();
        if(AudioSourceController.instance){
            AudioSourceController.instance.SetPlayerPrefs();
        }
        if(cameraHandler){
            cameraHandler.SetPlayerPref();
        }
    }

    void ButtonSound(){
        if(AudioSourceController.instance != null){
            AudioSourceController.instance.Play("On");
        }
    }

    void ResetAllElement(){
        rightFrameTitleObject.SetActive(true);
        leftFrame.SetActive(true);
        rightFrame.SetActive(true);

        inventoryUI.SetActive(false);
        inventoryNavigation.SetActive(false);
        statusUI.SetActive(false);
        statsUI.SetActive(false);
        equipmentUI.SetActive(false);
        artifactUI.SetActive(false);
        money.SetActive(false);
        mapUI.SetActive(false);
        infoFrame.SetActive(false);
        optionList.SetActive(false);
        settingsWindow.SetActive(false);
        hintsWindow.SetActive(false);
    }
}