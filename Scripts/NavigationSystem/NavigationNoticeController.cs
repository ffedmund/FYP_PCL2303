using System.Collections;
using System.Threading;
using FYP;
using UnityEngine;

public class NavigationNoticeController : MonoBehaviour 
{
    enum TrackingTarget{
        QuestTarget,
        QuestReceiver
    }

    [SerializeField] GameObject[] effectPrefabs;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float stopDistance;
    PlayerManager playerManager;
    [SerializeField] TrackingTarget trackingTarget;
    float timer = 0;
    int navigationQuestId;
    [SerializeField] bool navigationToTarget;


    void Start(){
        TryGetComponent(out playerManager);
    }

    private void Update() 
    {
        if(navigationToTarget)
        {
            Vector2 target2DPos = new Vector2(targetPosition.x,targetPosition.z);
            Vector2 m_2DPos = new Vector2(transform.position.x,transform.position.z);
            bool navigationEnd = Vector2.SqrMagnitude(target2DPos - m_2DPos) <= stopDistance*stopDistance 
                                || playerManager.playerData.quests.Count == 0
                                || playerManager.playerData.quests[0].id != navigationQuestId 
                                || (trackingTarget == TrackingTarget.QuestTarget && playerManager.playerData.quests[0].goalChecker.isReached());
            if(navigationEnd)
            {
                navigationToTarget = false;
                return;
            }

            if(timer <= 0)
            {
                StartCoroutine(SpawnNavigationNotice());
                timer = Random.Range(5,8);
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        else if(playerManager.playerData.quests.Count > 0 )
        {
            Quest trackedQuest = playerManager.playerData.quests[0];
            string targetId;
            targetId = !trackedQuest.goalChecker.isReached()?trackedQuest.goalChecker.targetId:trackedQuest.targetNPC == ""?"QuestBoard":trackedQuest.targetNPC;
            trackingTarget = trackedQuest.goalChecker.isReached()?TrackingTarget.QuestReceiver:TrackingTarget.QuestTarget;
            navigationQuestId = trackedQuest.id;

            Vector3? closestPoint = NavigationSystem.instance?.FindClosestTarget(targetId,transform.position);
            if(closestPoint.HasValue && closestPoint.Value.y > -9999){
                SetTarget(closestPoint.Value);
            }
        }
    }

    IEnumerator SpawnNavigationNotice(){
        int i = 0;
        while(i++ < 3)
        {
            int rndIndex = Random.Range(0,effectPrefabs.Length);
            Vector3 effectPos = transform.position + new Vector3(Random.Range(-4,4),Random.Range(0,3)+1,Random.Range(-4,4));
            Transform effect = Instantiate(effectPrefabs[rndIndex], transform.position, Quaternion.identity).transform;
            Vector3 targetPostition = new Vector3(targetPosition.x, effect.position.y, targetPosition.z ) ;
            effect.LookAt(targetPostition);
            effect.position = effectPos;
            effect.Rotate(new Vector3(0,-90,0));
            yield return new WaitForSeconds(Random.Range(0,1.0f)*2);
        }
        yield return null;
    }

    public void SetTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        navigationToTarget = true;
    }
}