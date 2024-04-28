using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{

    [SerializeField] Button serverBtn;
    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;

    void Awake(){
        serverBtn.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartServer();
            gameObject.SetActive(false);
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
    }
}
