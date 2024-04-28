using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem 
    {
        [Header("Projectile Damage")]
        public float baseDamage;

        [Header("Projectile Physics")]
        public float projectileForwardVelocity;
        public float projectileUpwardVelocity;
        public float projectileMass;
        public bool isAffectedByGravity;
        Rigidbody rigidbody;

        public override void AttemptToCastSpell(AnimatorHandler animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager)
        {
            base.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
            GameObject instantiatedWarmUpFX = Instantiate(spellWarmUpFX, weaponSlotManager.rightHandSlot.transform);
            // instantiatedWarmUpFX.gameObject.transform.localScale = new Vector3(100, 100, 100);
            animatorHandler.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SuccessfullyCastSpell(AnimatorHandler animatorHandler, PlayerStats playerStats, CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager)
        {
            base.SuccessfullyCastSpell(animatorHandler, playerStats, cameraHandler, weaponSlotManager);
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, weaponSlotManager.rightHandSlot.transform.position, cameraHandler.transform.rotation);
            instantiatedSpellFX.GetComponent<SpellDamageCollider>().spellCaster = playerStats;
            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            // spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();

            if (cameraHandler.currentLockOnTarget != null)
            {
                instantiatedSpellFX.transform.LookAt(cameraHandler.currentLockOnTarget.transform);
            }
            else
            {
                instantiatedSpellFX.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, playerStats.transform.eulerAngles.y, 0f);
            
            }

            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
            rigidbody.useGravity = isAffectedByGravity;
            rigidbody.mass = projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }

        public override void SuccessfullyCastSpell(AnimatorHandler animatorHandler, EnemyManager enemyManager, EnemyWeaponSlotManager weaponSlotManager)
        {
            base.SuccessfullyCastSpell(animatorHandler, enemyManager, weaponSlotManager);
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, weaponSlotManager.rightHandSlot.transform.position + enemyManager.transform.forward.normalized * 0.5f, enemyManager.transform.rotation);
            instantiatedSpellFX.GetComponent<SpellDamageCollider>().spellCaster = enemyManager.enemyStats;
            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            // spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();

            if (enemyManager.currentTarget != null)
            {
                instantiatedSpellFX.transform.LookAt(enemyManager.currentTarget.transform);
            }
            else
            {
                instantiatedSpellFX.transform.rotation = Quaternion.Euler(enemyManager.transform.eulerAngles.x, enemyManager.transform.eulerAngles.y, 0f);
            }

            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
            rigidbody.useGravity = isAffectedByGravity;
            rigidbody.mass = projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }
    }
}
