using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        EnemyManager enemyManager;
        EnemyEffectManager enemyEffectManager;

        private new void Awake()
        {
            anim = GetComponent<Animator>();
            enemyManager = GetComponentInParent<EnemyManager>();
            enemyEffectManager = GetComponentInParent<EnemyEffectManager>();
        }

        public void PlayWeaponTrailFX()
        {
            enemyEffectManager.PlayWeaponFX(false);
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidBody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidBody.velocity = velocity;
        }

        public void SuccessfullyCastSpell(int index)
        {
            enemyManager.enemyCombatManager.SuccessfullyCastSpell(index);
        }

        public void EnableSuperArmor()
        {
            enemyManager.isSuperArmor = true;
        }

        public void DisableSuperArmor()
        {
            enemyManager.isSuperArmor = false;
        }
    }
}