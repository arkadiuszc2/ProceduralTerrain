using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public bool onlyHeightsMap;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float firstOctaveFreq;
    public bool islands;
    public bool mountainChain;
    public int xOffset;
    public int yOffset;
    public bool rivers;
    public int numOfRivers;
    public int riversLength;
    public int mtOctaves;
    public float mtPersistance;
    public float mtLacunarity;
    public float mtFirstOctaveFreq;
    public int tempOctaves;
    public float tempPersistance;
    public float tempLacunarity;
    public float tempFirstOctaveFreq;


    public void GenerateMap() {
        float[,] elevationMap = Noise.GenerateElevationMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, islands, mountainChain, xOffset, yOffset, firstOctaveFreq);
        bool[,] riversMap = Rivers.GenerateRiversMap(elevationMap, numOfRivers, riversLength);
        float[,] moistureMap = Noise.GenerateMoistureMap(mapWidth, mapHeight, noiseScale, mtOctaves, mtPersistance, mtLacunarity, xOffset, yOffset, mtFirstOctaveFreq);
        float[,] temperatureMap = Noise.GenerateMoistureMap(mapWidth, mapHeight, noiseScale, tempOctaves, tempPersistance, tempLacunarity, xOffset, yOffset, tempFirstOctaveFreq);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(elevationMap, onlyHeightsMap, rivers, riversMap, moistureMap, temperatureMap);
    }
}
