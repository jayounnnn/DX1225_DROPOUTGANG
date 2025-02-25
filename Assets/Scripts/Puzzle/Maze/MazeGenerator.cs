using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    [SerializeField] private int startX = 0;
    [SerializeField] private int startZ = 0; 

    private Cell[,] grid;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject hiddenWallPrefab;
    public GameObject hiddenWallPrefab2;
    public GameObject endObjectPrefab;

    [Range(0f, 1f)]
    public float hiddenWallChance = 0.1f;

    public float cellSize = 1f;
    public float wallHeight = 1f;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = new Cell();
            }
        }

        MazeDepthFirstSearch();

        RemoveEntranceWall();

        DrawMaze();

        PlaceEndObject();
    }

    private void PlaceEndObject()
    {
        int endX = width - 1;
        int endZ = height - 1;
        Vector3 endPosition = transform.position + new Vector3(endX * cellSize, 0f, endZ * cellSize);
        endPosition.y = this.transform.position.y;

        Instantiate(endObjectPrefab, endPosition, Quaternion.identity);
    }

    private void RemoveEntranceWall()
    {

        if (startZ == 0)
        {
            grid[startX, startZ].wallBottom = false;
        }
        else if (startZ == height - 1)
        {
            grid[startX, startZ].wallTop = false;
        }

        if (startX == 0)
        {
            grid[startX, startZ].wallLeft = false;
        }
        else if (startX == width - 1)
        {
            grid[startX, startZ].wallRight = false;
        }
    }

    void MazeDepthFirstSearch()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(startX, startZ);
        grid[current.x, current.y].visited = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();

            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);

                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];

                RemoveWalls(current, chosen);

                grid[chosen.x, chosen.y].visited = true;
                stack.Push(chosen);
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.y + 1 < height && !grid[cell.x, cell.y + 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        if (cell.y - 1 >= 0 && !grid[cell.x, cell.y - 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.x + 1 < width && !grid[cell.x + 1, cell.y].visited)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.x - 1 >= 0 && !grid[cell.x - 1, cell.y].visited)
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));

        return neighbors;
    }

    void RemoveWalls(Vector2Int a, Vector2Int b)
    {
        if (b.x == a.x && b.y == a.y + 1)
        {
            grid[a.x, a.y].wallTop = false;
            grid[b.x, b.y].wallBottom = false;
        }
        else if (b.x == a.x && b.y == a.y - 1)
        {
            grid[a.x, a.y].wallBottom = false;
            grid[b.x, b.y].wallTop = false;
        }
        else if (b.x == a.x + 1 && b.y == a.y)
        {
            grid[a.x, a.y].wallRight = false;
            grid[b.x, b.y].wallLeft = false;
        }
        else if (b.x == a.x - 1 && b.y == a.y)
        {
            grid[a.x, a.y].wallLeft = false;
            grid[b.x, b.y].wallRight = false;
        }
    }

    void DrawMaze()
    {
        GameObject mazeParent = new GameObject("Maze");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Cell cell = grid[x, z];
                Vector3 cellPos = transform.position + new Vector3(x * cellSize, 0, z * cellSize);

                if (cell.wallTop)
                {
                    Vector3 wallPos = cellPos + new Vector3(0, 0.5f, cellSize / 2);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    wall.transform.parent = mazeParent.transform;
                }

                if (cell.wallBottom)
                {
                    Vector3 wallPos = cellPos + new Vector3(0, 0.5f, -cellSize / 2);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    wall.transform.parent = mazeParent.transform;
                }

                if (cell.wallLeft)
                {
                    Vector3 wallPos = cellPos + new Vector3(-cellSize / 2, 0.5f, 0);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0));
                    wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    wall.transform.parent = mazeParent.transform;
                }

                if (cell.wallRight)
                {
                    Vector3 wallPos = cellPos + new Vector3(cellSize / 2, 0.5f, 0);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0));
                    wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    wall.transform.parent = mazeParent.transform;
                }

                if (x == startX && z == startZ)
                    continue;

                // TOP 
                if (!cell.wallTop && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(0, 0.5f, cellSize / 2f);
                    GameObject hiddenWallPrefabToUse = (Random.value < 0.5f) ? hiddenWallPrefab : hiddenWallPrefab2;
                    GameObject hiddenWall = Instantiate(hiddenWallPrefabToUse, hiddenPos, Quaternion.identity);
                    hiddenWall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                // BOTTOM 
                if (!cell.wallBottom && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(0, 0.5f, -cellSize / 2f);
                    GameObject hiddenWallPrefabToUse = (Random.value < 0.5f) ? hiddenWallPrefab : hiddenWallPrefab2;
                    GameObject hiddenWall = Instantiate(hiddenWallPrefabToUse, hiddenPos, Quaternion.identity);
                    hiddenWall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                // LEFT
                if (!cell.wallLeft && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(-cellSize / 2f, 0.5f, 0);
                    GameObject hiddenWallPrefabToUse = (Random.value < 0.5f) ? hiddenWallPrefab : hiddenWallPrefab2;
                    GameObject hiddenWall = Instantiate(hiddenWallPrefabToUse, hiddenPos, Quaternion.Euler(0, 90, 0));
                    hiddenWall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                // RIGTH
                if (!cell.wallRight && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(cellSize / 2f, 0.5f, 0);
                    GameObject hiddenWallPrefabToUse = (Random.value < 0.5f) ? hiddenWallPrefab : hiddenWallPrefab2;
                    GameObject hiddenWall = Instantiate(hiddenWallPrefabToUse, hiddenPos, Quaternion.Euler(0, 90, 0));
                    hiddenWall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }
            }
        }
    }
}
