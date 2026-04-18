using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("News Display")] // which news to display?
    [SerializeField, Range(0, 100)] private int currentNewsIndex = 0;
    [SerializeField] private int totalNewsCount = 0;

    [Header("Terrain Sprites")]
    [SerializeField] private Sprite defaultSprite;

    [Header("Terrain Colors")]
    [SerializeField] private Color fieldColor = Color.green;
    [SerializeField] private Color forestColor = new Color(0f, 0.5f, 0f);
    [SerializeField] private Color mountainColor = Color.gray;
    [SerializeField] private Color waterColor = Color.gray;

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
    private List<News> globalNewsList = new List<News>();
    private bool isVisible = true;

    public Vector2 MapWorldSize => new Vector2(width * cellSize, height * cellSize);

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        GenerateGrid();
        DontDestroyOnLoad(gameObject);
        LoadMap();
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
        {
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



                node.bgVisual = bg;
                node.visual = go;
                grid[x, y] = node;
            }
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
        // => Key 'V' from the player
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 screenPos = new Vector3(
        //        Input.mousePosition.x,
        //        Input.mousePosition.y,
        //        Camera.main.transform.position.y
        //    );
        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        //    worldPos.y = 0f;
        //    Vector2Int gridPos = WorldToGrid(worldPos);
        //    SetTerrain(gridPos.x, gridPos.y, selectedTerrain);
        //    UpdateCostLabels();
        //}

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

        // TEST for visibility
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleVisibility();
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket)) // [
        {
            currentNewsIndex = Mathf.Max(0, currentNewsIndex - 1);
            RefreshNewsIcons();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket)) // ]
        {
            currentNewsIndex = Mathf.Min(globalNewsList.Count - 1, currentNewsIndex + 1);
            RefreshNewsIcons();
        }

        // SAVE to JSON
        if (Input.GetKeyDown(KeyCode.C))
            SaveMap();
    }

    void OnTurnAdvanced()
    {
        foreach (var news in globalNewsList)
        {
            List<GridNode> newlyReached = news.Spread(currentTurn, grid, width, height);
            foreach (var node in newlyReached)
                node.newsIDs.Add(news.GetID());
        }

        RefreshNewsIcons(); // do this every turn
    }

    

    void TryPlantGivenNews(int x, int y, News news)
    {
        if (!IsInBounds(x, y)) return;
        GridNode node = grid[x, y];

        if (node.spreadCost == TerrainConstants.BLOCKED)
        {
            return;
        }

        news.Plant(node);
        globalNewsList.Add(news);
        //SpawnNewsIcon(node, news.GetColor());
        node.newsIDs.Add(news.GetID());

        RefreshNewsIcons();
    }

    void TryPlantNews(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        GridNode node = grid[x, y];

        if (node.spreadCost == TerrainConstants.BLOCKED)
        {
            return;
        }

        News news = new News();
        news.Plant(node);
        globalNewsList.Add(news);
        //SpawnNewsIcon(node, news.GetColor());
        node.newsIDs.Add(news.GetID());

        currentNewsIndex = globalNewsList.Count - 1;
        RefreshNewsIcons();
    }

    // Called from the player
    public void EditMap(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        SetTerrain(x, y, selectedTerrain);
    }

    public void PlantGivenNewsAtWorldPosition(Vector3 worldPos, News news)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        TryPlantGivenNews(x, y, news);
    }

    public void PlantNewsAtWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        TryPlantNews(x, y);
    }

    //void SpawnNewsIcon(GridNode node, Color color)
    //{
    //    node.newsColors.Add(color);

    //    // DIVIDE NODE INTO 3*3 SO CAN SPAWN 9 ICONS MAX
    //    int index = node.newsColors.Count - 1;
    //    int col = index % 3;
    //    int row = index / 3;

    //    float subSize = cellSize / 3f * 0.6f;
    //    float spacing = cellSize / 3f;
    //    float offsetX = (col - 1) * spacing;
    //    float offsetZ = (row - 1) * spacing;

    //    GameObject icon = new GameObject($"News_{node.x}_{node.y}");
    //    icon.transform.position = node.worldPos + new Vector3(offsetX, 0.5f, offsetZ);
    //    icon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    //    icon.transform.localScale = Vector3.one * (subSize / defaultSprite.bounds.size.x);

    //    SpriteRenderer sr = icon.AddComponent<SpriteRenderer>();
    //    sr.sprite = newsSprite != null ? newsSprite : defaultSprite;
    //    sr.color = color;
    //    sr.sortingOrder = 3;

    //    newsIcons.Add(icon);
    //}

    void RefreshNewsIcons()
    {
        foreach (var icon in newsIcons)
            Destroy(icon);
        newsIcons.Clear();

        totalNewsCount = globalNewsList.Count;
        if (globalNewsList.Count == 0) return;

        currentNewsIndex = Mathf.Clamp(currentNewsIndex, 0, globalNewsList.Count - 1);
        News targetNews = globalNewsList[currentNewsIndex];
        Color color = targetNews.GetID();

        //Debug.Log($"[Refresh] index={currentNewsIndex}, color={color}, newsList={newsList.Count}");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = grid[x, y];


                if (!node.newsIDs.Contains(color)) continue;

                GameObject icon = new GameObject($"NewsIcon_{x}_{y}");
                icon.transform.position = node.worldPos + new Vector3(0f, 0.5f, 0f);
                icon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                icon.transform.localScale = Vector3.one * (cellSize * 0.5f / defaultSprite.bounds.size.x);

                SpriteRenderer sr = icon.AddComponent<SpriteRenderer>();
                sr.sprite = newsSprite != null ? newsSprite : defaultSprite;
                sr.color = color;
                sr.sortingOrder = 1000;

                newsIcons.Add(icon);
            }
        }

        // ToggleVisibility() +
        foreach (var icon in newsIcons)
            icon.GetComponent<SpriteRenderer>().enabled = isVisible;
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
            TerrainType.WATER => waterColor,
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

    public List<News> GetNewsAtNode(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;

        // List of every piece of news that has passed this node
        List<News> result = new List<News>();
        GridNode node = grid[x, y];

        // For each piece of news ID
        foreach (Color color in node.newsIDs)
            foreach (News news in globalNewsList)
                if (news.GetID() == color)
                    result.Add(news);

        return result;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    bool IsInBounds(int x, int y) =>
        x >= 0 && x < width && y >= 0 && y < height;


    public float GetNormalizedDistance(GridNode a, GridNode b)
    {
        float maxDistance = Vector3.Distance(
            grid[0, 0].worldPos,
            grid[width - 1, height - 1].worldPos
        );
        return Vector3.Distance(a.worldPos, b.worldPos) / maxDistance;
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;

        renderVisiblity();
    }

    public void HideGrid()
    {
        isVisible = false;
        renderVisiblity();
    }

    public void ShowGrid()
    {
        isVisible = true;
        renderVisiblity();
    }

    private void renderVisiblity()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                grid[x, y].visual.GetComponent<SpriteRenderer>().enabled = isVisible;
                grid[x, y].bgVisual.GetComponent<SpriteRenderer>().enabled = isVisible;
            }

        foreach (var icon in newsIcons)
            icon.GetComponent<SpriteRenderer>().enabled = isVisible;
    }

    //////////////////////////////////////////////////////////////
    // Save as JSON
    private string SavePath()
    {
        string dir = Path.Combine(Application.dataPath, "Data");
        Directory.CreateDirectory(dir); // if theres no /Data

        return Path.Combine(dir, $"map_{gameObject.name}.json");
    }

    public void SaveMap()
    {
        MapSaveData data = new MapSaveData();
        data.width = width;
        data.height = height;
        data.terrainData = new int[width * height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                data.terrainData[x * height + y] = (int)grid[x, y].terrain;

        File.WriteAllText(SavePath(), JsonUtility.ToJson(data));
        Debug.Log($"Map saved: {SavePath()}");
    }

    public void LoadMap()
    {
        if (!File.Exists(SavePath()))
        {
            Debug.Log("No save file found, using default map.");
            return;
        }

        MapSaveData data = JsonUtility.FromJson<MapSaveData>(File.ReadAllText(SavePath()));

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                SetTerrain(x, y, (TerrainType)data.terrainData[x * height + y]);

        Debug.Log($"Map loaded: {SavePath()}");
    }
}