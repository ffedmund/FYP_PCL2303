using UnityEngine;

public class LimitedItemGenerator : ItemGenerator {
    [Header("Extra Setting")]
    public bool isInfinityGenerate;
    public int remainItemAmount;
    public int generateCoolDown;

    float previousGenerateTime;
    public int initItemAmount;

    void Awake(){
        initItemAmount = remainItemAmount;
    }   

    public override void Generate()
    {
        if(Time.time - previousGenerateTime >= generateCoolDown){
            if(!isInfinityGenerate){
                if(remainItemAmount > 0){
                    base.Generate();
                    remainItemAmount--;
                }
            }else{
                base.Generate();
            }
            UpdateInteractVisibility();
            previousGenerateTime = Time.time;
        }
    }

    public void UpdateInteractVisibility(){

        if(remainItemAmount < 1 && TryGetComponent(out Collider collider) && (gameObject.tag == "Interactable" || gameObject.tag == "Untagged")){
            collider.enabled=false;
            OutlineTrigger outline = transform.parent.GetComponentInChildren<OutlineTrigger>();
            if(outline){
                outline.LockOutline();
            }
        }
    }

    private void OnDisable() {
        remainItemAmount = initItemAmount;    
    }

}