using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraShakingController : MonoBehaviour
{
    public StressReceiver stressReceiver;
    public Slider healthBarSlider;

    float previousAmount = 0;
 
    void Start(){
        if(healthBarSlider == null){
            healthBarSlider = GameObject.Find("Health Bar").GetComponent<Slider>();
        }
        if(healthBarSlider != null){
            healthBarSlider.onValueChanged.AddListener((float currentAmount)=>SetStress(currentAmount));
        }
    }

    void SetStress(float currentAmount){
        float diff = currentAmount-previousAmount;
        if(diff<0){
            stressReceiver.InduceStress(Mathf.Min(-1.2f*diff/healthBarSlider.maxValue,1));
        }
        previousAmount = currentAmount;
    }
}
