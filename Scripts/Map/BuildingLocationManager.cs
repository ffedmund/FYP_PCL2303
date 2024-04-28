using System.Collections.Generic;
using FYP;
using UnityEngine;

public class BuildingLocationManager : MonoBehaviour {
    
    #region Singleton
    public static BuildingLocationManager instance;
    private void Awake() {
        if(instance == null){
            instance = this;
        }
    }
    #endregion
    [System.Serializable]
    public struct Location{
        public GameObject building;
        public string buildingName;
        public bool isNPCLocation;
        public NPC[] residentNPCArray;
    }
    public List<Location> locationList = new List<Location>();
    Dictionary<string,Location> npcAddressDictionary;

    void Start() {
        npcAddressDictionary = new Dictionary<string, Location>();
        foreach(Location location in locationList){
            foreach(NPC npc in location.residentNPCArray){
                npcAddressDictionary.Add(npc.npcName,location);
            }
        }
    }

    public Transform GetNPCHomeAddress(string name){
        if(!npcAddressDictionary.ContainsKey(name)){
            return null;
        }
        return npcAddressDictionary[name].building.transform;
    }

    public Transform GetRandomNPCLocation(){
        // Debug.Log("Finding Location");
        List<Location> npcLocations = locationList.FindAll((location) => location.isNPCLocation);
        if(npcLocations.Count == 0){
            return null;
        }
        int randomIndex = Random.Range(0,npcLocations.Count);
        Location targetLocation = npcLocations[randomIndex];
        // Debug.Log("Go to location" + targetLocation.buildingName);
        return targetLocation.building.transform;
    }
}