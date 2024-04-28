using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class FootstepComponent : MonoBehaviour
{   
    public enum FootSide{
        Left,
        Right
    }
    public ParticleSystem footstepParticle;
    public FootSide footSide;
    public LayerMask mask;

    CharacterManager characterManager;
    InputHandler _inputHandler;

    void Start(){
        characterManager = transform.root.GetComponent<CharacterManager>();
        _inputHandler = characterManager.GetComponentInChildren<InputHandler>();
    }

    private void OnEnable() {
        footstepEvent();
    }

    public void footstepEvent(){
        if(characterManager && !characterManager.isInteracting && _inputHandler.moveAmount > 0){
            footstepParticle.Play();
            if(AudioSourceController.instance){
                if (Physics.Raycast(transform.position+new Vector3(0,0.5f,0), new Vector3(0,-1,0), out RaycastHit hit, 2f, mask)) 
                {
                    // Debug.Log(hit.transform.name);
                    AudioSourceController.instance.Play((hit.transform.tag == "Stone"?"Stone":"Grass")+"Step"+(footSide == FootSide.Left?"L":"R"),transform);
                }
            }
        }
    }
}
