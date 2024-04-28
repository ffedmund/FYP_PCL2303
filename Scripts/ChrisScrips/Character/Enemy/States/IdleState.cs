using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FYP
{
    public class IdleState : State
    {
        public PursueTargetState pursueTargetState;
        public LayerMask detectionLayer;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            #region Handle Enemy Target Detection
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

                if (characterStats != null)
                {
                    if (characterStats.isDead)
                        continue;

                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = characterStats;
                        return pursueTargetState;
                    }
                }
            }
            #endregion

            #region Handle Swithcing To Next State
            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }

            return this;
            #endregion
        }
    }
}

