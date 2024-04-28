using UnityEngine;
using IngameDebugConsole;
using FYP;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.IO;
using Tayx.Graphy;

public class TestingCommand : MonoBehaviour{
    
    public NavMeshSurface navMeshSurface;
    public GameObject inGameConsole;
    public GameObject fpsReader;

    [ConsoleMethod( "attribute", "Edit player attribute by key and value")]
    public static void attribute(string key, int value, bool isReplace = false){
        if(isReplace)UIController.playerData.ReplacePlayerData(key,value);
        else UIController.playerData.AddPlayerData(key,value);
    }

    [ConsoleMethod( "bake", "Bake the AI navigation")]
    public static void bake(){
        FindAnyObjectByType<TestingCommand>().navMeshSurface.BuildNavMesh();
    }

    [ConsoleMethod( "treechunk", "Get the world tree chunk")]
    public static void treechunk(){
        Debug.Log(LandmarkGenerator.worldTreeChunkPosition/(MapGenerator.mapChunkSize-1));
    }

    [ConsoleMethod( "crystalball", "Get the world tree chunk")]
    public static void crystalball(){
        Debug.Log(LandmarkGenerator.crystalBallChunkPosition/(MapGenerator.mapChunkSize-1));
    }

    [ConsoleMethod( "cb-dir", "Get the crystal ball chunk")]
    public static void crystalBallDir(){
        Debug.Log(CrystalBall.instance.GetDirection(FindFirstObjectByType<PlayerManager>().transform.position));
    }

    [ConsoleMethod("time", "Time command")]
    public static void time(string method){
        switch(method){
            case "value":
                Debug.Log("Current Time: "+DayCycleManager.instance.currentTime);
                break;
            case "get":
                float currentTime = DayCycleManager.instance.currentTime;
                int hour = (int)currentTime/15;
                int minute = Mathf.FloorToInt(currentTime - hour*15)*4;
                Debug.Log("Current Time: "+$"{(hour<10?"0":"")}{hour}:{(minute<10?"0":"")}{minute}");
                break;
            default:
                break;
        }
    }

    [ConsoleMethod("time", "Time command")]
    public static void time(string method, string timeText = ""){
            switch(method){
                case "set":
                    DayCycleManager.instance.SetTime(timeText);
                    break;
                default:
                    break;
            }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F3)){
            if(fpsReader == null){
                fpsReader = FindAnyObjectByType<GraphyManager>().gameObject;
            }
            if(inGameConsole == null){
                inGameConsole = FindAnyObjectByType<DebugLogManager>().gameObject;
            }
            fpsReader.SetActive(!fpsReader.activeSelf);
            inGameConsole.SetActive(!inGameConsole.activeSelf);
        }
    }

    // Function to save y-values to a file
    public static void SaveYValuesToFile(Vector3[] vertices, string filename)
    {
        string path = Path.Combine(Application.dataPath, filename);

        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (Vector3 vertex in vertices)
            {
                writer.WriteLine($"{vertex.x},{vertex.z} : " + vertex.y.ToString());
            }
        }

        Debug.Log("Y-values saved to: " + path);
    }

}