using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    // Uses Perlin Noise to generate noise maps using selected parametres

    // Size of the generated map
    [Header("Size of generated texture")]
    public int xMapSize;
    public int yMapSize;

    // Origin of the generated world
    public float xMapOrigin;
    public float yMapOrigin;

    // Map Detail setting
    [Header("Map detail setting")]
    public float mapDetail = 1.0f;

    // Colours 
    public Color GroundColour;

    private Texture2D generatedTexture;
    private Color[] pix;
    private Renderer noiseRenderer;

    private void Start()
    {
        // Assings the Renderer component
        try
        {
            noiseRenderer = GetComponent<Renderer>();
        } catch
        {
            Debug.LogWarning("Missing Renderer component!");
        }
        
        PrepareTexture();
        GenerateMap();
    }

    private void PrepareTexture()
    {
        // Creates new 2D texture that will be used to hold generated noise data
        generatedTexture = new Texture2D(xMapSize, yMapSize);
        pix = new Color[generatedTexture.width * generatedTexture.height];
        noiseRenderer.material.mainTexture = generatedTexture;
    }

    private Color SelectColour(float sampleValue)
    {
        Color color = new Color();
        if (sampleValue < 0.2f)
        {
            // Water
            color = new (0.071f, 0.42f, 0.718f);
        } else if (sampleValue < 0.7f)
        {
            // Ground
            color = GroundColour;
        } else
        {
            // Mountain
            color = new(0.341f, 0.31f, 0.235f);
        }

        return color;
    }

    private void GenerateMap()
    {
        for (float y = 0.0f; y < generatedTexture.height; y++)
        {
            for (float x = 0.0f; x < generatedTexture.width; x++)
            {
                float xCoord = xMapOrigin + x / generatedTexture.width * mapDetail;
                float yCoord = yMapOrigin + y / generatedTexture.height * mapDetail;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Mathf.Clamp(sample, 0.0f, 1.0f);
                Color selectedColor = SelectColour(sample);
                pix[(int)y * generatedTexture.width + (int)x] = selectedColor;
            }
        }

        generatedTexture.SetPixels(pix);
        generatedTexture.Apply();
    }

}
