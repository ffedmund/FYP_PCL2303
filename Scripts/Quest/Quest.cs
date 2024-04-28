using Unity.VisualScripting;

[System.Serializable]
public class Quest{
    public bool isActive;
    public bool isFinished;

    public int id;
    public QuestType questType;
    public HonorRank honorRank;
    public string title;
    public string description;
    public int expReward;
    public int  moneyReward;
    public int honorReward;
    public string itemReward;
    public int itemRewardAmount;
    public string targetNPC;
    public string completeDialog;

    public GoalChecker goalChecker;

    public Quest(int id,QuestType questType, HonorRank honorRank, string title, string description, int expReward, int moneyReward, int honorReward, string itemReward, int itemRewardAmount, string targetNPC, string completeDialog){
        this.id = id;
        this.questType = questType;
        this.honorRank = honorRank;
        this.title = title;
        this.description = description;
        this.expReward = expReward;
        this.moneyReward = moneyReward;
        this.honorReward = honorReward;
        this.itemReward = itemReward;
        this.itemRewardAmount = itemRewardAmount;
        this.targetNPC = targetNPC;
        this.completeDialog = completeDialog;
    }
    public string ToString(string color = "white") {
        return $"<color=\"{color}\">{title}\n</color> - "+ (goalChecker.isReached() && targetNPC != ""?$"Find {targetNPC}\n":$"{description} {goalChecker}\n");
    }
}

public enum QuestType{
    Regular = 0,
    Rank = 1,
    Special = 2
}