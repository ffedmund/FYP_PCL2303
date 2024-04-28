using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class WeaponFX : MonoBehaviour
    {
        [Header("Weapon FX")]
        public ParticleSystem normalWeaponTrail;

        public void PlayWeaponFX()
        {
            if (normalWeaponTrail == null)
            {
                return;
            }
            normalWeaponTrail.Stop();

            // if (normalWeaponTrail.isStopped)
            // {
            //     normalWeaponTrail.Play();
            // }

            normalWeaponTrail.Play();
        }

        public void StopWeaponFX()
        {
            if (normalWeaponTrail == null)
            {
                return;
            }

            normalWeaponTrail.Stop();
        }
    }
}

