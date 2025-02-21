using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    [SerializeField] private int startX = 0;
    [SerializeField] private int startY = 0;

    private Cell[,] grid;

    public GameObject wallPrefab;  
    public GameObject floorPrefab;
    public GameObject hiddenWallPrefab;

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
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell();
            }
        }

        MazeDepthFirstSearch();

        RemoveEntranceWall();

        DrawMaze();
    }

    private void RemoveEntranceWall()
    {
        grid[startX, startY].wallBottom = false;
    }

    void MazeDepthFirstSearch()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(startX, startY);
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
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                Vector3 cellPos = new Vector3(x * cellSize, 0, y * cellSize);

                GameObject floor = Instantiate(floorPrefab, cellPos, Quaternion.identity);
                floor.transform.localScale = new Vector3(cellSize, 1, cellSize);
                floor.transform.parent = mazeParent.transform;

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

                //////////////////////////////////////////////////////////////////////

                if (x == startX && y == startY)
                {
                    continue;
                }


                if (!cell.wallTop && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(0, 0.5f, cellSize / 2f);
                    GameObject hiddenWall = Instantiate(hiddenWallPrefab, hiddenPos, Quaternion.identity);
                    hiddenWall.transform.localScale = new Vector3(cellSize, 1, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                

                if (!cell.wallBottom && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(0, 0.5f, -cellSize / 2f);
                    GameObject hiddenWall = Instantiate(hiddenWallPrefab, hiddenPos, Quaternion.identity);
                    hiddenWall.transform.localScale = new Vector3(cellSize, 1, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                if (!cell.wallLeft && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(-cellSize / 2f, 0.5f, 0);
                    GameObject hiddenWall = Instantiate(hiddenWallPrefab, hiddenPos, Quaternion.Euler(0, 90, 0));
                    hiddenWall.transform.localScale = new Vector3(cellSize, 1, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }

                if (!cell.wallRight && Random.value < hiddenWallChance)
                {
                    Vector3 hiddenPos = cellPos + new Vector3(cellSize / 2f, 0.5f, 0);
                    GameObject hiddenWall = Instantiate(hiddenWallPrefab, hiddenPos, Quaternion.Euler(0, 90, 0));
                    hiddenWall.transform.localScale = new Vector3(cellSize, 1, 0.1f);
                    hiddenWall.transform.parent = mazeParent.transform;
                }
            }
        }
    }
}
