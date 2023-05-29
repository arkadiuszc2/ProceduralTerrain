using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    public Tilemap grid;
    public Tile grass;
    public Tile grass1;
    public Tile grass2;
    public GameObject player;
    int chunkSize = 16;
    int chunkVisibilityDist = 20;
    public Vector2 playerPos;

    Dictionary<Vector2, TerrainChunk> terrainChunksDict = new Dictionary<Vector2, TerrainChunk> ();

    // Start is called before the first frame update
    void Start()
    {
        Tile tile = new Tile();
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
        }

   
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = new Vector2(player.transform.position.x, player.transform.position.y); //maybe not necessary
    }

    void UpdateVisibleChunks() {
        int currChunkXcoord = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currChunkYcoord = Mathf.RoundToInt(playerPos.y / chunkSize);

        for(int xOffset = -chunkVisibilityDist; xOffset <= chunkVisibilityDist; xOffset++) {
            for(int yOffset = -chunkVisibilityDist; yOffset<=chunkVisibilityDist; yOffset++) {
                Vector2 generatedChunkCoord = new Vector2(currChunkXcoord + xOffset, currChunkYcoord + yOffset);

                if (terrainChunksDict.ContainsKey(generatedChunkCoord)) {
                    // wyswietl
                }
                else {
                    Tile[,] tiles = new Tile[16, 16];
                    tiles = setTilesForChunk(generatedChunkCoord);
                    terrainChunksDict.Add(generatedChunkCoord, new TerrainChunk(generatedChunkCoord, tiles));
                    //dodaj nowy do slownika i wyswietl
                }
            }
        }
    }

    public Tile[,] setTilesForChunk(Vector2 chunkCoord) {
        TileMap[,] chunkTiles = new TileMap[16,16];
        for (int x = 0; x < 16; x++) {
            for (int y = 0; y < 16; y++) {
                chunkTiles[x, y] = checkTile();
            }
        }
        return null; //TO DO
    }

    public TileMap checkTile() {
        return null; //TO DO
    }

    public class TerrainChunk {
        private Vector2 generatedChunkCoord;
        private bool visibility;
        private Tile[,] tiles;

        public TerrainChunk(Vector2 chunkCoord, Tile[,] tiles) {
            generatedChunkCoord = chunkCoord;
            tiles = tiles; //maybe unnecessary
        }

        public void setActive() {
            //TO DO
        }
        
        
        
    }


}
