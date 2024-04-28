using UnityEngine;
using Unity.Netcode;
using FYP;
using System;
using System.Collections;

public class NetworkItem : NetworkBehaviour {

    public Action OnDespawnEvent;

    [SerializeField] GameObject particle;
    private NetworkVariable<int> m_itemId = new NetworkVariable<int>(-1);
    private NetworkVariable<bool> m_particleState = new NetworkVariable<bool>(false);
    Item item;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Coroutine destroyCoroutine;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Network Item id: " + m_itemId.Value);
        if(IsClient){
            m_itemId.OnValueChanged += OnIdChanged;
            m_particleState.OnValueChanged += OnParticleStateChanged;
            if(m_itemId.Value >= 0){
                OnIdChanged(-1,m_itemId.Value);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient){
            Debug.Log("Network Item OnValueChanged Remove");
            m_itemId.OnValueChanged -= OnIdChanged;
            m_particleState.OnValueChanged -= OnParticleStateChanged;
            OnDespawnEvent?.Invoke();
            OnDespawnEvent = null;
        }
        if(IsServer){
            m_itemId.Value = -1;
            m_particleState.Value = false;
            item = null;
            if(destroyCoroutine != null){
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }
        if(TryGetComponent(out MaterialPickUp materialPickUp)){
            Destroy(materialPickUp);
        }
        if(TryGetComponent(out ArtifactPickUp artifactPickUp)){
            Destroy(artifactPickUp);
        }
        if(TryGetComponent(out WeaponPickUp weaponPickUp)){
            Destroy(weaponPickUp);
        }
        if(TryGetComponent(out EquipmentPickUp equipmentPickUp)){
            Destroy(equipmentPickUp);
        }
        transform.parent.SetParent(null,true);
    }

    private void OnIdChanged(int previousValue, int newValue)
    {
        if(newValue >= 0){
            item = NetworkItemReferences.Singleton.GetItem(m_itemId.Value);
            SetRenderer();
            SetPickUp();
        }
    }

    private void OnParticleStateChanged(bool previousValue, bool newValue)
    {
        particle.SetActive(newValue);
    }



    public void SetItemId(int id){
        if(IsServer && id >= 0){
            m_itemId.Value = id;
        }
    }

    public void SetParticle(bool state){
        if(IsServer){
            m_particleState.Value = state;
            particle.SetActive(m_particleState.Value);
        }
    }

    public void SetDestroyTime(float destroyTime, Action callback = null){
        destroyCoroutine = StartCoroutine(NetObjectDespawnTimer(destroyTime, callback));
    }

    void SetRenderer(){
        if(NetworkItemReferences.Singleton){
            if(meshFilter == null){
                meshFilter = transform.parent.GetComponentInChildren<MeshFilter>();
            }   
            if(meshRenderer == null){
                meshRenderer = transform.parent.GetComponentInChildren<MeshRenderer>();
            }
            GameObject itemObjectPrefab = null;
            if(item is WeaponItem){
                itemObjectPrefab = ((WeaponItem)item).modelPrefab;
            }else if(item is MaterialItem){
                itemObjectPrefab = ((MaterialItem)item).modelPrefab;
            }
            if(itemObjectPrefab == null){
                return;
            }
            if(itemObjectPrefab.TryGetComponent(out MeshFilter itemMeshFilter)){
                meshFilter.sharedMesh = itemMeshFilter.sharedMesh;
                meshFilter.transform.localScale = itemObjectPrefab.transform.localScale;
            }
            if(itemObjectPrefab.TryGetComponent(out MeshRenderer itemMeshRenderer)){
                meshRenderer.sharedMaterial = itemMeshRenderer.sharedMaterial;
            }
        }
        particle.SetActive(m_particleState.Value);
    }

    void SetPickUp(){
        if(item == null){
            return;
        }
        Item newItem = Instantiate(item);
        newItem.name = item.name;
        if(item is WeaponItem)
        {
            WeaponPickUp weaponPickUp = gameObject.AddComponent<WeaponPickUp>();
            weaponPickUp.weapon = (WeaponItem)newItem;
            weaponPickUp.interactableText = item.name;
        }
        if(item is MaterialItem)
        {
            MaterialPickUp materialPickUp = item.itemName == "crystal_ball"? gameObject.AddComponent<CrystalBallPickUp>() : gameObject.AddComponent<MaterialPickUp>();
            materialPickUp.material = (MaterialItem)newItem;
            materialPickUp.interactableText = item.name;
        }
        if(item is ArtifactItem)
        {
            ArtifactPickUp artifactPickUp = gameObject.AddComponent<ArtifactPickUp>();
            artifactPickUp.artifact = (ArtifactItem)newItem;
            artifactPickUp.interactableText = item.name;
        }
        if(item is EquipmentItem)
        {
            EquipmentPickUp equipmentPickUp = gameObject.AddComponent<EquipmentPickUp>();
            equipmentPickUp.equipment = (EquipmentItem)newItem;
            equipmentPickUp.interactableText = item.name;
        }
    }

    private IEnumerator NetObjectDespawnTimer(float destroyTime, Action callback = null)
    {
        yield return new WaitForSeconds(destroyTime);
        if(callback != null)
        {
            callback.Invoke();
        }
        NetworkObject.Despawn();
        yield break;
    }

}