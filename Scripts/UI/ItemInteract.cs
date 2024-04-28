using UnityEngine;

public class ItemInteract : MonoBehaviour {
    public bool isInteracting;
    public bool isLock;

    public void ItemSlotOnClick(){
        if(isLock)return;
        isInteracting = !isInteracting;
        if(isInteracting){
            ItemInteractManager.OnItemInteract(this,transform.position);
        }else{
            ItemInteractManager.OnItemNotInteract();
        }
    }
}