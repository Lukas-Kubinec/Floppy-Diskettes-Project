using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    // Uses Wave-function collapse to generate obstacles (rock, tree, cactus, etc.)

    static readonly int[] tiles = { 0, 1, 2 };

    // Adjacency Rules: Key can be next to Value
    private static readonly Dictionary<int, int[]> rules = new()
    {
        { 0, new[] { 0, 1 } }, // Empty can be next to Empty or Path
        { 1, new[] { 0, 1 } }  // Path can be next to Path or Empty (simple rules)
    };

    public GameObject meshPart1;
    public GameObject meshPart2;
    public GameObject meshPart3;

    private static readonly int size = 3;
    private static readonly List<int>[,,] grid = new List<int>[size, size, size];
    static readonly System.Random rand = new();

    private void Awake()
    {
        StartWaveFunction();
    }

    void StartWaveFunction()
    {
        CreateGrid();
        while (CollapseCell()) { }
        BuildModel();
    }

    void BuildModel()
    {
        var pos = this.gameObject.transform.position;

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (grid[z,y,x][0] != 0)
                    {
                        var newObs = Instantiate(meshPart1, pos, Quaternion.identity);
                        newObs.transform.SetParent(this.transform);
                        var goPos = new Vector3(x, y, z);
                        newObs.transform.localPosition = goPos;
                    }
                }
            }
        }
    }

    static void CreateGrid()
    {
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                for (int z  = 0; z < size; z++)
                    grid[z, y, x] = new List<int>(tiles);
    }

    static bool CollapseCell()
    {
        // 1. Find cell with lowest entropy (>1 option)
        int minEntropy = int.MaxValue;
        int targetX = -1, targetY = -1; int targetZ = -1;

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int entropy = grid[z, y, x].Count;
                    if (entropy > 1 && entropy < minEntropy)
                    {
                        minEntropy = entropy;
                        targetX = x; targetY = y; targetZ = z;
                    }
                }
            }
        }


        if (targetX == -1) 
        {
            Debug.Log("Done!");
            return false;
        }

        // 2. Collapse Cell
        int chosenTile = grid[targetZ, targetY, targetX][rand.Next(grid[targetZ, targetY, targetX].Count)];
        grid[targetZ, targetY, targetX] = new List<int> { chosenTile };
        Debug.Log(chosenTile.ToString());

        // 3. Propagate (Simple adjacency)
        Propagate(targetX, targetY, targetZ);
        return true;
    }

    static void Propagate(int x, int y, int z)
    {
        int[] dx = {0, 0, 1, -1 , 0, 0};
        int[] dy = {1, -1, 0, 0 , 0, 0};
        int[] dz = { 0, 0, 0, 0, 1, -1 };

        // Checking the top part
        for (int _z = 0; _z < 3; _z++)
        {
            for (int _x = 0; _x < 3; _x++)
            {
                // Ensure a cell is underneath
                if (grid[_z, 2, _x][0] != 0)
                {
                    if (grid[_z, 1, _x][0] != 0)
                    {
                        // There is a block supporting the one on top
                        //Debug.Log("Yes");
                    } else
                    {
                        // There is no block supporting the one on top
                        //Debug.Log("No support");
                        //break;
                    }
                }
            }
        }

        for (int i = 0; i < 6; i++)
        {
            /*
             * Side view - from the starting side
             * - TOP - TOP - TOP - TOP - TOP -
             * X0/Y2/Z0 - X1/Y2/Z0 - X2/Y2/Z0
             * X0/Y1/Z0 - X1/Y1/Z0 - X2/Y1/Z0
             * X0/Y0/Z0 - X1/Y0/Z0 - X2/Y0/Z0
             *  - BOTTOM - BOTTOM - BOTTOM - 
             *  
             * Side view - from ending side
             * - TOP - TOP - TOP - TOP - TOP -
             * X0/Y2/Z2 - X1/Y2/Z2 - X2/Y2/Z2
             * X0/Y1/Z2 - X1/Y1/Z2 - X2/Y1/Z2
             * X0/Y0/Z2 - X1/Y0/Z2 - X2/Y0/Z2
             *  - BOTTOM - BOTTOM - BOTTOM - 
             *  
             *  Bottom view
             *  - Bottom -
             * X0/Y0/Z0 - X1/Y0/Z0 - X2/Y0/Z0
             * X0/Y0/Z1 - X1/Y0/Z1 - X2/Y0/Z1
             * X0/Y0/Z2 - X1/Y0/Z2 - X2/Y0/Z2
             * 
             *  Top view
             *  - Top -
             * X0/Y2/Z0 - X1/Y2/Z0 - X2/Y2/Z0
             * X0/Y2/Z1 - X1/Y2/Z1 - X2/Y2/Z1
             * X0/Y2/Z2 - X1/Y2/Z2 - X2/Y2/Z2
             */
            int nx = x + dx[i], ny = y + dy[i], nz = z + dz[i];

            if (nx >= 0 && nx < size && ny >= 0 && ny < size && nz >= 0 && nz < size && grid[nz, ny, nx].Count > 1)
            {
                // All cubes within
            }
        }
    }
}
