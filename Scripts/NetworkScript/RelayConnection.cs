using IngameDebugConsole;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayConnection : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay() {
        try {
           Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);            

           string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
           
           Debug.Log("Join Code: " + joinCode);

           RelayServerData relayServerData= new RelayServerData(allocation, "dtls");

           NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

           NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException ex) {
            Debug.Log(ex);
        }
    }

    private async void JoinRelay(string joinCode) {
        try {
            Debug.Log($"Join relay {joinCode}");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);


           RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

           NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

           NetworkManager.Singleton.StartClient();
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

}
