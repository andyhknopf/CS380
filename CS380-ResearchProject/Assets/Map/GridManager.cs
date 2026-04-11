using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("Terrain Sprites")]
    [SerializeField] private Sprite defaultSprite;

    [Header("Terrain Colors")]
    [SerializeField] private Color fieldColor = Color.green;
    [SerializeField] private Color forestColor = new Color(0f, 0.5f, 0f);
    [SerializeField] private Color mountainColor = Color.gray;

    [Header("Terrain Data")]
    [SerializeField] private TerrainData[] terrainDataList;

    [Header("Edit Settings")]
    [SerializeField] private TerrainType selectedTerrain = TerrainType.FIELD;

    [Header("Debug")]
    [SerializeField] private bool showSpreadCost = false;
    private bool prevShowSpreadCost;

    [Header("Turn Settings")]
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private bool isTurnPaused = true;

    [Header("News Icon")]
    [SerializeField] private Sprite newsSprite;
    [SerializeField] private Color newsColor = Color.yellow;

    private GridNode[,] grid;
    private Vector3 origin;
    private List<GameObject> costLabels = new List<GameObject>();
    private List<GameObject> newsIcons = new List<GameObject>();
    private int currentTurn = 0;
    private float timer = 0f;
    private News news;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
      Camera cam = Camera.main;
      float screenHeight = cam.orthographicSize * 2f;
      float screenWidth = screenHeight * cam.aspect;

      // cellSize = Mathf.Min(screenWidth / width, screenHeight / height);

      float gridWidth = width * cellSize;
      float gridHeight = height * cellSize;

      // Bake the navmesh at runtime
      BakeNavMesh(gridWidth, gridHeight);

      origin = new Vector3(
          -gridWidth / 2f + cellSize / 2f,
          0f,
          -gridHeight / 2f + cellSize / 2f
      );

      grid = new GridNode[width, height];
      Vector2 spriteSize = defaultSprite.bounds.size;

      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
          GridNode node = new GridNode
          {
            x = x,
            y = y,
            terrain = TerrainType.FIELD,
            worldPos = origin + new Vector3(x * cellSize, 0f, y * cellSize),
            spreadCost = GetSpreadCost(TerrainType.FIELD),
            leftCount = GetSpreadCost(TerrainType.FIELD)
          };

          // background for borders
          GameObject bg = new GameObject($"BG_{x}_{y}");
          bg.transform.position = node.worldPos;
          bg.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
          bg.transform.SetParent(transform);
          bg.transform.localScale = Vector3.one * (cellSize / spriteSize.x);
          SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
          bgSr.sprite = defaultSprite;
          bgSr.color = Color.black;
          bgSr.sortingOrder = -5; // Render in back
                                  // BoxCollider collider = bg.AddComponent<BoxCollider>();

          GameObject go = new GameObject($"Node_{x}_{y}");
          go.transform.position = node.worldPos;
          go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
          go.transform.SetParent(transform);
          go.transform.localScale = Vector3.one * (cellSize / spriteSize.x * 0.9f);
          SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
          sr.sprite = defaultSprite;
          sr.color = GetTerrainColor(node.terrain);
          sr.sortingOrder = bgSr.sortingOrder + 1;




          node.visual = go;
          grid[x, y] = node;
        }
  }

  private static void BakeNavMesh(float gridWidth, float gridHeight)
  {
    NavMeshSurface navMesh = FindFirstObjectByType<NavMeshSurface>(); // Should be ParthCube
    if (navMesh != null)
    {
      navMesh.transform.localScale = new Vector3(gridWidth, 0.0f, gridHeight);
      navMesh.BuildNavMesh();
    }
  }

  void Update()
    {
        // DEBUG - cost label
        if (prevShowSpreadCost != showSpreadCost)
        {
            prevShowSpreadCost = showSpreadCost;
            UpdateCostLabels();
        }
        
        // TURN
        if (!isTurnPaused)
        {
            timer += Time.deltaTime;
            if (timer >= turnSpeed)
            {
                timer = 0f;
                currentTurn++;
                OnTurnAdvanced();
            }
        }

        // TERRAIN(MAP) EDIT
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.y
            );
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.y = 0f;
            Vector2Int gridPos = WorldToGrid(worldPos);
            SetTerrain(gridPos.x, gridPos.y, selectedTerrain);
            UpdateCostLabels();
        }

        // TEMP & DEBUGGING for news creatino
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 screenPos = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.y
            );
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.y = 0f;
            Vector2Int gridPos = WorldToGrid(worldPos);
            TryPlantNews(gridPos.x, gridPos.y);
        }
    }

    void OnTurnAdvanced()
    {
        if (news == null) return;
        List<GridNode> newlyReached = news.Spread(currentTurn, grid, width, height);
        foreach (var node in newlyReached)
            SpawnNewsIcon(node);
    }

    void TryPlantNews(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        GridNode node = grid[x, y];

        if (node.spreadCost == TerrainConstants.BLOCKED)
        {
            return;
        }

        news = new News();
        news.Plant(node);
        SpawnNewsIcon(node);
    }

    void SpawnNewsIcon(GridNode node)
    {
        GameObject icon = new GameObject($"News_{node.x}_{node.y}");
        icon.transform.position = node.worldPos + new Vector3(0, 0.1f, 0);
        icon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        icon.transform.localScale = Vector3.one * (cellSize / defaultSprite.bounds.size.x * 0.2f);

        SpriteRenderer sr = icon.AddComponent<SpriteRenderer>();
        sr.sprite = newsSprite != null ? newsSprite : defaultSprite;
        sr.color = newsColor;
        sr.sortingOrder = 3;

        newsIcons.Add(icon);
    }

    void UpdateCostLabels()
    {
        foreach (var label in costLabels)
            Destroy(label);
        costLabels.Clear();

        if (!showSpreadCost) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = grid[x, y];
                int cost = GetSpreadCost(node.terrain);

                GameObject labelObj = new GameObject($"Label_{x}_{y}");
                labelObj.transform.position = node.worldPos + new Vector3(0, 0.1f, 0f);
                labelObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                labelObj.transform.SetParent(transform);

                var tmp = labelObj.AddComponent<TextMeshPro>();
                tmp.text = cost == TerrainConstants.BLOCKED ? "X" : cost.ToString();
                tmp.fontSize = 3f;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.black;
                tmp.sortingOrder = 10;

                costLabels.Add(labelObj);
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), $"Turn: {currentTurn}");
    }

    public Color GetTerrainColor(TerrainType terrain)
    {
        return terrain switch
        {
            TerrainType.FIELD => fieldColor,
            TerrainType.FOREST => forestColor,
            TerrainType.MOUNTAIN => mountainColor,
            _ => Color.white
        };
    }

    public int GetSpreadCost(TerrainType terrain)
    {
        foreach (var data in terrainDataList)
            if (data.terrainType == terrain)
                return data.spreadCost;
        return 1;
    }

    public void SetTerrain(int x, int y, TerrainType terrain)
    {
        if (!IsInBounds(x, y)) return;
        grid[x, y].terrain = terrain;
        grid[x, y].spreadCost = GetSpreadCost(terrain);
        SetNodeColor(x, y, GetTerrainColor(terrain));
    }

    public void SetNodeColor(int x, int y, Color color)
    {
        if (!IsInBounds(x, y)) return;
        grid[x, y].visual.GetComponent<SpriteRenderer>().color = color;
    }

    public GridNode GetNode(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;
        return grid[x, y];
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    bool IsInBounds(int x, int y) =>
        x >= 0 && x < width && y >= 0 && y < height;

    public void PlantNewsAtWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        TryPlantNews(x, y);
    }
}