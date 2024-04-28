using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(DayCycleManager))]
public class NetworkDayCycleSynchronize : NetworkBehaviour {
    DayCycleManager dayCycleManager;

    void Awake(){
        TryGetComponent(out dayCycleManager);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsServer){
            RequestCurrentTimeRPC(NetworkManager.Singleton.LocalClientId);
        }
    }

    [Rpc(SendTo.Server)]
    void RequestCurrentTimeRPC(ulong sourceNetworkObjectId)
    {
        float currentTime = dayCycleManager.currentTime;
        float time = dayCycleManager.time;
        UpdateCurrentTimeRPC(currentTime,time,RpcTarget.Single(sourceNetworkObjectId,RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void UpdateCurrentTimeRPC(float currentTime, float time, RpcParams rpcParams)
    {
        dayCycleManager.time = time;
        dayCycleManager.currentTime = currentTime;
    }
}