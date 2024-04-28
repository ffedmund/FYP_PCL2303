using UnityEngine;
using FYP;

public class QuestBoardInteraction : InteractableScript {
    [SerializeField] QuestBoardUI questBoardUI;
    public void Awake()
    {
        canvasUIController = FindAnyObjectByType<UIController>();
        questBoardUI = FindAnyObjectByType<QuestBoardUI>(FindObjectsInactive.Include);
        targetUIWindow = questBoardUI.transform.parent.gameObject;
        isUITrigger = true;
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        if(playerManager){
            questBoardUI.InteractingQuestBoard(transform,playerManager);
            canvasUIController.activePopupWindow = targetUIWindow;
        }
    }
}