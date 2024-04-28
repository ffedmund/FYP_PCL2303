using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace FYP
{
    public class PursueTargetState : State
    {
        public IdleState idleState;
        public CombatStanceState combatStanceState;

        const float SqrAgentSyncThreshold = 4f;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            if (distanceFromTarget > enemyManager.detectionRadius)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.0f, Time.deltaTime);
                if(enemyManager.currentTarget is PlayerStats)
                {
                    //Remove BGM Trigger
                    BGMEvent.RemoveTrigger("Battle",enemyManager.transform);
                }
                enemyManager.currentTarget = null;
                return idleState;
            }

            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);

                float speed = 3f;
                Transform rootParent = enemyManager.transform.root;
                if (rootParent.name != "Enemy" && rootParent.name != "Enemy(Clone)" && rootParent.name != "EnemyWorm" && rootParent.name != "EnemyWorm(Clone)" && rootParent.name != "EnemyViking" && rootParent.name != "EnemyViking(Clone)"
                    && rootParent.name != "NetworkEnemy" && rootParent.name != "NetworkEnemy(Clone)" && rootParent.name != "NetworkEnemyWorm" && rootParent.name != "NetworkEnemyWorm(Clone)" && rootParent.name != "NetworkEnemyViking" && rootParent.name != "NetworkEnemyViking(Clone)")
                // {
                //     speed = 5f;
                // }
                {
                    enemyManager.transform.position += enemyManager.transform.forward * speed * Time.deltaTime;
                }

                if(enemyManager.currentTarget is PlayerStats)
                {
                    //Trigger BGM
                    BGMEvent.TriggerEvent("Battle",enemyManager.transform);
                }
            }

            HandleRotateTowardsTarget(enemyManager);

            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

            if (distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return combatStanceState;
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
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

                /* Original Code*/
                // enemyManager.navMeshAgent.enabled = true;
                // //Check Nav Mesh before set destination
                // if(enemyManager.navMeshAgent.isOnNavMesh){
                //     enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                // }

                if(Vector3.SqrMagnitude(enemyManager.navMeshAgent.transform.position - enemyManager.transform.position) > SqrAgentSyncThreshold)
                {
                    enemyManager.navMeshAgent.enabled = false;
                    enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
                }

                enemyManager.navMeshAgent.enabled = true;

                if(NavMesh.SamplePosition(enemyManager.transform.position, out NavMeshHit hit, enemyManager.detectionRadius/2, NavMesh.AllAreas) && enemyManager.navMeshAgent.isOnNavMesh)
                {
                    enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                    if(enemyManager.navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                    {
                        Debug.Log($"[Reset Path] enemy path status: {enemyManager.navMeshAgent.pathStatus}");
                        enemyManager.navMeshAgent.ResetPath();
                        enemyManager.enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                    }
                    // Debug.Log($"Enemy Path Status: {enemyManager.navMeshAgent.pathStatus}");
                }

                enemyManager.enemyRigidBody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }
    }
}

