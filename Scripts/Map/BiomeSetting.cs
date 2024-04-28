using FYP;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeSetting", menuName = "Biome/BiomeSetting", order = 0)]
public class BiomeSetting : ScriptableObject {
    public string biomeName;
    public int index;
    public ParticleType weatherParticleType;
    public AnimationCurve meshHeightCurve;
    public Material material;
    [Header("Creature")]
    public LairSetting[] lairSetting;
    [Header("Environment")]
    public EnvironmentPrefabData[] stones;
    public EnvironmentPrefabData[] trees;
    public EnvironmentPrefabData[] structures;
    [Header("Minimap")]
    public MinimapColorBlock[] colorBlock;
    [Header("Water")]
    public float waterLevel;
    [Header("Grass")]
    public bool removeGrass;
} 