using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour {
    public Tilemap grid;
    public Tile deepWater;
    public Tile water;
    public Tile tundra;
    public Tile taiga;
    public Tile woodland;
    public Tile forest;
    public Tile grass;
    public Tile rocks;
    public Tile snow;
    public Tile jungle;
    public Tile savanna;
    public Tile desert;


    public GameObject player;
    int chunkSize = 8;
    int chunkVisibilityDist = 1; //player is able to see 1 chunk in every direction (and chunk on which he is currently standing)
    public Vector2 playerPos;
    int mapSize = 16;  //meaning that one mam have 16x16 chunks
    int mapSizeInPixels = 255; //meaning size of creating map (elevation map, moistureMap etc. )

    public float[,] elevationMap;  //later delete this variables and put into currentMap
    public bool[,] riversMap;
    public float[,] moistureMap;
    public float[,] temperatureMap;

    private Map currentMap;

    Dictionary<Vector2, TerrainChunk> terrainChunksDict = new Dictionary<Vector2, TerrainChunk>();
    Dictionary<Vector2, TerrainChunk> visibleChunks = new Dictionary<Vector2, TerrainChunk>();
    Dictionary<Vector2, Map> generatedMaps = new Dictionary<Vector2, Map>();
    // Start is called before the first frame update
    void Start() {
        player.transform.position = player.transform.position + new Vector3(128, 128, 0); //set player position on the middle of first chunk 
        generateMap(0, 0);
        currentMap = new Map(new Vector2(0,0), elevationMap, riversMap, moistureMap, temperatureMap);
        /* Tile tile = new Tile();
         for(int x = 0; x<10; x++) {
             for(int y=0; y<10; y++) {
                 float num = Random.Range(0, 3);

                 if(num < 1) {
                     tile = grass;
                 } else if(num < 2) {
                     tile = grass1;
                 } else {
                     tile = grass2;
                 }
                 grid.SetTile(new Vector3Int(x,y,0), tile);
             }
         }*/


    }

    // Update is called once per frame
    void Update() {
        playerPos = new Vector2(player.transform.position.x, player.transform.position.y); //maybe not necessary
        UpdateVisibleChunks();
    }


    public void generateMap(int xOffset, int yOffset) {
        //bool onlyHeightsMap;
        int mapWidth = 255;
        int mapHeight = 255;
        float noiseScale = 3f;
        int octaves = 6;
        float persistance = 0.5f;//
        float lacunarity = 2f;
        float firstOctaveFreq = 0.04f;
        bool islands = false;
        bool mountainChain = false;
        //int xOffset;
        //int yOffset;
        //bool rivers;
        int numOfRivers = 10;
        int riversLength = 200;
        int mtOctaves = 6;
        float mtPersistance = 0.5f;
        float mtLacunarity = 2f;
        float mtFirstOctaveFreq = 0.02f;
        int tempOctaves = 6;
        float tempPersistance = 0.5f;
        float tempLacunarity = 2f;
        float tempFirstOctaveFreq = 0.005f;

        elevationMap = Noise.GenerateElevationMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, islands, mountainChain, xOffset, yOffset, firstOctaveFreq);
        riversMap = Rivers.GenerateRiversMap(elevationMap, numOfRivers, riversLength);
        moistureMap = Noise.GenerateMoistureMap(mapWidth, mapHeight, noiseScale, mtOctaves, mtPersistance, mtLacunarity, xOffset, yOffset, mtFirstOctaveFreq);
        temperatureMap = Noise.GenerateMoistureMap(mapWidth, mapHeight, noiseScale, tempOctaves, tempPersistance, tempLacunarity, xOffset, yOffset, tempFirstOctaveFreq);
    }

    public void UpdateVisibleChunks() {


        int currChunkXcoord = Mathf.FloorToInt(playerPos.x / chunkSize); //0, 0.1 0.8 0.9 are still chunk 0,0 but 1 is chunk 1,1
        int currChunkYcoord = Mathf.FloorToInt(playerPos.y / chunkSize);


        //checking if any chunk which are too far are visible and making them invisible
        if (visibleChunks.Count > 9) {
            List<Vector2> keysCopy = new List<Vector2>(visibleChunks.Keys);
            foreach (Vector2 chunkCoords in keysCopy) {
                if (Mathf.Abs(chunkCoords.x - currChunkXcoord) > 1 || Mathf.Abs(chunkCoords.y - currChunkYcoord) > 1) {
                    TerrainChunk terrainChunk;
                    terrainChunksDict.TryGetValue(chunkCoords, out terrainChunk);
                    terrainChunk.setActive(false, grid);
                    visibleChunks.Remove(chunkCoords);
                }
            }
        }

        for (int i = -1; i <= 1; i++) { //maybe increment by xOffset+chunkVisibilityDist
            for (int j = -1; j <= 1; j++) {
                Vector2 generatedChunkCoord = new Vector2(currChunkXcoord + i, currChunkYcoord + j);
                //Debug.Log("koordy chunka: " + generatedChunkCoord);
                if (terrainChunksDict.ContainsKey(generatedChunkCoord)) {
                    TerrainChunk terrainChunk;
                    terrainChunksDict.TryGetValue(generatedChunkCoord, out terrainChunk);

                    //add to dictionary with visible chunks if its not already present
                    if (visibleChunks.ContainsKey(generatedChunkCoord) == false) {
                        visibleChunks.Add(generatedChunkCoord, terrainChunk);
                    }
                    if (terrainChunk.visibility != true) {  // show chunk only if its not already shown
                        terrainChunk.setActive(true, grid);
                    } else {

                    }

                } else {
                    Tile[,] tiles = new Tile[chunkSize, chunkSize];

                    if (chunkOutsideCurrentMapRange(currChunkXcoord, currChunkYcoord)) {
                        int currMapXcoord = Mathf.FloorToInt(currChunkXcoord / mapSize); //0, 0.1 0.8 0.9 are still chunk 0,0 but 1 is chunk 1,1
                        int currMapYcoord = Mathf.FloorToInt(currChunkYcoord / mapSize);
                        Vector2 expectedMapCoords = new Vector2(currMapXcoord, currMapYcoord);

                        if (generatedMaps.ContainsKey(expectedMapCoords)) {
                            generatedMaps.TryGetValue(expectedMapCoords, out currentMap);      // try to find in maps dictionary map with expected coords
                            elevationMap = currentMap.ElevationMap;   //maybe then do it to look better then 4 lines of code
                            riversMap = currentMap.RiversMap;
                            moistureMap = currentMap.MoistureMap;
                            temperatureMap = currentMap.TemperatureMap;
                        } else {
                            Vector2 offsets = calculateOffsets(currentMap.Coords, expectedMapCoords);
                            generateMap(Mathf.RoundToInt(offsets.x), Mathf.RoundToInt(offsets.y)); //generate and set current map from which are chunks generated to new map
                            Map newMap = new Map(expectedMapCoords, elevationMap, riversMap, moistureMap, temperatureMap);
                            generatedMaps.Add(expectedMapCoords, newMap);               //add to dictionary map with new coords (ex 0,0 or 5,0) 
                        }

                    }

                    tiles = generateTilesForChunk(generatedChunkCoord);
                    TerrainChunk terrainChunk = new TerrainChunk(generatedChunkCoord, tiles);
                    terrainChunksDict.Add(generatedChunkCoord, terrainChunk);
                    terrainChunk.setActive(true, grid); //show tiles while creating new chunk
                }


            }
        }
    }

    private bool chunkOutsideCurrentMapRange(int xCoord, int yCoord) {
        if(xCoord > currentMap.Coords.x*chunkSize + 7 || xCoord < currentMap.Coords.x * chunkSize) {
            return true;
        }
        if (yCoord > currentMap.Coords.y * chunkSize + 7 || yCoord < currentMap.Coords.y * chunkSize) {
            return true;
        }
        return false;

    }

    private Vector2 calculateOffsets(Vector2 currentMapCoords, Vector2 expectedMapCoords) { //do poprawki
        Vector2 offsets = new Vector2();

        if (expectedMapCoords.x > currentMapCoords.x) { //czy na pewno mapsize-1?
            offsets.y = 1;
        } else {
            offsets.y = -1;
        }

        if (expectedMapCoords.y > currentMapCoords.y) {  //czy na pewno mapsize-1 ?
            offsets.y = 1;
        } else {
            offsets.y = -1;
        }

        offsets.x = offsets.x * mapSizeInPixels;
        offsets.y = offsets.y * mapSizeInPixels;

        return offsets;

    }



    public Tile[,] generateTilesForChunk(Vector2 chunkCoord) {
        Tile[,] chunkTiles = new Tile[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                chunkTiles[x, y] = checkTile(chunkCoord, x, y);
            }
        }
        return chunkTiles;
    }

    public Tile checkTile(Vector2 chunkCoord, int TileX, int TileY) {
        
        int chunkX = Mathf.RoundToInt(chunkCoord.x);   //why roundToInt? becouse basically vector2 storages floats
        int chunkY = Mathf.RoundToInt(chunkCoord.y);
        //int a = chunkSize * chunkX + TileX;
        // int b = chunkSize * chunkY + TileY;
        // Debug.Log("a: " + a + " b: " + b);

        //fix coord according to currentMap (zeby nie probowalo wzic np. chunka (-1,0) bo nie ma takich wspolrzednych w tablicy)
        //Vector2 fixedCoords = fixCoords(chunkX ,chunkX);
        //chunkX = Mathf.RoundToInt(fixedCoords.x);   //przez ta czec kodu swiat sie generowal zle, chunki sie powtarzaly
        //chunkY = Mathf.RoundToInt(fixedCoords.y);
        Debug.Log(""+(chunkSize * chunkX + TileX) + " " + (chunkSize * chunkY + TileY));
        float elevation = elevationMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];

        float temperature = temperatureMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];
        float moisture = moistureMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];
        bool isRiver = riversMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];

        if (isRiver == true) {
            // return lakeTile;  //add lake tile to script
            return grass; //for tests
        }


        if (elevation < 0.3) {
            return deepWater;           //for rendering tests
                                    //  return depthTile;
        } else if (elevation < 0.39) {
            return water;
            // return lakeTile;
        } else if (elevation < 0.62) {
            if (temperature < 0.39) {
                if (moisture < 0.39) {
                    //return snowTile;
                    return snow;
                } else if (moisture < 0.49) {
                    return tundra;
                    //  return tundraTile;
                } else {
                    return taiga;
                    //   return taigaTile;
                }
            } else if (temperature < 0.49) {
                if (moisture < 0.39) {
                    return woodland;
                    //  return woodlandTile;
                } else if (moisture < 0.49) {
                    return grass;
                    //  return grasslandTile;
                } else {
                    return forest;
                    // return forestTile;
                }
            } else {
                if (moisture < 0.39) {
                    return desert;
                    //  return desertTile;
                } else if (moisture < 0.49) {
                    return savanna;
                    //   return savannaTile;
                } else {
                    return jungle;
                    //   return tropicalForestTile;
                }
            }
        } else {
            if (temperature < 0.40) {
                return snow;
                //  return snowTile;
            } else {
                return rocks;
                //  return rocksTile;
            }
        }

        return null; //maybe unnecessary
    }

    private Vector2 fixCoords(int chunkX, int chunkY) {
        Vector2 fixedCoords = new Vector2();
        //narazie nie obluguje ujemnych wspolrzednych chunków
        if (chunkX >= 0) {
            fixedCoords.x = chunkX % mapSize;
        } else {
            fixedCoords.x = mapSize + chunkX % mapSize;  //+ because ex -14%16 = -14 not 14
        }

        if (chunkY >= 0) {
            fixedCoords.y = chunkY % mapSize;
        } else {
            fixedCoords.y = mapSize + chunkY % mapSize;
        }

        return fixedCoords;
    }

    public class TerrainChunk {
        private Vector2 generatedChunkCoord;
        public bool visibility;
        private Tile[,] tiles;

        public TerrainChunk(Vector2 chunkCoord, Tile[,] tiless) {
            generatedChunkCoord = chunkCoord;
            tiles = tiless; //maybe unnecessary
        }

        public void setActive(bool isActive, Tilemap grid) {
            visibility = isActive;

            if (visibility == true) {
                //make tiles visible
                for (int x = 0; x < 8; x++) {
                    for (int y = 0; y < 8; y++) {
                        int chunkX = Mathf.RoundToInt(generatedChunkCoord.x);
                        int chunkY = Mathf.RoundToInt(generatedChunkCoord.y);
                        grid.SetTile(new Vector3Int(chunkX * 8 + x, chunkY * 8 + y, 0), tiles[x, y]);
                    }
                }

            } else {
                //make tiles invisible
                for (int x = 0; x < 8; x++) {
                    for (int y = 0; y < 8; y++) {
                        int chunkX = Mathf.RoundToInt(generatedChunkCoord.x);
                        int chunkY = Mathf.RoundToInt(generatedChunkCoord.y);
                        grid.SetTile(new Vector3Int(chunkX * 8 + x, chunkY * 8 + y, 0), null);
                    }
                }
            }
            //TO DO
        }



    }


}
