using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour {
    public Tilemap grid;
    public Tile grass;
    public Tile grass1;
    public Tile grass2;
    public GameObject player;
    int chunkSize = 8;
    int chunkVisibilityDist = 1; //player is able to see 1 chunk in every direction (and chunk on which he is currently standing)
    public Vector2 playerPos;

    public float[,] elevationMap;
    public bool[,] riversMap;
    public float[,] moistureMap;
    public float[,] temperatureMap;


    Dictionary<Vector2, TerrainChunk> terrainChunksDict = new Dictionary<Vector2, TerrainChunk>();
    Dictionary<Vector2, TerrainChunk> visibleChunks = new Dictionary<Vector2, TerrainChunk>();
    // Start is called before the first frame update
    void Start() {
        player.transform.position = player.transform.position + new Vector3(128, 128, 0); //set player position on the middle of first chunk 
        generateMap(0, 0);

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
        float persistance = 0.5f;
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

        for (int xOffset = -chunkVisibilityDist; xOffset <= chunkVisibilityDist; xOffset++) {
            for (int yOffset = -chunkVisibilityDist; yOffset <= chunkVisibilityDist; yOffset++) {
                Vector2 generatedChunkCoord = new Vector2(currChunkXcoord + xOffset, currChunkYcoord + yOffset);

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
                    tiles = generateTilesForChunk(generatedChunkCoord);
                    TerrainChunk terrainChunk = new TerrainChunk(generatedChunkCoord, tiles);
                    terrainChunksDict.Add(generatedChunkCoord, terrainChunk);
                    terrainChunk.setActive(true, grid); //show tiles while creating new chunk
                }


            }
        }
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
            ;
            int chunkX = Mathf.RoundToInt(chunkCoord.x);
            int chunkY = Mathf.RoundToInt(chunkCoord.y);
            //int a = chunkSize * chunkX + TileX;
            // int b = chunkSize * chunkY + TileY;
            // Debug.Log("a: " + a + " b: " + b);
            float elevation = elevationMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];

            float temperature = temperatureMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];
            float moisture = moistureMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];
            bool isRiver = riversMap[chunkSize * chunkX + TileX, chunkSize * chunkY + TileY];

            if (isRiver == true) {
                // return lakeTile;  //add lake tile to script
                return grass; //for tests
            }


            if (elevation < 0.3) {
                return grass;           //for rendering tests
                                        //  return depthTile;
            } else if (elevation < 0.39) {
                return grass;
                // return lakeTile;
            } else if (elevation < 0.62) {
                if (temperature < 0.39) {
                    if (moisture < 0.39) {
                        //return snowTile;
                        return grass;
                    } else if (moisture < 0.49) {
                        return grass;
                        //  return tundraTile;
                    } else {
                        return grass;
                        //   return taigaTile;
                    }
                } else if (temperature < 0.49) {
                    if (moisture < 0.39) {
                        return grass;
                        //  return woodlandTile;
                    } else if (moisture < 0.49) {
                        return grass;
                        //  return grasslandTile;
                    } else {
                        return grass;
                        // return forestTile;
                    }
                } else {
                    if (moisture < 0.39) {
                        return grass;
                        //  return desertTile;
                    } else if (moisture < 0.49) {
                        return grass;
                        //   return savannaTile;
                    } else {
                        return grass;
                        //   return tropicalForestTile;
                    }
                }
            } else {
                if (temperature < 0.40) {
                    return grass;
                    //  return snowTile;
                } else {
                    return grass;
                    //  return rocksTile;
                }
            }

            return null; //maybe unnecessary
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
