using System.Collections.Generic;
using UnityEngine;
using FYP;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Netcode;
using System.Collections;

public class ItemGenerator : MonoBehaviour
{
    public enum ItemInitMovement{
        None,
        GoUp,
        FallDown,
        MoveFront,
        MoveBack,
        MoveLeft,
        MoveRight
    }

    [Header("Generate Setting")]
    public Vector3Int range;
    public Vector3 pivot;
    public bool setParent;
    public bool useRaycast;
    public LayerMask layerMask;
    [Header("Item Setting")] 
    public Item item;
    public ItemInitMovement itemInitMovement;
    public float extraMovingDistance;
    public bool isItemDealDamage;
    public bool isItemDestroyByTime;
    public float destroyTime;
    [Header("Generator Object Setting")]
    public Transform shakeTransform;

    public System.Random random;
    public List<GameObject> generatedItemList = new List<GameObject>();

    public virtual void Start(){
        #region Random Item Generate Position Setting
            random = new System.Random(0);
        #endregion
    }

    public virtual void Generate(){
        float itemPositionX;
        float itemPositionZ;
        float itemPositionY;
        do
        {
            // Debug.Log(transform);
            itemPositionX = transform.position.x + pivot.x + random.Next(-range.x, range.x + 1);
            itemPositionZ = transform.position.z + pivot.z + random.Next(-range.z, range.z + 1);
            itemPositionY = transform.position.y + pivot.y + random.Next(-range.y, range.y + 1);

        } while (itemPositionX == itemPositionZ && itemPositionX == 0);
        Vector3 itemPosition = new Vector3(itemPositionX,itemPositionY,itemPositionZ);
        Vector3 itemNormal = Vector3.up;
        GameObject itemObjectPrefab = null;
        if(item is WeaponItem){
            itemObjectPrefab = ((WeaponItem)this.item).modelPrefab;
        }else if(item is MaterialItem){
            itemObjectPrefab = ((MaterialItem)this.item).modelPrefab;
        }
        if (useRaycast &&Physics.Raycast(new Vector3(itemPosition.x,100,itemPosition.z), Vector3.down ,out RaycastHit hit, 200f, layerMask)) {
            itemPosition = new Vector3(itemPosition.x,hit.point.y,itemPosition.z);
            itemNormal = hit.normal;
        }

        itemPosition += itemObjectPrefab?new Vector3(0,itemObjectPrefab.GetComponentInChildren<Renderer>().bounds.size.y/2,0):Vector3.zero;

        if(NetworkManager.Singleton.IsServer){
            SpawnNetworkItemObject(itemPosition,itemNormal);
        }else{
            SpawnLocalItemObject(itemObjectPrefab, itemPosition,itemNormal);
        }
    }

    void SpawnNetworkItemObject(Vector3 itemPosition,Vector3 itemNormal){
        
        if(NetworkItemReferences.Singleton == null){
            return;
        }

        int itemId = NetworkItemReferences.Singleton.GetId(item);

        NetworkObject poolObject = NetworkObjectPool.Singleton.GetNetworkObject("item_drop",itemPosition,Quaternion.identity);
        if(poolObject){
            NetworkItem networkItem = poolObject.GetComponentInChildren<NetworkItem>();
            InteractableScript interactableScript = networkItem.GetComponent<InteractableScript>();
            generatedItemList.Add(networkItem.gameObject);
            poolObject.Spawn();
            if(isItemDestroyByTime){
                networkItem.SetDestroyTime(destroyTime, () => {generatedItemList.Remove(networkItem.gameObject);});
            }
            networkItem.SetItemId(itemId);
            networkItem.OnDespawnEvent = () => generatedItemList.Remove(networkItem.gameObject);
        }
    }

    void SpawnLocalItemObject(GameObject itemObjectPrefab, Vector3 itemPosition, Vector3 itemNormal){
        GameObject itemObject = Instantiate(itemObjectPrefab,itemPosition,Quaternion.identity);
        generatedItemList.Add(itemObject);
        itemObject.tag = "Interactable";
        Item newItem = Instantiate(item);
        newItem.name = item.name;
        if(setParent){
            itemObject.transform.SetParent(transform);
        }
        if(useRaycast){
            Quaternion.LookRotation(itemObject.transform.forward,itemNormal);
        }
        if(isItemDestroyByTime){
            Destroy(itemObject,destroyTime);
        }

        if(!itemObject.TryGetComponent(out Collider collider)){
            SphereCollider sphereCollider = itemObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 1;
            sphereCollider.isTrigger = true;
        }
        if(item is WeaponItem){
            itemObject.AddComponent<WeaponPickUp>().weapon = (WeaponItem)newItem;
            itemObject.GetComponent<WeaponPickUp>().interactableText = item.name;
            if(isItemDealDamage){
                itemObject.transform.GetComponentInChildren<DamageCollider>().EnableDamageCollider();
            }
        }else{
            itemObject.AddComponent<MaterialPickUp>().material = (MaterialItem)newItem;
            itemObject.GetComponent<MaterialPickUp>().interactableText = item.name;
            if(isItemDealDamage){
                itemObject.AddComponent<DamageCollider>().EnableDamageCollider();
            }
        }
        itemObject.TryGetComponent(out InteractableScript interactableScript);
        if(interactableScript){
            interactableScript.customCallback = new EventTrigger.TriggerEvent();
            interactableScript.customCallback.AddListener((BaseEventData arg0) => { generatedItemList.Remove(itemObject); });
        }

        AnimateItem(itemObject,itemPosition);
    }

    void AnimateItem(GameObject itemObject, Vector3 itemPosition){
        Vector3 movingDestination;

        switch(itemInitMovement){
            case ItemInitMovement.GoUp:
                movingDestination = new Vector3(itemPosition.x,itemPosition.y+extraMovingDistance,itemPosition.z);
                break;
            case ItemInitMovement.FallDown:
                itemObject.transform.position += new Vector3(0,extraMovingDistance,0);
                movingDestination = new Vector3(itemPosition.x,transform.position.y,itemPosition.z);
                if(useRaycast){
                    movingDestination.y = itemPosition.y;
                }
                break;
            case ItemInitMovement.MoveRight:
                movingDestination = new Vector3(itemPosition.x+extraMovingDistance,itemPosition.y,itemPosition.z);
                break;
            case ItemInitMovement.MoveLeft:
                movingDestination = new Vector3(itemPosition.x-extraMovingDistance,itemPosition.y,itemPosition.z);
                break;
            case ItemInitMovement.MoveFront:
                movingDestination = new Vector3(itemPosition.x,itemPosition.y,itemPosition.z+extraMovingDistance);
                break;
            case ItemInitMovement.MoveBack:
                movingDestination = new Vector3(itemPosition.x,itemPosition.y,itemPosition.z-extraMovingDistance);
                break;
            default:
                return;
        }
        itemObject.transform.DOMove(movingDestination,0.5f);
    }

}