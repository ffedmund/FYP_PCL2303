using System.Collections.Generic;
using UnityEngine;
namespace FYP
{
    public enum ParticleType
    {
        None = -1,
        Snow = 0,
        Leaves = 1
    }

    public class ParticleController : MonoBehaviour 
    {
        [SerializeField] GameObject[] particlePrefabs;
        [SerializeField] ParticleType currentParticleEffect = ParticleType.None;
        GameObject[] particles;
        
        void Start()
        {
            particles = new GameObject[particlePrefabs.Length];
            for(int i = 0; i < particles.Length; i++){
                GameObject @object = Instantiate(particlePrefabs[i],transform);
                @object.transform.localPosition = new Vector3(0,15,0);
                particles[i] = @object;
                @object.SetActive(false);
            }
        }

        public void SetParticle(ParticleType particleType)
        {
            if(currentParticleEffect != particleType)
            {
                if(currentParticleEffect != ParticleType.None)
                {
                    particles[(int)currentParticleEffect].SetActive(false);
                }
                if(particleType != ParticleType.None)
                {
                    particles[(int)particleType].SetActive(true);
                }
                currentParticleEffect = particleType;
            }
        }
    }
}