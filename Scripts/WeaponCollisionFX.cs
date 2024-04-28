using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollisionFX : MonoBehaviour
{
    public ParticleSystem collisionFX;
    public ParticleSystem bloodFX;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "GameController"){
            return;
        }
        if(other.tag=="Enemy" || other.tag=="Player" || other.GetComponent<NPCController>()){
            bloodFX.transform.position = GetComponent<Collider>().ClosestPoint(other.transform.position);
            bloodFX.transform.parent = null;
            bloodFX.Play();
        }else{
            AudioSourceController.instance.Play("SwordClash"+Random.Range(1,3),transform);
        }
        collisionFX.transform.position = GetComponent<Collider>().ClosestPoint(other.transform.position);
        collisionFX.transform.parent = null;
        collisionFX.Play();
    }
}
