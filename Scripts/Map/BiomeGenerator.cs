using System.Collections.Generic;
using FYP;
using UnityEngine;

[System.Serializable]
public struct BiomeData{
    public int index;
    public BiomeSetting biomeSetting;
    public int[] connectBiomeIndex;
}

public class BiomeGenerator : MonoBehaviour {
    [SerializeField] int noiseScale;
    [SerializeField] int octaves;//Increase the complexity
    [Range(0,1)]
    [SerializeField] float persistance;//Change the amplitude
    [SerializeField] float lacunarity;// Change the frequency
    public BiomeSetting[] biomeSettings;
    public Dictionary<int,BiomeSetting> biomeSettingDictionary = new Dictionary<int, BiomeSetting>();
    public static BiomeGenerator instance;
    void Awake(){
        instance = this;
        foreach(BiomeSetting setting in biomeSettings){
            if(biomeSettingDictionary.ContainsKey(setting.index)){
                continue;
            }
            biomeSettingDictionary.Add(setting.index,setting);
        }
    }

    void OnValidate() {
        foreach(BiomeSetting setting in biomeSettings){
            if(biomeSettingDictionary.ContainsKey(setting.index)){
                continue;
            }
            biomeSettingDictionary.Add(setting.index,setting);
        }
    }


    public BiomeData GetBiomeSetting(int seed, Vector2 centre){
        if(biomeSettings.Length == 0){
            throw new System.Exception();
        }
        Vector2 chunkCoordinate = centre/(MapGenerator.mapChunkSize-1);
        int biomeIndex = Mathf.RoundToInt(Noise.GeneratePerlinNoise(noiseScale, seed, octaves, persistance, lacunarity, chunkCoordinate)*10);
        // Debug.Log($"Biome Index: {biomeIndex}");
        BiomeSetting setting = biomeSettingDictionary[NearestBiomeSettingIndex(biomeIndex)];
        BiomeData biome = new BiomeData{
            index = setting.index,
            biomeSetting = setting,
            connectBiomeIndex = new int[8]
        };
        
        int index = 0;
        for(int i = 1; i > -2; i--){
            for(int j = -1; j < 2; j++){
                if(i == 0 && j == 0){
                    continue;
                }
                int connectBiomeIndex = Mathf.RoundToInt(Noise.GeneratePerlinNoise(noiseScale, seed, octaves, persistance, lacunarity, chunkCoordinate + new Vector2(j,i))*10);
                biome.connectBiomeIndex[index++] = NearestBiomeSettingIndex(connectBiomeIndex);
            }
        }

        return biome;
    }

    int NearestBiomeSettingIndex(int index)
    {

        for(int i = 1; i < biomeSettings.Length; i++)
        {
            if(index < biomeSettings[i].index)
            {
                return biomeSettings[i-1].index;
            }
        }

        return biomeSettings[biomeSettings.Length-1].index;
    }

    public AnimationCurve GetBiomeHeightCurve(int index){
        BiomeSetting setting = biomeSettings[0];
        if(biomeSettingDictionary.ContainsKey(index))
        {
            setting = biomeSettingDictionary[index];
        }
        return new AnimationCurve(setting.meshHeightCurve.keys);
    }


    public string GetBiomeName(int index){
        BiomeSetting setting = biomeSettings[0];
        if(biomeSettingDictionary.ContainsKey(index))
        {
            setting = biomeSettingDictionary[index];
        }
        return setting.name;
    }

    public MinimapColorBlock[] GetBiomeColorBlock(int index){
        BiomeSetting setting = biomeSettings[0];
        if(biomeSettingDictionary.ContainsKey(index))
        {
            setting = biomeSettingDictionary[index];
        }
        return setting.colorBlock;
    }

}