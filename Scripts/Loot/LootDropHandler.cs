using System.Collections.Generic;
using System.Linq;
using FYP;
using Unity.Netcode;
using UnityEngine;

public class LootDropHandler : MonoBehaviour {
    [SerializeField] LootList lootList;
    [SerializeField] GameObject particleEffectPrefab;
    int totalWeight;
    bool droped;

    void Start(){
        if(lootList){
            totalWeight = lootList.list.Sum(loot => loot.weight);
        }
    }

    void OnEnable() {
        droped = false;
    }

    public void SetLootList(LootList lootList){
        this.lootList = lootList;
        totalWeight = lootList.list.Sum(loot => loot.weight);
    }

    public void DropLoot(){
        if(droped){
            return;
        }
        int randomNumber = Random.Range(0,totalWeight);
        int weightSum = 0;
        foreach(var loot in  lootList.list){
            weightSum += loot.weight;
            if(weightSum > randomNumber){
                LootSpawn(loot.lootItem);
                break;
            }
        }
        droped = true;
    }

    void LootSpawn(Item item){
        if(item != null){
            if(NetworkManager.Singleton){
                if(NetworkManager.Singleton.IsServer){
                    SpawnNetworkItemObject(item);
                }
            }else{
                SpawnLocalItemObject(item);
            }
        }
    }

    void SpawnNetworkItemObject(Item item){
        NetworkObject poolObject = NetworkObjectPool.Singleton.GetNetworkObject("item_drop",transform.position,Quaternion.identity);
        if(item == null){
            return;
        }
        if(poolObject){
            NetworkItem networkItem = poolObject.GetComponentInChildren<NetworkItem>();
            poolObject.Spawn();
            if(NetworkItemReferences.Singleton){
                networkItem.SetItemId(NetworkItemReferences.Singleton.GetId(item));
                networkItem.SetParticle(true);
            }
        }
    }

    void SpawnLocalItemObject(Item item){
        GameObject lootObject = new GameObject();
        lootObject.transform.position = transform.position;
        Instantiate(particleEffectPrefab,lootObject.transform);
        if(item is MaterialItem){
            lootObject.AddComponent<MaterialPickUp>().material = (MaterialItem)item;
        }
        if(item is WeaponItem){
            lootObject.AddComponent<WeaponPickUp>().weapon = (WeaponItem)item;
        }
        if(item is ArtifactItem){
            lootObject.AddComponent<ArtifactPickUp>().artifact = (ArtifactItem)item;
        }
        if(item is EquipmentItem){
            lootObject.AddComponent<EquipmentPickUp>().equipment = (EquipmentItem)item;
        }
        if(lootObject.TryGetComponent(out InteractableScript interactable)){
            lootObject.AddComponent<SphereCollider>().isTrigger = true;
            lootObject.layer = 13;
            interactable.interactableText = "Pick Up";
            lootObject.tag = "Interactable";
        }
    }
    
}