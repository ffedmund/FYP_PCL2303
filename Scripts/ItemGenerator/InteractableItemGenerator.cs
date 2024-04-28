
using DG.Tweening;
using FYP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableItemGenerator : RandomItemGenerator {
    public bool isShaking;
    public float shakingStrength = 1;
    [Header("VFX")]
    public bool enableVFX;
    public ParticleSystem interactEffect;
    public ParticleSystem destroyEffect;

    [Header("Network Setting")]
    [SerializeField] bool isNetworkObject;

    ItemAmountSyncManager itemAmountSyncManager;

    public override void Start()
    {
        base.Start();
        isNetworkObject = transform.root.GetComponent<NetworkObject>() != null;
        if(isNetworkObject)
        {
            itemAmountSyncManager = GetComponent<ItemAmountSyncManager>();
        }
    }

    public void Interact(){
        if(isShaking){
            shakeTransform.DOShakePosition(1, new Vector3(0.08f, 0f, 0.08f)*shakingStrength);
        }
        if(enableVFX && interactEffect){
            interactEffect.Play();
        }
        if(isNetworkObject)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Generate();
                itemAmountSyncManager.remainItemAmount.Value--;
            }
            else
            {
                remainItemAmount--;
                UpdateInteractVisibility();
            }
        }
        else
        {
            Generate();
        }
}
    
    // public void Interact(BaseEventData eventData){
    //     if(isShaking){
    //         shakeTransform.DOShakePosition(1, new Vector3(0.08f, 0f, 0.08f)*shakingStrength);
    //     }
    //     if(enableVFX && interactEffect){
    //         interactEffect.Play();
    //     }
    //     if(eventData.selectedObject != null){
    //         if(isNetworkObject)
    //         {
    //             if(itemAmountSyncManager.remainItemAmount.Value > 0 || isInfinityGenerate)
    //             {
    //                 Generate();
    //                 itemAmountSyncManager.ReduceRemainItemAmountRPC(NetworkManager.Singleton.LocalClientId,1);
    //             }
    //         }
    //         else
    //         {
    //             Generate();
    //         }
    //     }
    // }

    private void OnDisable() {
        if(enableVFX && destroyEffect){
            destroyEffect.Play();
        }
    }

}