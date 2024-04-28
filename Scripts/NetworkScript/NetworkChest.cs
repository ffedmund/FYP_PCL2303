using UnityEngine;
using Unity.Netcode;
using FYP;

[RequireComponent(typeof(ChestInteraction))]
public class NetworkChest : NetworkInteraction {
    NetworkVariable<bool> m_chestState = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        TryGetComponent(out Animator chestAnimator);
        if(m_chestState.Value && chestAnimator){
            chestAnimator.Play(ChestInteraction.aniamtionClipName);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(m_chestState.Value && IsServer){
            m_chestState.Value = false;
        }
    }

    [Rpc(SendTo.Server)]
    public override void InteractServerRPC(ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId} Try Interact {gameObject.name}");
        InteractRPC(sourceNetworkObjectId, RpcTarget.Not(sourceNetworkObjectId,RpcTargetUse.Temp));
        SetDestroyTime(120);
        m_chestState.Value = true;
    }


}