using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CharacterEffectManager : MonoBehaviour
    {
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        public void PlayWeaponFX(bool isLeft)
        { 
            if (isLeft == false)
            {
                if (rightWeaponFX != null)
                {
                    rightWeaponFX.PlayWeaponFX();
                }
            }
            else
            {
                if (leftWeaponFX != null)
                {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }
    }
}