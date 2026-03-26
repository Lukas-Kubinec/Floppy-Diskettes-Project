using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ProceduralObstacleGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    public int sizeX = 3;
    public int sizeY = 3;
    public int sizeZ = 3;

    // 3x3 Grid - X Y(up) Z
    private float[,,] genGrid;
    private int cellsCollapsed = 0;

    [Header("Settings")]
    public int collapseProbability = 75; // %chance of collapse
    public bool cellMustBeSupported = false; // Cell supported from underneath (rock pile)
    public bool sideCellsMustBeRotatedTowardCentre = false; // Cells rotated toward centre (branches)
    public bool centreMustBeOneType = false; // Core consists of core mesh parts
    public bool enableRandomRotation = false; // Cells can be rotated at initiation

    // Meshes for obstacles
    [Header("Mesh parts")]
    public GameObject[] meshParts;

    private void Awake()
    {
        CreateNewGrid();
        while (ChooseEmptyCell());
        BuildModel();
    }

    void CreateNewGrid()
    {
        genGrid = new float[sizeX, sizeY, sizeZ];
    }

    bool ChooseEmptyCell()
    {
        int chosenX = Random.Range(0, sizeX);
        int chosenY = Random.Range(0, sizeY);
        int chosenZ = Random.Range(0, sizeZ);

        if (cellsCollapsed == genGrid.Length)
        {
            // Finished collapsing
            return false;
        }

        // Checks if selected cell is empty
        if (genGrid[chosenX, chosenY, chosenZ] == 0 )
        {
            // Collapses cell
            CheckCellRules(chosenX, chosenY, chosenZ);
            return true;
        }
        else
        {
            // Must repeat
            return true;
        }
    }

    int GetRandomIntWithProbability(int probability, int returnTrue, int returnFalse)
    {
        // Probability must be between 1 and 99 %
        int randomInt = Random.Range(1, 99);

        if (randomInt < probability)
        {
            return returnTrue;
        } else
        {
            return returnFalse;
        }
    }

    void CheckCellRules(int x,int y,int z)
    {
        // Cell types - 0 Not yet collapsed / 1 Empty cell / 2 Filled cell / 
        // Used for Rocks generation
        if (cellMustBeSupported)
        {
            // Checks if cell is supported from underneath
            if (y > 0)
            {
                // Checks if cell under is empty/not-collapsed
                if (genGrid[x, y - 1, z] < 2)
                {
                    // Cell would not be supported from underneath
                    genGrid[x, y, z] = 1; // Gives it empty collapsed cell
                    cellsCollapsed++;
                }
                else
                {
                    // Cell can be supported from underneath
                    int randomTile = GetRandomIntWithProbability(collapseProbability, 2, 1);
                    genGrid[x, y, z] = randomTile;
                    cellsCollapsed++;
                }

            }
            else
            {
                // Cell is placed at bottom
                int randomTile = GetRandomIntWithProbability(collapseProbability, 2, 1);
                genGrid[x, y, z] = randomTile;
                cellsCollapsed++;
            }
        }

        // Trees or Cactus
        if (sideCellsMustBeRotatedTowardCentre && centreMustBeOneType)
        {
            // Builds the core
            if (x == 1 && z == 1)
            {
                // Cactus is always at least 2 parts tall
                if (y == 0 || y == 1)
                {
                    genGrid[x, y, z] = 2;
                    cellsCollapsed++;
                } else if (y == 2)
                {
                    // Checks if the cell below already contains fill part
                    if (genGrid[x, y-1, z] == 2)
                    {
                        // Randomly chooses cell based on probability
                        int randomTile = GetRandomIntWithProbability(collapseProbability, 2, 1);
                        genGrid[x, y, z] = randomTile;
                        cellsCollapsed++;
                    }
                    else
                    {
                        // If the cell below doesnt contain fill part, this cell stays empty
                        genGrid[x, y, z] = 1;
                        cellsCollapsed++;
                    }
                }
            }
            else
            {
                // Ensures that the core is filled in
                 if (genGrid[1, y, 1] == 2)
                {
                    // Randomly chooses cell based on probability
                    int randomTile = GetRandomIntWithProbability(collapseProbability, 2, 1);
                    genGrid[x, y, z] = randomTile;
                    cellsCollapsed++;
                } else
                {
                    // If the core is not yet collapsed, this cell stays empty
                    genGrid[x, y, z] = 1;
                    cellsCollapsed++;
                }
            }
        }

    }

    void BuildModel()
    {
        //var ObstaclesParent = new GameObject("ObstaclesParent");
        var pos = this.gameObject.transform.position;

        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    // Rocks
                    if (cellMustBeSupported)
                    {
                        if (genGrid[x, y, z] > 1)
                        {
                            // Piles rocks from largest to smallest - Must be set in Inspector in this order!
                            InstatiatePrefab(x, y, z, pos, meshParts[y], 0.0f);
                        }
                    }

                    // Builds the core parts of Cactus or Trees (0 - bottom core / 1 - top core / 2 - side)
                    if (centreMustBeOneType)
                    {
                        if (x == 1 && z == 1)
                        {
                            // Builds the bottom core part
                            if (y == 0)
                            {
                                InstatiatePrefab(x, y, z, pos, meshParts[0], 0.0f);
                            } else if (y == 1)
                            {
                                // Checks if there is a fill cell above
                                if (genGrid[x, y+1, z] > 1)
                                {
                                    // If there is a fill cell above, this cell gets bottom core part
                                    InstatiatePrefab(x, y, z, pos, meshParts[0], 0.0f);
                                } else
                                {
                                    // If cell above is empty, this cell gets the top core part
                                    InstatiatePrefab(x, y, z, pos, meshParts[1], 0.0f);
                                }
                            } else if (y == 2 && genGrid[x, y, z] > 1)
                            {
                                // Gets the top core part
                                InstatiatePrefab(x, y, z, pos, meshParts[1], 0.0f);
                            }
                        }
                    }

                    // Builds sides/branches around core
                    if (sideCellsMustBeRotatedTowardCentre && genGrid[x, y, z] > 1)
                    {
                        if (x == 0 && z == 0)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 135.0f);
                        } else if (x == 1 && z == 0)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 90.0f);
                        }
                        else if (x == 2 && z == 0)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 45.0f);
                        }
                        else if (x == 0 && z == 1)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 180.0f);
                        }
                        else if (x == 2 && z == 1)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 0.0f);
                        }
                        else if (x == 0 && z == 2)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 225.0f);
                        }
                        else if (x == 1 && z == 2)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 270.0f);
                        }
                        else if (x == 2 && z == 2)
                        {
                            InstatiatePrefab(x, y, z, pos, meshParts[2], 315.0f);
                        }
                    }
                }
            }
        }
    }

    void InstatiatePrefab(int x, int y, int z, Vector3 originPosition, GameObject meshPart, float rotateBy)
    {
        GameObject newObs;
        if (enableRandomRotation)
        {
            // Rotates randomly (rock piles)
            float rot = Random.Range(0, 360); // Random rotation
            var initialRotation = Quaternion.Euler(rot, rot, rot);
            newObs = Instantiate(meshPart, originPosition, initialRotation);
        } else if (sideCellsMustBeRotatedTowardCentre)
        {
            // Rotates by set amount (branches, etc.)
            var initialRotation = Quaternion.Euler(0, rotateBy, 0);
            newObs = Instantiate(meshPart, originPosition, initialRotation);
        } else
        {
            // No rotation to object
            newObs = Instantiate(meshPart, originPosition, Quaternion.identity);
        }
        newObs.transform.SetParent(this.transform);
        var goPos = new Vector3(x, y, z);
        newObs.transform.localPosition = goPos;
    }

}
