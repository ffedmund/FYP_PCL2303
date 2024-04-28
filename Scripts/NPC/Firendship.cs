using UnityEngine;

public class Firendship : MonoBehaviour {
    readonly int dailyIncreaseLimit = 5;

    public int firendshipValue = 0;
    public bool firendshipValueLock = false;

    int currentDailyIncreasing = 0;

    public void IncreaseFirendship(int value){
        if(firendshipValueLock){
            return;
        }
        firendshipValue += value;
        if(firendshipValue >= 99){
            firendshipValueLock = true;
        }
    }

    public void IncreaseFirendship(){
        if(firendshipValueLock){
            return;
        }
        if(currentDailyIncreasing < dailyIncreaseLimit){
            firendshipValue += 5;
            currentDailyIncreasing += 5;
        }
    }

    public void DecreaseFirendship(int value){
        if(firendshipValueLock){
            return;
        }
        if(firendshipValue - value < 0){
            firendshipValueLock = true;
            firendshipValue = 0;
        }else{
            firendshipValue = firendshipValue-value;
        }
    }

    public void ResetDailyIncrease(){
        currentDailyIncreasing = 0;
    }
}