using UnityEngine;

public class FootstepController : MonoBehaviour {
    [SerializeField] FootstepComponent left;
    [SerializeField] FootstepComponent right;   
    
    public void LeftFoot(){
        if(left){
            left.footstepEvent();
        }
    }


    public void RightFoot(){
        if(right){
            right.footstepEvent();
        }
    }
}