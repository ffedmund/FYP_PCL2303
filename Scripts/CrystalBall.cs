using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall : MonoBehaviour
{
    public static CrystalBall instance;
    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        if(instance == this && KeywordFunctionManager.instance != null){
            KeywordFunctionManager.instance.keywordFunctionDict.Add("cb_dir_tree",(object value)=>{
                try{
                    GameObject worldTree = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "World Tree").building;
                    Vector3 startingPosition = worldTree.transform.position;
                    return GetDirection(startingPosition);
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
            KeywordFunctionManager.instance.keywordFunctionDict.Add("cb_dir_town",(object value)=>{
                try{
                    GameObject town = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "Town Center").building;
                    Vector3 startingPosition = town.transform.position;
                    return GetDirection(startingPosition);
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
            KeywordFunctionManager.instance.keywordFunctionDict.Add("cb_dist_euc_tree",(object value)=>{
                try{
                    GameObject worldTree = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "World Tree").building;
                    Vector3 startingPosition = worldTree.transform.position;
                    return (Mathf.RoundToInt(GetEuclideanDistance(startingPosition)/10)*10).ToString();
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
            KeywordFunctionManager.instance.keywordFunctionDict.Add("cb_distndir_man_gate",(object value)=>{
                try{
                    GameObject town = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "Town Center").building;
                    GameObject eastGate = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "East Gate").building;
                    GameObject northGate = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "North Gate").building;
                    string positionDescription = "";
                    string[] dir = GetSeparateDirection(northGate.transform.position);
                    string gateName = (town.transform.position.z > transform.position.z)?"east gate":"north gate";
                    Vector2 manDis = (town.transform.position.z > transform.position.z)?GetManhattanDistance(eastGate.transform.position):GetManhattanDistance(northGate.transform.position);
                    if(gateName == "east gate"){
                        positionDescription = $"{manDis.y} meters {dir[1]} from the {gateName}, then {manDis.x} meters {dir[0]}";
                    }else{
                        positionDescription = $"{manDis.x} meters {dir[0]} from the {gateName}, then {manDis.y} meters {dir[1]}";
                    }
                    return positionDescription;
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
            KeywordFunctionManager.instance.keywordFunctionDict.Add("cb_dist_euc_town",(object value)=>{
                try{
                    GameObject town = BuildingLocationManager.instance.locationList.Find(loc => loc.buildingName == "Town Center").building;
                    Vector3 startingPosition = town.transform.position;
                    return (Mathf.RoundToInt(GetEuclideanDistance(startingPosition)/10)*10).ToString();
                }catch{
                    Debug.LogWarning("Input Error");
                }
                return null;
            });
        }
    }

    public float GetEuclideanDistance(Vector3 startingPosition){
        return (transform.position - startingPosition).magnitude;
    }

    public Vector2 GetManhattanDistance(Vector3 startingPosition){
        return new Vector2(Mathf.Abs(transform.position.x-startingPosition.x),Mathf.Abs(transform.position.y-startingPosition.y));
    }

    public string[] GetSeparateDirection(Vector3 startingPosition){
        string direction = GetDirection(startingPosition);
        string xDir = direction.Length>5?direction.Substring(5):"";
        string yDir = direction.Substring(0,5);
        string[] dirArr = new string[2];
        dirArr[0] = xDir;
        dirArr[1] = yDir;
        return dirArr;
    }

    public string GetDirection(Vector3 startingPosition){
        Vector3 targetDir = transform.position - startingPosition;
        float angle = Vector3.Angle(targetDir,transform.forward);
        int region =  Mathf.RoundToInt(angle/45f);
        bool leftside = transform.position.x < startingPosition.x;
        string direction = "";
        switch(region){
            case 0: 
                direction = "north";
                break;
            case 1:
                direction = leftside?"northwest":"northeast";
                break;
            case 2:
                direction = leftside?"west":"east";
                break;
            case 3:
                direction = leftside?"southwest":"southeast";
                break;
            case 4:
                direction = "south";
                break;
        }
        return direction;
    }

    void OnDestroy() {
        instance = null;
    }
}
