using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LimitedItemGenerator))]
public class ItemAmountSyncManager : NetworkBehaviour {
    public NetworkVariable<int> remainItemAmount = new NetworkVariable<int>();
    private LimitedItemGenerator limitedItemGenerator;

    void Start() 
    {
        limitedItemGenerator = GetComponent<LimitedItemGenerator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(limitedItemGenerator == null)
        {
            limitedItemGenerator = GetComponent<LimitedItemGenerator>();
        }
        if(IsServer)
        {
            remainItemAmount.Value = limitedItemGenerator.remainItemAmount;
        }else if(remainItemAmount.Value != limitedItemGenerator.initItemAmount)
        {
            limitedItemGenerator.remainItemAmount = remainItemAmount.Value;
        }
        limitedItemGenerator.UpdateInteractVisibility();
    }
}