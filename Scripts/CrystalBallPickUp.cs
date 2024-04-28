using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class CrystalBallPickUp : MaterialPickUp
{
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        if(playerManager)
        {
            playerManager.minimapIconController.SetID(1);
        }
    }
}
