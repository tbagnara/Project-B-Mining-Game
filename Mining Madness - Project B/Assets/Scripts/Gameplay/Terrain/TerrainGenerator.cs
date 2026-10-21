using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public int chunkSize = 16;
    public int worldSize = 200;
    public bool generateCaves = true;
    public int dirtLayerHeight = 5;
    public float surfaceValue = 0.25f;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;
    public PhysicsMaterial2D groundMaterial;

    [Header("Noise Settings")]
    public float terrainFreq = 0.05f;    
    public float noiseFreq = 0.05f;
    public float caveFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public float coalRarity;
    public float coalSize;
    public float copperRarity, copperSize;
    public float ironRarity, ironSize;
    public float diamondRarity, diamondSize;
    public float emeraldRarity, emeraldSize;
    public float rubyRarity, rubySize;
    public Texture2D coalSpread;
    public Texture2D copperSpread;
    public Texture2D ironSpread;
    public Texture2D diamondSpread;
    public Texture2D emeraldSpread;
    public Texture2D rubySpread;

    [Header ("Player Settings")]
    public GameObject player;
    
    private GameObject[] worldChunks;
    public List<Vector2> worldTiles = new List<Vector2>();
    public List<GameObject> worldTilesObjects = new List<GameObject>();
    
    private void OnValidate()
    {
        if (caveNoiseTexture == null || caveNoiseTexture.width != worldSize) 
        {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalSpread = new Texture2D(worldSize, worldSize);
            copperSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);
            emeraldSpread = new Texture2D(worldSize, worldSize);
            rubySpread = new Texture2D(worldSize, worldSize);
        }
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        // Ores
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(copperRarity, copperSize, copperSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);
        GenerateNoiseTexture(emeraldRarity, emeraldSize, emeraldSpread);
        GenerateNoiseTexture(rubyRarity, rubySize, rubySpread);
    }
    public void Start()
    {
        seed = UnityEngine.Random.Range(-10000, 10000);
        
        if (coalSpread == null || caveNoiseTexture.width != worldSize) 
        {
            coalSpread = new Texture2D(worldSize, worldSize);
            copperSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);
            emeraldSpread = new Texture2D(worldSize, worldSize);
            rubySpread = new Texture2D(worldSize, worldSize);
        }
        
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        // Ores
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(copperRarity, copperSize, copperSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);
        GenerateNoiseTexture(emeraldRarity, emeraldSize, emeraldSpread);
        GenerateNoiseTexture(rubyRarity, rubySize, rubySpread);

        CreateChunks();
        GenerateTerrain();
    }

    void Update()
    {
        RefreshChunks();
    }

    void RefreshChunks()
    {
        for (int i = 0; i < worldChunks.Length; i++ )
        {
            if (Mathf.Abs(i * chunkSize + (chunkSize /2 ) - player.transform.position.x) > Camera.main.orthographicSize*4f)
            {
                worldChunks[i].SetActive(false);
            }
            else
            {
                worldChunks[i].SetActive(true);
            }
        }
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i< numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    
    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x ++ )
        {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier * heightAddition;
            for (int y = 0; y < worldSize; y++) // changed height to world size
            {
                Sprite tileSprite;
                tileSprite = tileAtlas.stone.tileSprite;
                
                if (coalSpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.coal.tileSprite;
                else if (ironSpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.iron.tileSprite;
                else if (copperSpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.copper.tileSprite;
                else if (diamondSpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.diamond.tileSprite;
                else if (emeraldSpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.emerald.tileSprite;
                else if (rubySpread.GetPixel(x,y).r > 0.5f) 
                    tileSprite = tileAtlas.ruby.tileSprite;
                else
                    tileSprite = tileAtlas.stone.tileSprite;
                
                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x,y).r > 0.5f)
                    {
                        PlaceTile(tileSprite, x, y);
                    }
                }
                else
                {
                    PlaceTile(tileSprite, x, y);
                }

                if (y >= height -1)
                {
                    // Above ground generation NA;
                    continue;
                }

            }
        }
        
        RemoveSpawnLocation();

    }
    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit) 
                    noiseTexture.SetPixel(x,y, Color.white);
                else 
                    noiseTexture.SetPixel(x,y, Color.black);
            }
        }

        noiseTexture.Apply();
    }

    private void RemoveSpawnLocation()
    {
        int radius = 3;

        for (int y = 0; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if ((x * x) + (y * y) <= (radius * radius))
                {
                    RemoveTile(worldSize/2 + x, worldSize*3/4 + y);
                }
                
            }
            
        }
    }

    public string RemoveTile(int x, int y)
    {
        if (worldTiles.Contains(new Vector2Int(x,y)) && x >= 1 && x < worldSize -1 && y >= 1 && y < worldSize -1 )
        {
            string ret = worldTilesObjects[worldTiles.IndexOf(new Vector2(x,y))].GetComponent<SpriteRenderer>().sprite.name;
            Destroy(worldTilesObjects[worldTiles.IndexOf(new Vector2(x,y))] );
            return ret;
        }
        return null;
    }
    public Sprite GetTileSprite(int x, int y)
    {
        try
        {
            if (worldTiles.Contains(new Vector2Int(x,y)) && x >= 1 && x < worldSize -1 && y >= 1 && y < worldSize -1)
            {
                Sprite ret = worldTilesObjects[worldTiles.IndexOf(new Vector2(x,y))].GetComponent<SpriteRenderer>().sprite;
                return ret;
            }
        }
        catch
        {
            
        }
        return null;
    }
    public void PlaceTile(Sprite tileSprite, int x, int y)
    {
        if (!worldTiles.Contains(new Vector2Int(x,y)))
        {
            GameObject newTile = new GameObject();
            
            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;

            newTile.transform.parent = worldChunks[chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
            newTile.name = tileSprite.name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
            newTile.AddComponent<BoxCollider2D>();

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
            worldTilesObjects.Add(newTile);
        }
    }
}
