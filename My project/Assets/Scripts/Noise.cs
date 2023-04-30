using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Noise {
    public static float[,] GenerateElevationMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, bool islands, bool mountainChain, int xOffset, int yOffset, float firstOctaveFreq) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) {
            scale = 0.0001f;
        }

        // float minNoiseVal = 1;
        // float maxNoiseVal = 0;

        /*if (mountainChain == true) {
            firstOctaveFreq = 0.011f;
            persistance = 0.2f;
            lacunarity = 5;
            octaves = 6;
        }*/

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float amplitude = 1;
                float frequency = firstOctaveFreq;
                float elevation = 0;
                float divider = 0;
                int octaveOmmit = 0;

                if (mountainChain == true) {
                    divider += 2;
                    float sampleX = (x + xOffset + 61) / 1.5f/scale * 0.011f;
                    float sampleY = (y + yOffset + 122) / 1.5f/scale * 0.011f;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    elevation += perlinValue * 2;
                    octaveOmmit++;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                for (int i = 0 + octaveOmmit; i < octaves; i++) {
                    divider += amplitude;
                    float sampleX = (x + xOffset) / scale * frequency;
                    float sampleY = (y + yOffset) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    elevation += perlinValue * amplitude;



                    amplitude *= persistance;
                    frequency *= lacunarity;
                }


                // for code with lerp distribution
                /*if (elevation > maxNoiseVal) {
                    maxNoiseVal = elevation;
                } else if (elevation < minNoiseVal) {
                    minNoiseVal = elevation;
                }*/

                noiseMap[x, y] = elevation / divider;
            }
        }


        //code below makes better elevation distribution (allows to use more the full range from 0 to 1) BUT it also prevents from easy
        // scrolling of the map because it lerps values which are showed at the moment so if we scroll a little right/left and new bigger 
        //elevation than every on past map will be found then colours on the map will change (lerp changes the range)

        /*for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if (valleys == true) {
                    noiseMap[x, y] = Mathf.Pow(noiseMap[x, y], 2);  // warning: it enlarge only values bigger than 1!
                }   else if (mountainChain == true) {
                    if (x > 100 && x <= 125) {
                        noiseMap[x, y] = noiseMap[x, y] * x / 100f;
                        //noiseMap[x, y] = Mathf.Pow(noiseMap[x, y], 2);
                    } else if (x > 125 && x < 150) {
                        noiseMap[x, y] = noiseMap[x, y] * 150f / x;
                    }
                    if (noiseMap[x, y] > maxNoiseVal) {
                        noiseMap[x, y] = maxNoiseVal; 
                    }
                }

                noiseMap[x, y] = Mathf.InverseLerp(minNoiseVal, maxNoiseVal, noiseMap[x, y]);

            }
        }*/

        if (islands == true) {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    noiseMap[x, y] = Mathf.Pow(noiseMap[x, y], 1.7f);

                }
            }
        }

        /* if (mountainChain == true) {
             for (int y = 0; y < mapHeight; y++) {
                 for (int x = 0; x < mapWidth; x++) {
                     float frequency = firstOctaveFreq;
                     float amplitude = 2;


                     float sampleX = x / scale * frequency + xOffset / scale;
                     float sampleY = y / scale * frequency + yOffset / scale;

                     if (sampleX > mapWidth / 3.0f && sampleX < mapWidth / 2.0f) {

                         if (noiseMap[x, y] < 0.5f) {
                             noiseMap[x, y] += 0.5f / noiseMap[x,y];
                         }
                     }

                 }
             }
         }*/

        /*if (mountainChain == true) {
            float amplitude = 0.5f;
            float frequency = 0.011f;
            int mcOctaves = 1;
            float divider = 0;
            float elevation = 0;
            float mcPersistance = 1;
            float mcLacunarity = 1;
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    for (int i = 0; i < mcOctaves; i++) {
                        divider += amplitude;
                        float sampleX = (x+xOffset)/scale * frequency;
                        float sampleY = (y+yOffset)/scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                        elevation += perlinValue * amplitude;

                        amplitude *= mcPersistance;
                        frequency *= mcLacunarity;
                    }

                    noiseMap[x,y] = (noiseMap[x, y] + elevation)/divider;
                }
            }
        }*/

        return noiseMap;
    }

    public static float[,] GenerateMoistureMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int xOffset, int yOffset, float firstOctaveFreq) {
        float[,] moistureMap = new float[mapWidth, mapHeight];

        moistureMap = GenerateElevationMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, false, false,xOffset, yOffset, firstOctaveFreq);

        return moistureMap;
    }

    public static float[,] GenerateTemperatureMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int xOffset, int yOffset, float firstOctaveFreq) {
        float[,] moistureMap = new float[mapWidth, mapHeight];

        moistureMap = GenerateElevationMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity, false, false, xOffset, yOffset, firstOctaveFreq);

        return moistureMap;
    }

}
