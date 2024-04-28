using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FYP;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private GameObject _settlementCanvas;
    [SerializeField] private TextMeshProUGUI _settlementContent;
    [SerializeField] private Image _progressBar;

    static float _target;

    public enum MyScene {
        MultiPlayerGameScene,
        JoinGameScene,
        Lobby
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public async void LoadScene(int sceneIndex)
    {   
        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneIndex);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        do{
            await Task.Delay(100);
            _target = scene.progress/0.9f;
        }while(scene.progress<0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }

    public async void LoadScene(MyScene myScene)
    {   
        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(myScene.ToString());
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        do{
            await Task.Delay(100);
            _target = scene.progress/0.9f;
        }while(scene.progress<0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }

    public void QuitGameHandler(){
        UIController uiController = FindAnyObjectByType<UIController>();
        if(uiController){
            uiController.BlackCoverFadeIn(()=>Application.Quit());
        }else{
            Application.Quit();
        }
    }

    public async void LoadSceneDirectly(int sceneIndex)
    {   
        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneIndex);
        scene.allowSceneActivation = false;

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
    }

    public void LoadNetworkScene(MyScene scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(),mode);
    }

    public void UnLoadNetworkScene(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.UnloadScene(scene);
    }

    public void EnableSettlementInterface(PlayerManager player){
        _settlementCanvas.SetActive(true);
        _settlementContent.SetText($"{player.playerData.GetInfo().playerBackground.role} Win\nPlayer {player.gameObject.name} put down the crystal ball");
    }

    public void EnableSettlementInterface(string playerName){
        _settlementCanvas.SetActive(true);
        _settlementContent.SetText($"{playerName} Win\n{playerName} put down the crystal ball");
    }

    void Update(){
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount,_target,3*Time.deltaTime);
    }
}
