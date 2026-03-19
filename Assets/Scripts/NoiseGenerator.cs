using Unity.Mathematics;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    // Uses Perlin, Cnoise and Snoise(Simplex) noise generators
    public bool usePerlin;
    public bool useCNoise;
    public bool useSNoise;

    // Size of the generated map
    [Header("Size of generated texture")]
    public int xTexSize = 512;
    public int yTexSize = 512;

    // Origin of the generated world
    public float xTexOffset;
    public float yTexOffset;

    // Map Detail setting
    [Header("Map detail setting")]
    public float mapDetail = 1.0f;
    public float heightAdjustment = 1.0f;

    // Colours 
    public Color GrassColour;
    public Color SandColour;
    public Color WaterColour;
    public Color MountainColour;

    // Generation of 2D textures
    private Texture2D generatedTexture;
    private Color[] colourOfPosition;
    private Renderer noiseRenderer;

    // Terrain
    private Terrain mapTerrain;

    private void Start()
    {
        // Assings the required components
        noiseRenderer = GetComponent<Renderer>();
        mapTerrain = GetComponent<Terrain>();

        PrepareTexture();

        if (usePerlin)
        {
            GeneratePerlinNoiseMap();
        }
        else if (useCNoise)
        {
            GenerateCNoiseMap();
        }
        else if (useSNoise)
        {
             GenerateSNoiseMap();
        } else
        {
            Debug.LogWarning("Choose noise");
        }
    }

    private void Update()
    {

    }

    private void PrepareTexture()
    {
        // Creates new 2D texture that will be used to hold generated noise data
        generatedTexture = new Texture2D(xTexSize, yTexSize);

        // Holds all possible positions of texture inside a field
        colourOfPosition = new Color[generatedTexture.width * generatedTexture.height];
        noiseRenderer.material.mainTexture = generatedTexture;
    }

    private void SetTerrainHeight(float[,] generatedHeights)
    {
        mapTerrain.terrainData.SetHeights(0, 0, generatedHeights);
    }

    private Color SelectColour(float sampleValue)
    {
        Color color = new Color();
        if (sampleValue < 0.2f)
        {
            // Water
            color = WaterColour;
        } 
        else if (sampleValue < 0.4f)
        {
            // Sand
            color = SandColour;
        }
        else if (sampleValue < 0.8f)
        {
            // Ground
            color = GrassColour;
        } 
        else
        {
            // Mountain
            color = MountainColour;
        }

        return color;
    }

    private void GenerateCNoiseMap()
    {
        // Creates 
        float[,] generatedHeights = new float[xTexSize, yTexSize];

        for (float y = 0.0f; y < generatedTexture.height; y++)
        {
            for (float x = 0.0f; x < generatedTexture.width; x++)
            {
                float xCoord = xTexOffset + x / generatedTexture.width * mapDetail;
                float yCoord = yTexOffset + y / generatedTexture.height * mapDetail;

                float perlinNoiseValue = noise.cnoise(new float2(xCoord, yCoord));
                math.unlerp(-1,1, perlinNoiseValue);
                generatedHeights[(int)x, (int)y] = perlinNoiseValue * heightAdjustment;

                // Generates colour map
                Color selectedColor = SelectColour(perlinNoiseValue);
                colourOfPosition[(int)y * generatedTexture.width + (int)x] = selectedColor;
            }
        }

        SetTerrainHeight(generatedHeights);
    }

    private void GeneratePerlinNoiseMap()
    {
        // Creates 
        float[,] generatedHeights = new float[xTexSize, yTexSize];

        for (float y = 0.0f; y < generatedTexture.height; y++)
        {
            for (float x = 0.0f; x < generatedTexture.width; x++)
            {
                float xCoord = xTexOffset + x / generatedTexture.width * mapDetail;
                float yCoord = yTexOffset + y / generatedTexture.height * mapDetail;

                float perlinNoiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                math.unlerp(-1, 1, perlinNoiseValue);
                Mathf.Clamp(perlinNoiseValue, 0.0f, 1.0f);
                generatedHeights[(int)x, (int)y] = perlinNoiseValue * heightAdjustment;

                // Generates colour map
                Color selectedColor = SelectColour(perlinNoiseValue);
                colourOfPosition[(int)y * generatedTexture.width + (int)x] = selectedColor;
            }
        }

        SetTerrainHeight(generatedHeights);
    }

    private void GenerateSNoiseMap()
    {
        // Creates 
        float[,] generatedHeights = new float[xTexSize, yTexSize];

        for (float y = 0.0f; y < generatedTexture.height; y++)
        {
            for (float x = 0.0f; x < generatedTexture.width; x++)
            {
                float xCoord = xTexOffset + x / generatedTexture.width * mapDetail;
                float yCoord = yTexOffset + y / generatedTexture.height * mapDetail;

                //float perlinNoiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                float perlinNoiseValue = noise.snoise(new float2(xCoord, yCoord));
                math.unlerp(-1, 1, perlinNoiseValue);
                Mathf.Clamp(perlinNoiseValue, 0.0f, 1.0f);
                generatedHeights[(int)x, (int)y] = perlinNoiseValue * heightAdjustment;

                // Generates colour map
                Color selectedColor = SelectColour(perlinNoiseValue);
                colourOfPosition[(int)y * generatedTexture.width + (int)x] = selectedColor;
            }
        }

        SetTerrainHeight(generatedHeights);
    }

}
