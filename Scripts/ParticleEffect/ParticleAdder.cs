using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class ParticleAdder : MonoBehaviour
{   
    public ParticleType type;

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && other.transform.TryGetComponent(out ParticleController particleController)){
            particleController.SetParticle(type);
        }
    }
}
