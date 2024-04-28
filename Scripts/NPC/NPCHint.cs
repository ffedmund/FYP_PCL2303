using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class NPCHint : MonoBehaviour
{
    public static NPCHint instance;

    public CrystalBall crystalBall;

    public MapGenerator mapGenerator;
    int SEED;

    public List<string> hintList = new List<string>();

    public Vector2 crystalBallChunkPosition;

    void Start()
    {   
        // int size = (int)(MapGenerator.mapChunkSize * EndlessTerrain.scale);
        mapGenerator = FindObjectOfType<MapGenerator>();
        SEED = mapGenerator.seed;

        crystalBallChunkPosition = LandmarkGenerator.crystalBallChunkPosition / (MapGenerator.mapChunkSize - 1);

        BiomeGenerator biomeGenerator = BiomeGenerator.instance;

        Invoke("GenerateHintData", 1.0f);
        
    }

    public bool isHintListEmpty() {
        return hintList.Count == 0;
    }

    public string getNewHint() {
        if (!isHintListEmpty()) {
            string hint = hintList[0];
            hintList.RemoveAt(0);
            return hint;
        }
        return "You have discovered all the hints!";
    }

    public void GenerateHintData()
    {
        string newHint = GetBetweenHint(new Vector2(crystalBallChunkPosition.x, crystalBallChunkPosition.y + 1), new Vector2(crystalBallChunkPosition.x, crystalBallChunkPosition.y - 1));
        if (!hintList.Contains(newHint)) hintList.Add(newHint);

        newHint = GetBetweenHint(new Vector2(crystalBallChunkPosition.x + 1, crystalBallChunkPosition.y), new Vector2(crystalBallChunkPosition.x - 1, crystalBallChunkPosition.y));
        if (!hintList.Contains(newHint)) hintList.Add(newHint);

        newHint = GetBetweenHint(new Vector2(crystalBallChunkPosition.x + 1, crystalBallChunkPosition.y + 1), new Vector2(crystalBallChunkPosition.x - 1, crystalBallChunkPosition.y - 1));
        if (!hintList.Contains(newHint)) hintList.Add(newHint);

        newHint = GetBetweenHint(new Vector2(crystalBallChunkPosition.x - 1, crystalBallChunkPosition.y + 1), new Vector2(crystalBallChunkPosition.x + 1, crystalBallChunkPosition.y - 1));
        if (!hintList.Contains(newHint)) hintList.Add(newHint);

        GetDirectionHint();

        GetTowerDistanceHint();

        System.Random rand = new System.Random();
        hintList = hintList.OrderBy(_ => rand.Next()).ToList();
    }

    private string GetBetweenHint(Vector2 biome1Coord, Vector2 biome2Coord)
    {
        int biome1Index = BiomeGenerator.instance.GetBiomeSetting(SEED, biome1Coord).index;
        int biome2Index = BiomeGenerator.instance.GetBiomeSetting(SEED, biome2Coord).index;

        string biome1Name = BiomeGenerator.instance.GetBiomeName(biome1Index);
        string biome2Name = BiomeGenerator.instance.GetBiomeName(biome2Index);

        if (biome1Index == biome2Index)
        {
            return "The crystal ball is between 2 " + biome1Name + "s.";
        }
        else
        {
            return "The crystal ball is between " + biome1Name + " and " + biome2Name + ".";
        }
    }

    private void GetDirectionHint()
    {
        string[] directions = new string[] { "north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest" };

        for (int i = 0; i < directions.Length; i++)
        {
            if (CrystalBall.instance.GetDirection(new Vector3(0, 0, 0)) != directions[i])
            {
                hintList.Add("The crystal ball is not to the " + directions[i] + " of the town.");
            }
        }
    }

    private void GetTowerDistanceHint()
    {
        hintList.Add(GetOldTowerDistanceHint());
        hintList.Add(GetNewTowerDistanceHint());
    }

    private Vector2 GetChunkManhattanDistance(Vector2 startingPosition, Vector2 targetPosition)
    {
        return new Vector2(Mathf.Abs(targetPosition.x - startingPosition.x), Mathf.Abs(targetPosition.y - startingPosition.y));
    }

    private string GetOldTowerDistanceHint()
    {
        Vector2 distance = GetChunkManhattanDistance(LandmarkGenerator.oldTowerChunkPosition / (MapGenerator.mapChunkSize - 1), crystalBallChunkPosition);
        if (Math.Max(distance.x, distance.y) <= 4)
        {
            return "The crystal ball is within 4 chunks of the old tower.";
        }
        else
        {
            return "The crystal ball is more than 4 chunks away from the old tower.";
        }
    }

    private string GetNewTowerDistanceHint()
    {
        Vector2 distance = GetChunkManhattanDistance(LandmarkGenerator.newTowerChunkPosition / (MapGenerator.mapChunkSize - 1), crystalBallChunkPosition);
        if (Math.Max(distance.x, distance.y) <= 4)
        {
            return "The crystal ball is within 4 chunks of the new tower.";
        }
        else
        {
            return "The crystal ball is more than 4 chunks away from the new tower.";
        }
    }

}
