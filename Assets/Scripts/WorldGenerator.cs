using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;

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
    public int xTexSize = 513; // Terrain settings must be adjusted !
    public int yTexSize = 513; // Terrain settings must be adjusted !
    public bool automaticTexSize = false; // Automatically sets texture size to terrain size
    [Header("Size of spawn texture")]
    public int xSpawnSize = 513; // Terrain settings must be adjusted !
    public int zSpawnSize = 513; // Terrain settings must be adjusted !
    public bool automaticSpawnSize = false; // Automatically sets texture size to terrain size
    public int spawnCreationProbability = 75;

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

    // Terrain 
    private Terrain WorldTerrain;
    private NavMeshSurface navMesh;
    public List<Vector3> spawnZombieLocations = new();
    public List<Vector3> spawnObstacleLocations = new();
    public float[,] generatedTerrainHeightsValues;
    public float[,] generatedZombieSpawnHeightsValues;
    public float[,] generatedObstacleSpawnHeightsValues;


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
        // Generates height values
        generatedTerrainHeightsValues = GenerateNoiseMap(xTexSize,yTexSize);
        generatedZombieSpawnHeightsValues = GenerateNoiseMap(xSpawnSize, zSpawnSize);
        generatedObstacleSpawnHeightsValues = GenerateNoiseMap(xSpawnSize, zSpawnSize);
        // Uses generated data for methods
        GenerateTerrain(generatedTerrainHeightsValues);
        spawnZombieLocations = BeginGenerateSpawnPoints(generatedTerrainHeightsValues, generatedZombieSpawnHeightsValues); // Generates random zombie spawn points
        spawnObstacleLocations = BeginGenerateSpawnPoints(generatedTerrainHeightsValues, generatedObstacleSpawnHeightsValues); // Generates random zombie spawn points
    }

    private List<Vector3> BeginGenerateSpawnPoints(float[,] TerrainHeights, float[,] SpawnHeights)
    {
        //spawnLocations = new float[xTexSize, yTexSize];
        float spawnAfterThreshold = 0.7f;
        List < Vector3 > newSpawnPoints = new List<Vector3>();

        for (int y = 0; y < zSpawnSize; y++)
        {
            for (int x = 0; x < xSpawnSize; x++)
            {
                if (spawnAfterThreshold >= SpawnHeights[y,x])
                {
                    var spawnRandomValue = UnityEngine.Random.Range(0, 100);

                    if (spawnRandomValue < spawnCreationProbability)
                    {
                        var currentHeight = WorldTerrain.SampleHeight(new Vector3(x, 0, y));
                        newSpawnPoints.Add(new Vector3(x, currentHeight, y));
                    }
                }
            }
        }

        // returns complete table
        return newSpawnPoints;
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
        // Automatic size for generated texture
        if (automaticTexSize)
        {
            xTexSize = WorldTerrain.terrainData.heightmapResolution;
            yTexSize = WorldTerrain.terrainData.heightmapResolution;
        }

        // Automatic size for generated spawns
        if (automaticSpawnSize)
        {
            xSpawnSize = (int)WorldTerrain.terrainData.size.x;
            zSpawnSize = (int)WorldTerrain.terrainData.size.x;
        }

        // Creates new 2D texture that will be used to hold generated noise data
        GeneratedColourTexture = new Texture2D(xTexSize, yTexSize);
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
        // Applies added colours
        GeneratedColourTexture.Apply();

        //Gets a terrain size
        var xWorldSize = WorldTerrain.terrainData.size.x;
        var zWorlsSize = WorldTerrain.terrainData.size.z;
        TerrainLayer terrainWaterLayer = new()
        {
            diffuseTexture = GeneratedColourTexture,
            tileSize = new Vector2(xWorldSize, zWorlsSize)
        };

        WorldTerrain.terrainData.terrainLayers = new TerrainLayer[] { terrainWaterLayer };
    }

    private Color SelectColour(float height)
    {
        if (height < waterHeightLevel * heightAdjustment)
        {
            // Water
            return WaterColour;
        } 
        else if (height < sandHeightLevel * heightAdjustment)
        {
            // Sand
            return SandColour;
        }
        else if (height < grassHeightLevel * heightAdjustment)
        {
            // Grass
            return GrassColour;
        } 
        else if (height < mountainHeightLevel * heightAdjustment)
        {
            // Mountain
            return MountainColour;
        }
        else
        {
            Debug.LogWarning("Error - Texture painting");
            return GrassColour;
        }
    }

    private float[,] GenerateNoiseMap(int xSize, int ySize)
    {
        // Creates 
        float[,] generatedHeights = new float[xSize, ySize];

        for (float y = 0.0f; y < ySize; y++)
        {
            for (float x = 0.0f; x < xSize; x++)
            {
                float xCoord = xTexOffset + x / xSize * mapDetail;
                float yCoord = yTexOffset + y / ySize * mapDetail;
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
