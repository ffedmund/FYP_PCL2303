using System.Collections.Generic;
using FYP;
using UnityEngine;
using Unity.Netcode;

public class AutoItemGenerator : ItemGenerator {
    [Header("Auto Generator")]
    public float generatePeriod;
    public float maximumItemIndex = 1;
    float previousGenerateTime;

    public override void Start() {
        base.Start();
        previousGenerateTime = 0;
        generatePeriod = generatePeriod < Time.fixedDeltaTime?Time.fixedDeltaTime:generatePeriod;
        Invoke("Generate",generatePeriod+Time.fixedDeltaTime);
    }

    public override void Generate(){
        //Avoid Network Client Auto Generate Item
        if(NetworkManager.Singleton && !NetworkManager.Singleton.IsServer){
            return;
        }
        if(Time.time - previousGenerateTime >= generatePeriod && generatedItemList.Count < maximumItemIndex){
            base.Generate();
        }
        previousGenerateTime = Time.time;
        Invoke("Generate",generatePeriod+Time.fixedDeltaTime);
    }
}