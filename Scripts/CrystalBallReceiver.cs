using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP;


public class CrystalBallReceiver : ItemReceiver
{
    override public void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        if(containTargetItem)
        {
            GameManager.instance.EndGameEventHandler();
        }
    }
}
