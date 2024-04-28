using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

namespace FYP
{
    public class EnemyManager : CharacterManager
    {
        const float TeleportThreshold = 36;

        EnemyLocomotionManager enemyLocomotionManager;
        public EnemyStats enemyStats;

        public State currentState;
        public CharacterStats currentTarget;
        public NavMeshAgent navMeshAgent;
        public Rigidbody enemyRigidBody;
        public LootDropHandler lootDropHandler;
        public EnemyCombatManager enemyCombatManager;
        public EnemyAnimatorManager enemyAnimatorManager;

        PlayerManager playerManager;

        public bool isPerformingAction;
        public bool isSuperArmor;
        // public bool isInteracting;
        public float rotationSpeed = 15f;
        public float maximumAttackRange = 1.5f;

        [Header("A.I. Settings")]
        public float detectionRadius = 20f;
        public float maximumDetectionAngle = 50f;
        public float minimumDetectionAngle = -50f;

        public float currentRecoveryTime = 0f;

        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        // public NetworkVariable<Vector3> networkRotation = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> networkYAxisRotation = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyCombatManager = GetComponent<EnemyCombatManager>();
            enemyStats = GetComponent<EnemyStats>();
            enemyRigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            lootDropHandler = GetComponent<LootDropHandler>();
            navMeshAgent.enabled = false;
        }

        private void Start()
        {
            enemyRigidBody.isKinematic = false;

            playerManager = GameManager.instance.localPlayerManager;

            if (NetworkObject.IsSpawned)
            {
                Debug.Log("Spawned");
                if (playerManager.IsHost)
                {
                    networkPosition.Value = transform.position;
                    networkYAxisRotation.Value = transform.eulerAngles.y;
                }
                else
                {
                    transform.position = networkPosition.Value;
                    transform.eulerAngles = new Vector3(0,networkYAxisRotation.Value,0);
                }
            }
            else
            {
                Debug.Log("Not Spawned");
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // if (IsServer)
            // {
            //     navMeshAgent.enabled = true;


            // }
            if(!IsServer)
            {
                transform.position = networkPosition.Value;
            }
        }

        private void Update()
        {
            HandleRecoveryTimer();

            isInteracting = enemyAnimatorManager.anim.GetBool("isInteracting");

            if (NetworkObject.IsSpawned)
            {
                // Debug.Log("Spawned");
                if (playerManager.IsHost)
                {
                    // Debug.Log("Host");
                    networkPosition.Value = transform.position;
                    networkYAxisRotation.Value = transform.eulerAngles.y;
                }
                else
                {
                    // Debug.Log("Client");
                    bool needTeleport = Vector3.SqrMagnitude(transform.position - networkPosition.Value) > TeleportThreshold;
                    transform.position = needTeleport? networkPosition.Value : Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime);
                    transform.eulerAngles = new Vector3(0,networkYAxisRotation.Value,0);
                }
            }
            else
            {
                Debug.Log("Not Spawned");
            }
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
        }

        private void HandleStateMachine()
        {
            // if (enemyLocomotionManager.currentTarget != null)
            // {
            //     enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
            // }

            // if (enemyLocomotionManager.currentTarget == null)
            // {
            //     enemyLocomotionManager.HandleDetection();
            // }
            // else if (enemyLocomotionManager.distanceFromTarget > enemyLocomotionManager.stoppingDistance)
            // {
            //     enemyLocomotionManager.HandleMoveToTarget();
            // }
            // else if (enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
            // {
            //     AttackTarget(); 
            // }

            if (enemyStats.isDead)
            {
                //Trigger quest goal checker when enemy killed
                if (lootDropHandler)
                {
                    lootDropHandler.DropLoot();
                }
                if (currentTarget is PlayerStats)
                {
                    PlayerData playerData = currentTarget.GetComponent<PlayerManager>().playerData;
                    foreach (Quest quest in playerData.quests)
                    {
                        quest.goalChecker.EnemyKilled(name.Substring(5).Split("(")[0]);
                    }
                    currentTarget = null;
                }
                SwitchToNextState(transform.Find("EnemyStates").GetComponentInChildren<IdleState>());

                isSuperArmor = false;
                return;
            }
            else if (currentState != null)
            {
                if(NetworkManager.Singleton && !NetworkManager.Singleton.IsServer)
                {
                    //Only allow server to run state logic
                    return;
                }

                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

        public void SwitchToNextState(State state)
        {
            currentState = state;
        }
    }
}

