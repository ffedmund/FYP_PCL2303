using FYP;
using UnityEngine;

public class RandomItemGenerator : LimitedItemGenerator {
    [System.Serializable]
    public struct RandomItemData{
        public Item item;
        [Range(0.02f,100)]
        public float weighting;
        public float lotteryNumber;
    }

    public RandomItemData[] randomItemDatas;
    float totalWeighting = 0;

    public override void Start(){
        for(int i = 0; i < randomItemDatas.Length; i++){
            randomItemDatas[i].lotteryNumber = totalWeighting + randomItemDatas[i].weighting;
            totalWeighting += randomItemDatas[i].weighting;
        }
        int rndIndex = Random.Range(0,100);
        random = new System.Random(rndIndex);
    }

    public override void Generate(){
        float rndIndex = Random.Range(0,totalWeighting+1);
        foreach(RandomItemData randomItemData in randomItemDatas){
            if(rndIndex<=randomItemData.lotteryNumber){
                item = randomItemData.item;
                base.Generate();
                break;
            }
        }
    }
}