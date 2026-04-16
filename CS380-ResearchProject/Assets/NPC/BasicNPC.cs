// using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class BasicNPC : MonoBehaviour
{
    float timer = 2.0f;
    public Vector2 TTM_Range;
    public Vector2 Travel_Range;

    public City city;
    Vector3 initialPos;
    public float Radius;

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
          Radius = city.GetComponent<CapsuleCollider>().radius;
      }

      if (TTM_Range == Vector2.zero)
          TTM_Range = new Vector2(2.0f, 5.0f);

      if (Travel_Range == Vector2.zero)
          Travel_Range = new Vector2(4.0f, 6.0f);

      if (Radius == 0) Radius = 8.0f;

        //mr.material.color = Color.red;
    }

    void Update()
    {
        if (timer < 0.0f)
        {
            //Debug.Log("Moving NPC");
            //StartCoroutine(MoveNPC());
            MoveAgentRandomly();
            timer = Random.Range(TTM_Range.x, TTM_Range.y);
        }
        timer -= Time.deltaTime;
    }

    void MoveAgentRandomly()
    {
      float minDist = 5f;
      float maxDist = 50f;
      Vector3 destination = new Vector3(Random.insideUnitCircle.x, 0f, Random.insideUnitCircle.y) * Random.Range(minDist, maxDist);
      _agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collision");
            GameObject.Find("GridManager").GetComponent<GridManager>().PlantNewsAtWorldPosition(collision.transform.position);

            //if collision is NPC
            /*
             * check npc's news list
             * if the last most value is different than my last most value, then enter double check
             * if col newest exists in my news list -> keep color. IF NOT, change mesh color to newest news color
             * */
        }
        if (collision.gameObject.tag == "NPC")
        {
            List<News> npcNewsList = collision.transform.GetComponent<NPCBrain>().knownNewsList;
            if ( npcNewsList.Count == 0) return;


            if(npcNewsList[npcNewsList.Count-1] != newsList[newsList.Count - 1])
            {
                if (newsList.Contains(npcNewsList[npcNewsList.Count - 1]) == false)    //UNSURE IF NEWSLIST IS PASSED COPY OR REFERENCE, COULD BE ISSUE HERE
                {
                    News newNews = npcNewsList[npcNewsList.Count - 1];
                    //mr.material.color = newNews.GetColor();
                    //newsList.Add(newNews);
                    //city.colorMap[newNews.GetColor()] += 1;
                    LearnNews(newNews);

                }
            }
        }
    }

    public void LearnNews(News news)
    {
        mr.material.color = news.GetColor();
        newsList.Add(news);
        city.colorMap[news.GetColor()] += 1;

        if(city.colorMap[news.GetColor()] > city.amtKnownMost)
        {
            city.newestNews = news.GetColor();
            city.amtKnownMost = city.colorMap[news.GetColor()];
        }
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
