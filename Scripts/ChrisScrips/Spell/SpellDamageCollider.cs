using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class SpellDamageCollider : DamageCollider
    {
        public CharacterStats spellCaster;

        public GameObject impactParticles;
        public GameObject projectileParticles;
        public GameObject muzzleParticles;

        bool hasCollided = false;

        CharacterStats spellTarget;
        Rigidbody rigidbody;

        Vector3 impactNormal;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if (muzzleParticles != null)
            {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 2f);
            }
        }

        private void OnCollisionEnter(Collision other)
            {
                if (!hasCollided)
                {
                    spellTarget = other.transform.root.GetComponent<CharacterStats>();
                    if (spellTarget != null)
                    {   
                        if (spellTarget == spellCaster)
                        {
                            return;
                        }

                        if(spellTarget is PlayerStats)
                        {
                            ((PlayerStats)spellTarget).TakeDamage(currentWeaponDamage, null);
                        }
                        else
                        {
                            spellTarget.TakeDamage(currentWeaponDamage, null);
                        }
                    }
                    else
                    {
                        Debug.Log($"Collision Target isn't Character {other.gameObject.name}");
                    }

                    hasCollided = true;
                    impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));

                    Destroy(projectileParticles);
                    Destroy(impactParticles, 3f);
                    Destroy(gameObject, 3f);
                }
            }
    }
}
