using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    public Tilemap grid;
    public Tile grass;
    public GameObject player;
    int chunkSize = 16;
    int chunkVisibilityDist = 20;
    public Vector2 playerPos;

    Dictionary<Vector2, TerrainChunk> terrainChunksDict = new Dictionary<Vector2, TerrainChunk> ();

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x<5; x++) {
            for(int y=0; y<5; y++) {
                grid.SetTile(new Vector3Int(x,y,0), grass);
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

        for(int xOffset = -chunkVisibilityDistt; xOffset <= chunkVisibilityDist; xOffset++) {
            for(int yOffset = -chunkVisibilityDist; yOffset<=chunkVisibilityDist; yOffset++) {
                Vector2 generatedChunkCoord = new Vector2(currChunkXcoord + xOffset, currChunkYcoord + yOffset);

                if (terrainChunksDict.ContainsKey(generatedChunkCoord)) {
                    // wyswietl
                }
                else {
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
    }

    public TileMap checkTile() {

    }

    public class TerrainChunk {
        private Vector2 generatedChunkCoord;
        private bool visibility;
        private Tile[,] tiles;

        public TerrainChunk(Vector2 chunkCoord) {
            generatedChunkCoord = chunkCoord;
            tiles = setTiles(chunkCoord); //maybe unnecessary
        }

        public void setActive() {
            //TO DO
        }
        
        
        
    }


}
