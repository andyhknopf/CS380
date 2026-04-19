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

    // List of news this city has already received
    [HideInInspector] public List<News> receivedNews = new List<News>();

    // List of news this city has already sent out
    [HideInInspector] public List<News> sentNews = new List<News>();

    public int numNPCs = 3;
    public List<GameObject> NPCs;

    // ID of the newest received piece of news
    public Color newestNews;

    // The amount of people that know the most known piece of news
    public int amtKnownMost;

    // Pass in an ID and get the amount of people that know this piece of news
    public NativeHashMap<Color, int> colorMap;

    GameObject spawnPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
      // Check the NPCs


      gm = GameObject.Find("GridManager").GetComponent<GridManager>();
        gm.SetVisibility(false);
      Vector2Int gridPos = gm.WorldToGrid(transform.position);
      gridNode = gm.GetNode(gridPos.x, gridPos.y);
      cityNewsList.Add(null);
    }

    // Update is called once per frame
    void Update()
    {
      //if (Input.GetKeyDown(KeyCode.L))
      //{
      //    GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(transform.position);
      //}


      // If more than of the citie's npcs know the citie's most popular piece of news
      if (amtKnownMost > NPCs.Count / 2)
      {
        //plant it on the world map at this city 
        //GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(transform.position);

        // TODO: Remove the piece of news from the cities news list
      }

      // Listen for new news
      ListenForNews();
    }

  // Returns true if new news was received, false if not
  private void ListenForNews()
  {
    Vector2Int gridPos = gm.WorldToGrid(transform.position);
    List<News> compare = gm.GetNewsAtNode(gridPos.x, gridPos.y);

    // Check for errors
    if (compare == null || compare.Count == 0) return;

    // Check if this piece of news has already been received by the city
    foreach (News alreadyKnownNews in receivedNews)
    {
      for (int i = 0; i < compare.Count - 1; ++i)
      {
        // Skip this piece of news if we already know it
        if (compare[i].GetID() == alreadyKnownNews.GetID())
          continue;

        receivedNews.Add(compare[i]);
        StartSpeadingNews(compare[i]);
        return;
      }
    }
  }

  void StartSpeadingNews(News news)
    {
        //add news to a bunch of npcs in the list
        if (cityNewsList.Contains(news)) return;

        int numToKnow = Random.Range(1, 2* (NPCs.Count / 3));

        for (int i = 0; i < numToKnow; i++)
        {
            //assign new news
            NPCs[i].GetComponent<BasicNPC>().LearnNews(news);
        }

        colorMap.Add(news.GetID(), numToKnow);

    }
}
