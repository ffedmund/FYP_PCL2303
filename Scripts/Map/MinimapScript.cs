using FYP;
using UnityEngine;

public class MinimapScript : PlayerSetupBehaviour {
    [SerializeField] Transform followPlayer;
    public bool stopFollowPlayer = false;

    void Update(){
        if(followPlayer && !stopFollowPlayer){
            transform.position = new Vector3(followPlayer.position.x,10*TerrainGenerationManager.scale,followPlayer.position.z);
        }
    }

    public override void Setup(PlayerManager playerManager)
    {
        followPlayer = playerManager.transform;
    }
}