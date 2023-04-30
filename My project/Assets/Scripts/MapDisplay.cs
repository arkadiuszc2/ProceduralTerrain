using System.Collections;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    public Renderer textureRender;

    public void DrawNoiseMap(float[,] noiseMap, bool rivers, bool[,] riversMap, float[,] moistureMap, float[,] temperatureMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (rivers == true) {
                    colorMap[y * width + x] = checkColor(noiseMap[x, y], riversMap[x, y], moistureMap[x, y], temperatureMap[x, y]);
                } else {
                    colorMap[y * width + x] = checkColor(noiseMap[x, y], false, moistureMap[x, y], temperatureMap[x, y]);
                }
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }


    public Color checkColor(float elevation, bool river, float moisture, float temperature) {
        Biomes biom = checkBiom(elevation, moisture, temperature);
        if (river == true) { //rivers
            return new Color(0.2588f, 0.7607f, 0.8980f);
        } else if (biom == Biomes.Depth) {
            return new Color(0.247f, 0.6745f, 0.96078f);
        } else if (biom == Biomes.Lake) {
            return new Color(0.2588f, 0.7607f, 0.8980f);
        } else if (biom == Biomes.Desert) {
            return new Color(1.0f, 0.87058f, 0.0f);
        } else if (biom == Biomes.Grassland) {
            return new Color(0.4196f, 0.7490f, 0.22745f);
        } else if (biom == Biomes.Forest) {
            return new Color(0.1686f, 0.70196f, 0.12941f);
        } else if (biom == Biomes.Rocks) {
            return new Color(0.7022f, 0.7022f, 0.7022f);
        } else if (biom == Biomes.Woodland) {
            return new Color(0.85882f, 0.8039f, 0.0627f);
        } else if (biom == Biomes.Snow || biom == Biomes.SnowyDesert) {
            return new Color(0.9647f, 0.9686f, 0.9098f);
        } else if (biom == Biomes.Tundra) {
            return new Color(0.6f, 0.6f, 0.0f);
        } else if (biom == Biomes.Taiga) {
            return new Color(0.0f, 0.4f, 0.2f);
        } else if (biom == Biomes.Savanna) {
            return new Color(0.8f, 0.4f, 0.0f);
        } else if (biom == Biomes.TropicalForest) {
            return new Color(0.6f, 1.0f, 0.2f );
        } else {
            return Color.black;
        }

    }

    public Biomes checkBiom(float elevation, float moisture, float temperature) {
        // temperature 0.5 = 15 stopni, 0 = -15, 1 = 45
        if (elevation < 0.3) {              //temperature ranges 0-0.39, 0.39 - 0.49, 0.49 - 1   -> it depends on what we want achieve (more reality or equal distribution)
            return Biomes.Depth;            // same moisture
        } else if (elevation < 0.39) {      // elevation ranges 0-0.39(only water), 0.39 - 0.62 (biomes normally generated), 0.62 -1 ( rocks or snowy rocks)
            return Biomes.Lake;
        } else if (elevation < 0.62) {
            if (temperature < 0.39) {
                if (moisture < 0.39) {
                    return Biomes.SnowyDesert;
                } else if (moisture < 0.49) {
                    return Biomes.Tundra;
                } else {
                    return Biomes.Taiga;
                }
            } else if (temperature < 0.49) {
                if (moisture < 0.39) {
                    return Biomes.Woodland;
                } else if (moisture < 0.49) {
                    return Biomes.Grassland;
                } else {
                    return Biomes.Forest;
                }
            } else {
                if (moisture < 0.39) {
                    return Biomes.Desert;
                } else if (moisture < 0.49) {
                    return Biomes.Savanna;
                } else {
                    return Biomes.TropicalForest;
                }
            }
        } else {
            if (temperature < 0.40) {
                return Biomes.Snow;
            } else {
                return Biomes.Rocks;
            }
        }

    }
}
