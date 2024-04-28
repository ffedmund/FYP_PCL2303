using UnityEngine;
using Unity.Netcode;
using FYP;
using System.Collections;

public class NetworkInteraction : NetworkBehaviour {
    InteractableScript interactableScript;
    Coroutine destroyCoroutine;

    public override void OnNetworkSpawn()
    {
        TryGetComponent(out interactableScript);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        OutlineTrigger outlineTrigger = NetworkObject.GetComponentInChildren<OutlineTrigger>();
        if(outlineTrigger){
            outlineTrigger.UnLockOutline();
        }
        if(destroyCoroutine != null){
            StopCoroutine(destroyCoroutine);
        }
    }

    [Rpc(SendTo.Server)]
    public virtual void InteractServerRPC(ulong sourceNetworkObjectId){
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Try Interact {gameObject.name}");
        InteractRPC(sourceNetworkObjectId, RpcTarget.Not(sourceNetworkObjectId,RpcTargetUse.Temp));
    } 

    [Rpc(SendTo.SpecifiedInParams)]
    public void InteractRPC(ulong sourceNetworkObjectId, RpcParams rpcParams){
        Debug.Log($"Client Received the RPC on NetworkObject #{sourceNetworkObjectId} Interact Object at {gameObject.name}");
        if(interactableScript == null && !TryGetComponent(out interactableScript)){
            return;
        }
        interactableScript.Interact(null);
    }

    [Rpc(SendTo.Server)]
    public void DespawnObjectRPC(ulong sourceNetworkObjectId){
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Try Destroy {gameObject.name}");
        NetworkObject.Despawn();
        if(NetworkObjectManager.Singleton){
            NetworkObjectManager.Singleton.DisableObjectDataRPC(NetworkManager.Singleton.LocalClientId,NetworkObject.transform.position);
        }
    }

    public void SetDestroyTime(float destroyTime){
        destroyCoroutine = StartCoroutine(NetObjectDespawnTimer(destroyTime));
    }

    private IEnumerator NetObjectDespawnTimer(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        NetworkObject.Despawn();
        yield break;
    }
}