using UnityEngine;
using FYP;
using DG.Tweening;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public NPC npc;
    public float maxRaycastDistance = 50.0f; // Set this to the maximum expected distance between the NPC and the ground
    public float turnSpeed = 5.0f; // Set this to adjust how quickly the NPC turns its head
    public float maxDistance = 5.0f; // Set this to adjust how close the player needs to be for the NPC to turn its head
    public float animationActiveDistance = 45.0f; // Set this to adjust how close the player needs to be for the NPC to turn its head
    public Animator animator;
    public NavMeshAgent agent;
    public NPCInteraction npcInteraction;
    public Transform chatingTargetTransform;
    public Transform playerTransform;
    public Firendship firendship;
    public NPCInventory npcInventory;
    public NPCStats npcStats;
    public Vector3 initialPosition;
    public bool isWalking;
    public bool doRotation;
    
    private Quaternion initialRotation;
    private NPCQuestManager npcQuestManager;
    private NPCItemGiver npcItemGiver;
    private NPCNoticeController npcNoticeController;
    private NPCDialogueController npcDialogueController;
    private LootDropHandler lootDropHandler;
    private Transform cameraTransform;
    private string speakingText = "";

    void OnDisable() {
        if(agent != null)
        {
            GetComponent<Collider>().enabled = true;
            agent.enabled = true;
            if(agent.isOnNavMesh)agent.ResetPath();
        }
    }   

    void OnEnable() {
        initialPosition = transform.position; 
    }

    void Start()
    {
        #region Initiate NPC Object
        GameObject npcObject = Instantiate(npc.npcPrefab,this.transform);
        animator = transform.GetComponentInChildren<Animator>();
        if(animator == null){
            animator = transform.AddComponent<Animator>();
        }
        animator.applyRootMotion = false;
        animator.runtimeAnimatorController = npc.npcAnimator;
        npcObject.layer = 12;
        animator.transform.AddComponent<FootstepController>();
        foreach(Transform child in npcObject.transform){
            child.gameObject.layer = 12;
        }
        // Save the initial rotation of the NPC
        initialRotation = transform.rotation; 
        #endregion

        // Cast a ray downwards from the position of this GameObject
        RaycastHit hit;
        int layerMask = 1 << 6;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxRaycastDistance,layerMask))
        {
            // Debug.Log("Ground height: " + hit.point.y);
            transform.position = hit.point;
            initialPosition = transform.position;
            // Debug.Log("Npc y position: " + hit.point.y);
        }
        else
        {
            Debug.LogWarning($"No ground found below NPC {npc.name}!");
        }

        if(!TryGetComponent(out firendship)){
            firendship = gameObject.AddComponent<Firendship>();
        }
        firendship.firendshipValue = npc.initFirendshipValue;

        gameObject.TryGetComponent(out npcQuestManager);
        npcQuestManager.Setup(npc);

        if(TryGetComponent(out npcInteraction)){
            npcInteraction.Setup(npc,animator,firendship);
        }
        if(TryGetComponent(out npcInventory)){
            npcInventory.Setup(npc);
        }
        if(TryGetComponent(out npcDialogueController)){
            npcDialogueController.Setup(npc);
        }

        if(TryGetComponent(out lootDropHandler) && npc.lootList != null)
        {
            lootDropHandler.SetLootList(npc.lootList);
        }
        TryGetComponent(out agent);
        TryGetComponent(out npcStats);
        cameraTransform = GameManager.instance.mainCamera?GameManager.instance.mainCamera.transform:Camera.main.transform;
    }

    public void Init(GameObject localPlayer)
    {
        playerTransform = localPlayer.transform;

        if(TryGetComponent(out npcNoticeController)){
            npcNoticeController.Setup(localPlayer.GetComponent<PlayerManager>());
        }

        gameObject.TryGetComponent(out npcItemGiver);
        npcItemGiver.Setup(npc,playerTransform);
    }

    public bool HeadFacingOn(Transform targetTransform){
        if(targetTransform == null){
            return false;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
        Vector3 directionToTarget = targetTransform.position - transform.position;
        // If the distance is less than maxDistance, turn the head of the NPC towards the player using an Animator
        if (distanceToTarget < maxDistance && doRotation)
        {
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if(angleToTarget < 120.0f && angleToTarget > -120.0f && !isWalking)
            {
                chatingTargetTransform = targetTransform;
                if (angleToTarget < 90.0f && angleToTarget > -90.0f) 
                {
                    float angleBetweenForwardAndDirectionToPlayer = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
                    float direction = (angleBetweenForwardAndDirectionToPlayer + 90.0f) / 180.0f;
                    DOTween.Kill("directionTween"+"-"+npc.name);
                    DOTween.To(()=>animator.GetFloat("direction"),x=>animator.SetFloat("direction",x),direction,0.5f).SetId("directionTween"+"-"+npc.name);
                }
                Quaternion targetRotation = new Quaternion();
                targetRotation = Quaternion.LookRotation(directionToTarget);
                targetRotation.eulerAngles = new Vector3(0,targetRotation.eulerAngles.y,0);
                // Debug.Log("targetRotation: "+targetRotation);
                DOTween.Kill("rotationTween"+"-"+npc.name);
                transform.DORotateQuaternion(targetRotation, turnSpeed).SetId("rotationTween"+"-"+npc.name);
            }
            else
            {
                return false;
            }
            chatingTargetTransform = chatingTargetTransform == playerTransform? cameraTransform:chatingTargetTransform;
            return true;
        }
        return false;
    }

    public void TurningTo(Vector3 position){
        ResetHeadFocus();
        Vector3 directionToTarget = position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        targetRotation.eulerAngles = new Vector3(0,targetRotation.eulerAngles.y,0);
        DOTween.Kill("rotationTween"+"-"+npc.name);
        transform.DORotateQuaternion(targetRotation, turnSpeed).SetId("rotationTween"+"-"+npc.name);
    }

    public void ResetHeadFocus(){
        // If the distance is greater than or equal to maxDistance, reset both objects' rotation using DOTween
        if(animator.GetFloat("direction") != 0.5)
        {
            DOTween.Kill("directionTween"+"-"+npc.name);
            DOTween.To(()=>animator.GetFloat("direction"),x=>animator.SetFloat("direction",x),0.5f,0.5f).SetId("directionTween"+"-"+npc.name);
        }
        if(transform.rotation != initialRotation){
            DOTween.Kill("rotationTween"+"-"+npc.name);
            transform.DORotateQuaternion(initialRotation, turnSpeed).SetId("rotationTween"+"-"+npc.name);
        }
    }

    public bool HeadFacingOnPlayer(){
        return HeadFacingOn(playerTransform);
    }

    public void SetSpeakingContent(string content){
        if(npc.isTalkable)
        {
            // Debug.Log($"NPC {gameObject.name}: {content}");
            speakingText = content;
        }
    }

    public void Dead(){
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;
        animator.SetFloat("speed",0);
        animator.SetFloat("direction",0.5f);
        npcNoticeController.ClearNotice();
        if(npc.lootList != null)
        {
            lootDropHandler.DropLoot();
        }
    }

    public void TraverseNavMeshLink(){
        StartCoroutine(NormalSpeedTraverse());
    }

    IEnumerator NormalSpeedTraverse()
    {
        animator.SetFloat("speed",agent.speed);
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;
        float duration = (endPos - startPos).magnitude / agent.speed;

        float t = 0.0f;
        float tStep = 1.0f / duration;
        while (t < 1.0f)
        {
            t += Time.deltaTime * tStep;
            agent.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        agent.CompleteOffMeshLink();
    }

    void Update()
    {
        //Some passive action of NPC
        if(npcStats.isDead)
        {
            return;
        }

        if(playerTransform){
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            // try{
                //SetNotice return false when there is no notice need to show
                if(!npcNoticeController.SetNotice(npcQuestManager,npcItemGiver,cameraTransform) && chatingTargetTransform != null){
                    if(!npcNoticeController.SetChatCloud(chatingTargetTransform,speakingText)){
                        chatingTargetTransform = null;
                    }
                }
                if (directionToPlayer.sqrMagnitude > maxDistance * maxDistance && npcInteraction.isInteracting){
                    npcInteraction.Init();
                }
                animator.enabled = directionToPlayer.sqrMagnitude < animationActiveDistance * animationActiveDistance;
            // }catch{
            //     Debug.Log($"[Notice Contoller Error] {npcQuestManager}, {npcItemGiver}, {directionToPlayer}");
            // }
        }
    }
}
