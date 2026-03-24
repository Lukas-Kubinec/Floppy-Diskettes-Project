using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameManager gameManager;

    // Uses Perlin, Cnoise and Snoise(Simplex) noise generators
    [Header("Noise Generator")]
    public bool usePerlinNoise = true; // Default method
    public bool useCNoise = false;
    public bool useSNoise = false;

    // Size of the generated map
    [Header("Size of generated texture")]
    public int xTexSize = 513; // Must be set to size of terrain
    public int yTexSize = 513; // Must be set to size of terrain
    public bool automaticSize = false; // Automatically sets texture size to terrain size

    // Origin of the generated world
    [Header("Texture offset")]
    public float xTexOffset;
    public float yTexOffset;

    // Map Detail setting
    [Header("Map detail setting")]
    public float mapDetail = 1.0f;
    public float heightAdjustment = 1.0f;
    public bool randomSeed = false; // Randomly offsets texture

    // Colours
    [Header("Colour setings")]
    public Color WaterColour;
    public Color SandColour;
    public Color GrassColour;
    public Color MountainColour;

    // Terrain height levels
    public float waterHeightLevel = 0.25f; // deepest
    public float sandHeightLevel = 0.5f;
    public float grassHeightLevel = 0.75f;
    public float mountainHeightLevel = 1.0f; // tallest

    // Generation of 2D textures
    private Texture2D GeneratedColourTexture;
    private Texture2D GeneratedGrayscaleTexture;

    // Terrain 
    private Terrain WorldTerrain;
    private NavMeshSurface navMesh;
    public List<Vector3> spawnLocations = new();
    public float[,] generatedTerrainHeightsValues;
    public float[,] generatedZombieSpawnHeightsValues;


    private void Start()
    {
        gameManager = GameManager.instance;
        navMesh = GetComponent<NavMeshSurface>();

        WorldTerrain = GetComponent<Terrain>(); // Assings the required components
    }

    public Terrain GetTerrainObject()
    {
        return WorldTerrain;
    }

    public void BakeNavigation()
    {
        navMesh.BuildNavMesh();
    }

    public void BeginGenerateTerrainAndSpawns()
    {
        CheckRandomSeed();
        PrepareTexture();
        generatedTerrainHeightsValues = GenerateNoiseMap();
        generatedZombieSpawnHeightsValues = GenerateNoiseMap();
        GenerateTerrain(generatedTerrainHeightsValues);
        BeginGenerateSpawnPoints(generatedTerrainHeightsValues, generatedZombieSpawnHeightsValues); // Generates random spawn points
    }

    private void BeginGenerateSpawnPoints(float[,] TerrainHeights, float[,] SpawnHeights)
    {
        if (TerrainHeights is null)
        {
            throw new ArgumentNullException(nameof(TerrainHeights));
        }

        if (SpawnHeights is null)
        {
            throw new ArgumentNullException(nameof(SpawnHeights));
        }

        //spawnLocations = new float[xTexSize, yTexSize];
        float spawnAfterThreshold = 0.7f;
        int spawnChance = 30; // % chance of spawning

        for (int y = 0; y < yTexSize; y++)
        {
            for (int x = 0; x < xTexSize; x++)
            {
                if (spawnAfterThreshold >= SpawnHeights[y,x])
                {
                    var spawnRandomValue = UnityEngine.Random.Range(0, 100);

                    if (spawnRandomValue < spawnChance)
                    {
                        var currentHeight = WorldTerrain.SampleHeight(new Vector3(x, 0, y));
                        spawnLocations.Add(new Vector3(x, currentHeight, y));
                    }
                }
            }
        }
    }

    private void CheckRandomSeed()
    {
        if (randomSeed)
        {
            // Randomly offsets texture
            xTexOffset = UnityEngine.Random.Range(0, xTexSize);
            yTexOffset = UnityEngine.Random.Range(0, yTexSize);
        }
    }

    private void PrepareTexture()
    {
        // Checks if automatic size is enabled, then sets it to terrain data
        if (automaticSize)
        {
            xTexSize = WorldTerrain.terrainData.heightmapResolution;
            yTexSize = WorldTerrain.terrainData.heightmapResolution;
        }

        // Creates new 2D texture that will be used to hold generated noise data
        GeneratedColourTexture = new Texture2D(xTexSize, yTexSize);
        GeneratedGrayscaleTexture = new Texture2D(xTexSize, yTexSize);
    }

    private void GenerateTerrain(float[,] generatedHeights)
    {
        if (generatedHeights is null)
        {
            throw new ArgumentNullException(nameof(generatedHeights));
        }

        WorldTerrain.terrainData.SetHeights(0, 0, generatedHeights);

        // Texturing
        for (int y=0; y < yTexSize; y++)
        {
            for (int x=0; x < xTexSize; x++)
            {
                var chosenColour = SelectColour(generatedHeights[y,x]);
                GeneratedColourTexture.SetPixel(x, y, chosenColour);
            }
        }
        // Colour texture
        GeneratedColourTexture.Apply();
        TerrainLayer terrainColourLayer = new()
        {
            diffuseTexture = GeneratedColourTexture,
            tileSize = new Vector2(yTexSize, yTexSize)
        };

        // Grayscale texture - TESTING 
        GeneratedGrayscaleTexture.Apply();
        TerrainLayer terrainGrayscaleLayer = new()
        {
            diffuseTexture = GeneratedGrayscaleTexture,
            tileSize = new Vector2(yTexSize, yTexSize)
        };

        WorldTerrain.terrainData.terrainLayers = new TerrainLayer[] { terrainColourLayer, terrainGrayscaleLayer  };
    }

    private Color SelectColour(float sampleValue)
    {
        Color color = new();
        if (sampleValue < waterHeightLevel * heightAdjustment)
        {
            // Water
            color = WaterColour;
        } 
        else if (sampleValue < sandHeightLevel * heightAdjustment)
        {
            // Sand
            color = SandColour;
        }
        else if (sampleValue < grassHeightLevel * heightAdjustment)
        {
            // Grass
            color = GrassColour;
        } 
        else if (sampleValue < mountainHeightLevel * heightAdjustment)
        {
            // Mountain
            color = MountainColour;
        }

        return color;
    }

    private float[,] GenerateNoiseMap()
    {
        // Creates 
        float[,] generatedHeights = new float[xTexSize, yTexSize];

        for (float y = 0.0f; y < GeneratedColourTexture.height; y++)
        {
            for (float x = 0.0f; x < GeneratedColourTexture.width; x++)
            {
                float xCoord = xTexOffset + x / GeneratedColourTexture.width * mapDetail;
                float yCoord = yTexOffset + y / GeneratedColourTexture.height * mapDetail;
                float genValue = 0.0f; // Used to store generated value

                if (usePerlinNoise)
                {
                    genValue = Mathf.PerlinNoise(xCoord, yCoord); // Generates values between 0.0f and 1.0f but can go beyond
                }
                else if (useCNoise)
                {
                    genValue = noise.cnoise(new float2(xCoord, yCoord)); // Generates values between -1.0f and 1.0f
                    genValue = math.unlerp(-1, 1, genValue); // Ensures only positive values
                }
                else if (useSNoise)
                {
                    genValue = noise.snoise(new float2(xCoord, yCoord)); // Generates values between -1.0f and 1.0f
                    genValue = math.unlerp(-1, 1, genValue); // Ensures only positive values
                }


                genValue = Mathf.Clamp(genValue, 0.0f, 1.0f); // Ensures no value goes below or above stated values
                generatedHeights[(int)x, (int)y] = genValue * heightAdjustment;
            }
        }
        // Sets terrain height and applies texture 
        return generatedHeights;
    }
}
