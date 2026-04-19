// using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class BasicNPC : MonoBehaviour
{
    float timer = 2.0f;
    public Vector2 TTM_Range;
    public Vector2 Travel_Range;

    public City city;
    Vector3 initialPos;
    public float knowledgeRadius;
    
    const int MAX_LOOPS = 10000;
    NavMeshAgent _agent;

    public List<News> newsList;

    private MeshRenderer mr;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        newsList = GetComponent<NPCBrain>().knownNewsList;
        mr = GetComponent<MeshRenderer>();
        
    }
  void Start()
    {
      if (city == null) initialPos = transform.position;
      else
      {
          initialPos = city.transform.position;
          // Radius = city.GetComponent<CapsuleCollider>().radius;
      }

      if (TTM_Range == Vector2.zero)
          TTM_Range = new Vector2(2.0f, 5.0f);

      if (Travel_Range == Vector2.zero)
          Travel_Range = new Vector2(4.0f, 6.0f);

      if (knowledgeRadius == 0) knowledgeRadius = 8.0f;

        //mr.material.color = Color.red;
    }

    void Update()
    {
      // Talk to other NPCs if within range
      TalkToOtherNPCs();

      // Move randomly based on a timer
      if (timer < 0.0f)
      {
        // Move randomly
        MoveAgentRandomly();
        timer = Random.Range(TTM_Range.x, TTM_Range.y);
      }

      // Decrease the timer count
      timer -= Time.deltaTime;
    }

  private void TalkToOtherNPCs()
  {
    News newNews = null;

    NPCBrain[] npcs = FindObjectsByType<NPCBrain>(FindObjectsSortMode.None);
    Debug.Assert(npcs.Length > 1, "There should always be more than 1 basic NPC");
    foreach (NPCBrain npc in npcs)
    {
      float dist = Vector3.Distance(transform.position, npc.transform.position);
      if (dist > knowledgeRadius)
        continue;

      // Skip if the other NPC has any news
      List<News> npcNewsList = npc.knownNewsList;
      if (npcNewsList.Count == 0) continue;

      // If I don't know know anything yet
      if (newsList.Count == 0)
      {
        Debug.Log("NewsListCount");
        newNews = npcNewsList[npcNewsList.Count - 1];
        CreateNewsLine(npc, newNews);
        LearnNews(newNews);

        return;
      }

      // If this piece of news is already known
      if (npcNewsList[npcNewsList.Count - 1] == newsList[newsList.Count - 1])
        continue;

      // UNSURE IF NEWSLIST IS PASSED COPY OR REFERENCE, COULD BE ISSUE HERE
      if (newsList.Contains(npcNewsList[npcNewsList.Count - 1]) == true)
        continue;

      Debug.Log("newNewsCreated");
      newNews = npcNewsList[npcNewsList.Count - 1];

      // Draw a line inbetween them
      CreateNewsLine(npc, newNews);

      //mr.material.color = newNews.GetColor();
      //newsList.Add(newNews);
      //city.colorMap[newNews.GetColor()] += 1;
      LearnNews(newNews);
    }
  }

  private void CreateNewsLine(NPCBrain npc, News newNews)
  {
    // Create a line from the prefab
    GameObject newsLine = Resources.Load<GameObject>("Prefabs/News/NewsLineRenderer");

    // Create the news line
    newsLine = Instantiate(newsLine);

    // Set the color to the color of the related news
    LineRenderer lineRenderer = newsLine.GetComponent<LineRenderer>();
    Debug.Assert(lineRenderer != null, "Linerenderer should never be null!");
    lineRenderer.startColor = newNews.GetID();
    lineRenderer.endColor = newNews.GetID();


    // Tell the line the other NPC is sending, we are learning for the first time
    NewsLineRenderer newsLineRenderer = newsLine.GetComponent<NewsLineRenderer>();
    newsLineRenderer.sender = npc.gameObject;
    newsLineRenderer.receiver = gameObject;
  }

  void MoveAgentRandomly()
    {
      float minDist = 1f;
      float maxDist = 12f;

      Vector3 destination = Vector3.zero;
      BasicNPC[] otherNpcs = FindObjectsByType<BasicNPC>(FindObjectsSortMode.None);
      int randIndex = Random.Range(0, otherNpcs.Length);

      // Walk randomly or to another NPC
      if (Random.Range(0f, 100f) < 50f)
        destination = otherNpcs[randIndex].transform.position;
      else
        destination = transform.position + new Vector3(Random.insideUnitCircle.x, 0.0f, Random.insideUnitCircle.x) * Random.Range(minDist, maxDist);

      _agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collision");
            //GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(collision.transform.position);

            //if collision is NPC
            /*
             * check npc's news list
             * if the last most value is different than my last most value, then enter double check
             * if col newest exists in my news list -> keep color. IF NOT, change mesh color to newest news color
             * */
        }
    }

    public void LearnNews(News news)
    {
        CityInner cityInner = FindFirstObjectByType<CityInner>();
        Debug.Assert(cityInner != null, "This should never be null!");

        cityInner.OnNPCLearnedNews(news);

        GetComponent<NPCBrain>().lastReceivedNews = news;
        mr.material.color = news.GetID();
        newsList.Add(news);
        //city.newestNews = news.GetColor();

        // Add this npc to this list of npcs that know this news
        //city.colorMap[news.GetID()] += 1;

        //// Update the most popular piece of known news in this city
        //if(city.colorMap[news.GetID()] > city.amtKnownMost)
        //{
        //    city.amtKnownMost = city.colorMap[news.GetID()];
        //}
    }


    //IEnumerator MoveNPC()
    //{
    //    float TTM = Random.Range(TTM_Range.x, TTM_Range.y);

    //    float t = 0;
    //    Vector3 start = transform.position;

    //    Vector3 destination = start + new Vector3(Random.Range(-Travel_Range.x, Travel_Range.x), 0.0f , Random.Range(-Travel_Range.y, Travel_Range.y));

    //    for (int i = 0; Vector2.Distance(initialPos, destination) > Radius; ++i)
    //    {
    //        if  (i > MAX_LOOPS)
    //        {
    //          Debug.LogError("Can't find a proper destination to move to!");
    //          break;
    //        }
    //        destination = start + new Vector3(Random.Range(-Travel_Range.x, Travel_Range.x), 0.0f, Random.Range(-Travel_Range.y, Travel_Range.y));
    //    }

    //    while ( t < 1 || Vector2.Distance(transform.position, destination) <= 1.0f)
    //    {
    //        transform.position = Vector3.Lerp(start, destination, t);
    //        t = t + Time.deltaTime / TTM;
    //        timer = Random.Range(TTM_Range.x, TTM_Range.y);
    //        yield return null;
    //    }
    //    //yield return new WaitForSeconds(3);
    //    //Debug.Log("done move");
    //    yield break;
    //}
}
