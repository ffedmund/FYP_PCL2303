using FYP;
using UnityEngine;

public class ItemReceiver : InteractableScript {
    [SerializeField] bool oneTime;
    [SerializeField] Item targetItem;
    protected bool containTargetItem = false;

    public override void Interact(PlayerManager playerManager)
    {
        PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
        containTargetItem = false;
        if(targetItem is MaterialItem){
            if(playerInventory.materialsInventory.ContainsKey(targetItem.itemName)){
                playerInventory.RemoveItem(targetItem);
                containTargetItem = true;
            }
        }
        if(targetItem is WeaponItem){
            if(playerInventory.weaponsInventory.Contains((WeaponItem)targetItem)){
                playerInventory.weaponsInventory.Remove((WeaponItem)targetItem);
                containTargetItem = true;
            }
        }
        if(targetItem is EquipmentItem){
            if(playerInventory.helmetEquipmentsInventory.Contains((HelmetEquipment)targetItem)){
                playerInventory.helmetEquipmentsInventory.Remove((HelmetEquipment)targetItem);
                containTargetItem = true;
            }
            if(playerInventory.torsoEquipmentsInventory.Contains((TorsoEquipment)targetItem)){
                playerInventory.torsoEquipmentsInventory.Remove((TorsoEquipment)targetItem);
                containTargetItem = true;
            }
            if(playerInventory.armEquipmentsInventory.Contains((ArmEquipment)targetItem)){
                playerInventory.armEquipmentsInventory.Remove((ArmEquipment)targetItem);
                containTargetItem = true;
            }
            if(playerInventory.legEquipmentsInventory.Contains((LegEquipment)targetItem)){
                playerInventory.legEquipmentsInventory.Remove((LegEquipment)targetItem);
                containTargetItem = true;
            }
        }
        if(containTargetItem){
            base.Interact(playerManager);
            AnimatorHandler animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();
            animatorHandler.PlayTargetAnimation("Pick Up Item", true);
            if(oneTime){
                GetComponent<Collider>().enabled = false;
                if(TryGetComponent(out NavigationTarget navigationTarget)){
                    navigationTarget.enabled = false;
                }
            }
        }
    }
}