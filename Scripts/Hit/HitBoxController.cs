using DG.Tweening;
using UnityEngine;

public class HitBoxController : MonoBehaviour {
    [Range(0,100)]
    public float bodyPartWeakness = 50;

    public void DoShake(Vector3 hitPos,float strengthMultiplier){
        Vector3 attackDir = transform.position - hitPos;
        attackDir.y = 0;
        attackDir = attackDir.normalized;
        strengthMultiplier *= 0.8f*Mathf.Abs(1-bodyPartWeakness/100.0f);
        transform.DOShakePosition(0.2f,attackDir*0.4f*strengthMultiplier);
    }
}