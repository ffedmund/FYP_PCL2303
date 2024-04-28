using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(WaterGenerator))]
[RequireComponent(typeof(EnvironmentGenerator))]
[RequireComponent(typeof(CreatureLairManager))]
[RequireComponent(typeof(LandmarkGenerator))]
public class TerrainGenerationManager : MonoBehaviour {
    public const float scale = 8;
    public static MapGenerator mapGenerator;
    public static WaterGenerator waterGenerator;
    public static EnvironmentGenerator environmentGenerator;
    public static CreatureLairManager creatureLairGenerator;
    public static LandmarkGenerator landmarkGenerator;

    void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        waterGenerator = FindObjectOfType<WaterGenerator>();
        environmentGenerator = FindObjectOfType<EnvironmentGenerator>();
        creatureLairGenerator = FindObjectOfType<CreatureLairManager>();
        landmarkGenerator = FindObjectOfType<LandmarkGenerator>();
    }
}