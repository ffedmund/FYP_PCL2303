using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject PlayerPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpawnPlayersServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    public void SpawnPlayersServerRpc(ulong clientId)
    {   
        GameObject playerObject = Instantiate(PlayerPrefab);
        NetworkObject playerNetObject = playerObject.GetComponent<NetworkObject>();
        StartCoroutine(GameManager.instance.SetRandomPlayerPosition(playerObject.transform));
        playerNetObject.SpawnAsPlayerObject(clientId,true);
    }
}
