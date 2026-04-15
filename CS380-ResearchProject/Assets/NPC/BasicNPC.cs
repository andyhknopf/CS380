// using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class BasicNPC : MonoBehaviour
{
    float timer = 2.0f;
    public Vector2 TTM_Range;
    public Vector2 Travel_Range;

    public GameObject city;
    Vector3 initialPos;
    public float Radius;

    const int MAX_LOOPS = 10000;
    NavMeshAgent _agent;

  private void Awake()
  {
    _agent = GetComponent<NavMeshAgent>();
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
