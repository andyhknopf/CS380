using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class City : MonoBehaviour
{
    public string citySceneName;
    public GridNode gridNode;

    public int dummyDataForCity;
    private GridManager gm;

    public List<News> cityNewsList = new List<News>();


    public int numNPCs = 3;
    public List<GameObject> NPCs;

    public Color newestNews;
    public int amtKnownMost;

    public NativeHashMap<Color, int> colorMap;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        gm = GameObject.Find("GridManager").GetComponent<GridManager>();
        Vector2Int gridPos = gm.WorldToGrid(transform.position);
        gridNode = gm.GetNode(gridPos.x, gridPos.y);
        cityNewsList.Add(null);

        //spawn npcs in map
        //assign npcs to 
        for(int i = 0; i < numNPCs; i++)
        {
            //GameObject npcPrefab = Resources.Load("NPCs/AndyNPC") as GameObject;
            GameObject npcPrefab = Instantiate(Resources.Load("Prefabs/NPCs/AndyNPC", typeof(GameObject))) as GameObject;
            Instantiate(npcPrefab, new Vector3(Random.Range(0, gm.MapWorldSize.x), 3f, Random.Range(0, gm.MapWorldSize.y)), Quaternion.identity);
            NPCs.Add(npcPrefab);
        }


    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(transform.position);
        //}
        if (amtKnownMost > NPCs.Count / 2)
        {
            GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(transform.position);
        }
        //if gridnode listener gets hit with news
        //StartSpeadingNews
        Vector2Int gridPos = gm.WorldToGrid(transform.position);
        List<News> compare = gm.GetNewsAtNode(gridPos.x, gridPos.y);
        //if (gm.GetNewsAtNode(gridPos.x, gridPos.y) != cityNewsList)
        if (compare.Count == 0) return;
        if (gm.GetNewsAtNode(gridPos.x, gridPos.y)[compare.Count-1] != cityNewsList[cityNewsList.Count -1])
        {
            cityNewsList = gm.GetNewsAtNode(gridPos.x, gridPos.y);
            StartSpeadingNews(cityNewsList[cityNewsList.Count - 1]);
        }


    }

    void StartSpeadingNews(News news)
    {
        //add news to a bunch of npcs in the list
        if (cityNewsList.Contains(news)) return;

        int numToKnow = Random.Range(1, NPCs.Count / 2);

        for (int i = 0; i < numToKnow; i++)
        {
            //assign new news
            NPCs[i].GetComponent<BasicNPC>().LearnNews(news);
        }

        colorMap.Add(news.GetColor(), numToKnow);

    }
}
