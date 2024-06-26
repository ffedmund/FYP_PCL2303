using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.AI;

namespace FYP
{
    public class CombatStanceState : State
    {
        public IdleState idleState;
        public AttackState attackState;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            HandleRotateTowardsTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }

            if (enemyManager.currentTarget.isDead)
            {
                return idleState;
            }

            Transform rootParent = enemyManager.transform.root;
            if (rootParent.name == "EnemyWorm" || rootParent.name == "EnemyWorm(Clone)")
            {
                enemyManager.maximumAttackRange = 4.5f;
            }

            if (rootParent.name == "EnemyDragon" || rootParent.name == "EnemyDragon(Clone)")
            {
                enemyManager.maximumAttackRange = 6f;
            }

            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return attackState;
            }
            else if (distanceFromTarget > enemyManager.maximumAttackRange && enemyManager.currentTarget.isDead == false)
            {
                return pursueTargetState;
            }

            return this;
        }

        public void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
                enemyManager.transform.DORotateQuaternion(targetRotation, 1 / enemyManager.rotationSpeed);
            }
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

                /* Original Code*/
                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);

                enemyManager.enemyRigidBody.velocity = targetVelocity;
                // enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
                enemyManager.transform.DORotateQuaternion(enemyManager.navMeshAgent.transform.rotation, 1 / enemyManager.rotationSpeed);
            }
        }
    }
}

