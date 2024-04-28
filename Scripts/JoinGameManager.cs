using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class JoinGameManager : NetworkBehaviour
{
    public static PlayerRole playerRole;
    public static int terrainSeed;
    public GameObject backgroundWindow;
    public GameObject title;
    public GameObject upTitle;
    public GameObject tarotPerfab;
    public GameObject tarotsHolder;
    public GameObject tarotDescription;
    public Transform skillUI;
    public Transform[] talentChoiceBoxesUI;

    public List<PlayerAbility> talentChoiceList = new List<PlayerAbility>();
    public Sprite[] sprites;

    TextMeshProUGUI titleText;
    Coroutine joinGameProcess;
    int initTitleSize = 75;
    int finalTitleSize = 250;

    //Network Code [Server Only]
    HashSet<ulong> readyPlayerSet = new HashSet<ulong>();

    // Start is called before the first frame update
    async void Start()
    {
        await DataReader.ReadBackgroundDataBase();
        await DataReader.ReadAbilityDataBase();
        await DataReader.ReadTargetDataBase();
        title.SetActive(false);
        title.TryGetComponent(out titleText);
        playerRole = new PlayerRole();
        playerRole.playerTalent.id = -1;
        AssignRandomRole(1);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsClient && !IsHost)
        {
            GetTerrainSeedRpc(NetworkManager.LocalClientId);
        }
        else
        {
            terrainSeed = Random.Range(1, int.MaxValue);
        }
    }

    public void AssignRandomRole(int mode = 0){
        switch(mode){
            case 0:
                playerRole.playerBackground = DataReader.backgorundDictionary[0];
                playerRole.playerSkill = DataReader.skillDictionary[0];

                int backgroundId = 0;
                playerRole.playerBackground = DataReader.backgorundDictionary[backgroundId];
                playerRole.playerSkill = DataReader.skillDictionary[playerRole.playerBackground.id];
                do{
                    int tempRndInt = Random.Range(0,DataReader.talentDictionary.Count);
                    if(!talentChoiceList.Contains(DataReader.talentDictionary[tempRndInt])){
                        talentChoiceList.Add(DataReader.talentDictionary[tempRndInt]);
                    }
                }while(talentChoiceList.Count<3);
                playerRole.playerTargets = new List<PlayerTarget>
                {
                    DataReader.roleTargetDictionary[backgroundId]
                };
                int maximum = 100;
                do{
                    PlayerTarget rndPlayerTarget = DataReader.targetList[Random.Range(0,DataReader.targetList.Count)];
                    if(!playerRole.playerTargets.Contains(rndPlayerTarget)){
                        playerRole.playerTargets.Add(rndPlayerTarget);
                    }
                    maximum--;
                }while(playerRole.playerTargets.Count < 3 && maximum>0);
                break;
            case 1:
                int rndBackgroundIndex = Random.Range(0,DataReader.backgorundDictionary.Count);
                playerRole.playerBackground = DataReader.backgorundDictionary[rndBackgroundIndex];
                playerRole.playerSkill = DataReader.skillDictionary[playerRole.playerBackground.id];
                do{
                    int tempRndInt = Random.Range(0,DataReader.talentDictionary.Count);
                    if(!talentChoiceList.Contains(DataReader.talentDictionary[tempRndInt])){
                        talentChoiceList.Add(DataReader.talentDictionary[tempRndInt]);
                    }
                }while(talentChoiceList.Count<3);
                playerRole.playerTargets = new List<PlayerTarget>
                {
                    DataReader.roleTargetDictionary[rndBackgroundIndex]
                };
                maximum = 100;
                do{
                    PlayerTarget rndPlayerTarget = DataReader.targetList[Random.Range(0,DataReader.targetList.Count)];
                    if(!playerRole.playerTargets.Contains(rndPlayerTarget)){
                        playerRole.playerTargets.Add(rndPlayerTarget);
                    }
                    maximum--;
                }while(playerRole.playerTargets.Count < 3 && maximum>0);
                break;
            default:
                break;
        }
    }

    void ResetText(){
        title.SetActive(false);
        titleText.fontSize = initTitleSize;
        titleText.alpha = 0;
    }

    void SetText(string content){
        title.SetActive(true);
        titleText.SetText(content);
    }

    bool FadeIn(float time){
        titleText.alpha += Time.deltaTime/time;
        if(titleText.alpha >= 1){
            return true;
        }
        return false;
    }

    bool FadeOut(float time){
        titleText.alpha -= Time.deltaTime/time;
        if(titleText.alpha <= 0){
            return true;
        }
        return false;
    }

    void Update()
    {
        if(joinGameProcess != null && Input.GetKeyDown(KeyCode.Escape))
        {   
            StopAllCoroutines();
            joinGameProcess = null;
            playerRole.playerTalent = talentChoiceList[0];
            if(NetworkManager.IsConnectedClient)
            {
                ReadyToGameSceneRpc(NetworkManager.LocalClientId);
            }
            else
            {
                MySceneManager.instance.LoadSceneDirectly(2);
            }
        }
    }

    public void StartJoinGameProcess()
    {
        joinGameProcess = StartCoroutine(SetUpUI());
    }

    IEnumerator SetUpUI(){
        ResetText();
        yield return new WaitForSeconds(1);
        SetText("You are " +("AEIOU".Contains(playerRole.playerBackground.role[0])?"an ":"a ")+playerRole.playerBackground.role);
        while(!FadeIn(2f)){ yield return null;}
        yield return new WaitForSeconds(2.5f);
        while(!FadeOut(2f)){ yield return null;}
        ResetText();
        SetText(playerRole.playerBackground.short_description);
        while(!FadeIn(2f)){ yield return null;}
        yield return new WaitForSeconds(5);
        while(!FadeOut(2f)){ yield return null;}
        ResetText();
        SetText("Here is a gift");
        while(!FadeIn(1f)){ yield return null;}
        yield return new WaitForSeconds(1);
        while(!FadeOut(1f)){ yield return null;}
        ResetText();
        yield return StartCoroutine(ShowTalentUI());
        SetText("Uncover the hidden clues");
        while(!FadeIn(2f)){ yield return null;}
        yield return new WaitForSeconds(2.5f);
        while(!FadeOut(1.5f)){ yield return null;}
        ResetText();
        SetText("And find the crystal ball");
        while(!FadeIn(2f)){ yield return null;}
        yield return new WaitForSeconds(2.5f);
        while(!FadeOut(1.5f)){ yield return null;}
        ResetText();
        SetText("You are the chosen one, our final hope.");
        while(!FadeIn(2.5f)){ yield return null;}
        yield return new WaitForSeconds(2.5f);
        while(!FadeOut(1.5f)){ yield return null;}
        if(NetworkManager.IsConnectedClient)
        {
            ReadyToGameSceneRpc(NetworkManager.LocalClientId);
        }
        else
        {
            MySceneManager.instance.LoadSceneDirectly(2);
        }
        yield return null;
    }

    IEnumerator ShowTalentUI(){
        Hover[] hovers = new Hover[3];
        for(int i = 0; i < 3; i++){
            int temp = i;
            GameObject tarot = Instantiate(tarotPerfab,tarotsHolder.transform);
            tarot.GetComponentInChildren<TextMeshProUGUI>().SetText(talentChoiceList[temp].name);
            tarot.GetComponent<Button>().onClick.AddListener(()=>{
                if(playerRole.playerTalent.id == -1){
                    playerRole.playerTalent = talentChoiceList[temp];
                }
            });
            tarot.transform.GetChild(0).GetComponent<Image>().sprite = sprites[talentChoiceList[temp].iconId];
            tarot.transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,0);
            hovers[temp] = tarot.GetComponent<Hover>();
            hovers[temp].id = temp;
        }
        upTitle.SetActive(true);
        upTitle.GetComponent<TextMeshProUGUI>().alpha = 0;
        for(float i = 0; i < 1; i+=Time.deltaTime){
            upTitle.GetComponent<TextMeshProUGUI>().alpha = i;
            foreach(Transform child in tarotsHolder.transform){
                Color tempColor = child.GetChild(0).GetComponentInChildren<Image>().color;
                child.GetChild(0).GetComponentInChildren<Image>().color = new Color(tempColor.r,tempColor.g,tempColor.b,i);
            }   
            yield return null;
        }
        while(playerRole.playerTalent.id == -1){
            for(int i = 0; i < 3; i++){
                if(hovers[i].onHover){
                    tarotDescription.SetActive(true);
                    tarotDescription.GetComponent<TextMeshProUGUI>().SetText(string.Format(talentChoiceList[hovers[i].id].description,talentChoiceList[hovers[i].id].growthRate));
                    tarotDescription.transform.position = hovers[i].transform.position;
                    tarotDescription.GetComponent<RectTransform>().anchoredPosition = new Vector3(tarotDescription.GetComponent<RectTransform>().anchoredPosition.x,0,0);
                    break;
                }else{
                    tarotDescription.SetActive(false);
                }
            }
            yield return null;
        }
        tarotDescription.SetActive(false);
        for(float i = 0; i < 1; i+=Time.deltaTime){
            upTitle.GetComponent<TextMeshProUGUI>().alpha = 1-i;
            foreach(Transform child in tarotsHolder.transform){
                Color tempColor = child.GetChild(0).GetComponentInChildren<Image>().color;
                child.GetChild(0).GetComponentInChildren<Image>().color = new Color(tempColor.r,tempColor.g,tempColor.b,1-i);
            }   
            yield return null;
        }
        foreach(Transform child in tarotsHolder.transform){
            Destroy(child.gameObject);
        }
        upTitle.SetActive(false);
        yield return null;
    }

    public void LoadScene(){
        MySceneManager.instance.LoadScene(2);
    }

    [Rpc(SendTo.Server)]
    void ReadyToGameSceneRpc(ulong clientId)
    {
        readyPlayerSet.Add(clientId);
        if(readyPlayerSet.Count == NetworkManager.ConnectedClients.Count)
        {
            MySceneManager.instance.LoadNetworkScene(MySceneManager.MyScene.MultiPlayerGameScene,UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.Server)]
    void GetTerrainSeedRpc(ulong clientId)
    {
        SetTerrainSeedRpc(terrainSeed, RpcTarget.Single(clientId,RpcTargetUse.Temp));
    }


    [Rpc(SendTo.SpecifiedInParams)]
    void SetTerrainSeedRpc(int seed, RpcParams rpcParams)
    {
        terrainSeed = seed;
    }

}
