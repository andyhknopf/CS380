using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Media;
using UnityEngine;

public class BasicNPC : MonoBehaviour
{
    float timer = 2.0f;
    public Vector2 TTM_Range;
    public Vector2 Travel_Range;

    Vector3 initialPos;
    public float Radius;

    void Start()
    {
        initialPos = transform.position;

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
            StartCoroutine(MoveNPC());
            timer = Random.Range(TTM_Range.x, TTM_Range.y);
        }
        timer -= Time.deltaTime;
    }
    IEnumerator MoveNPC()
    {
        float TTM = Random.Range(TTM_Range.x, TTM_Range.y);

        float t = 0;
        Vector3 start = transform.position;

        Vector3 destination = start + new Vector3(Random.Range(-Travel_Range.x, Travel_Range.x), 0.0f , Random.Range(-Travel_Range.y, Travel_Range.y));

        while (Vector2.Distance(initialPos, destination) > Radius)
        {
            destination = start + new Vector3(Random.Range(-Travel_Range.x, Travel_Range.x), 0.0f, Random.Range(-Travel_Range.y, Travel_Range.y));
        }

        while ( t < 1 || !(Vector2.Distance(transform.position, destination) < 1.0f))
        {
            transform.position = Vector3.Lerp(start, destination, t);
            t = t + Time.deltaTime / TTM;
            timer = Random.Range(TTM_Range.x, TTM_Range.y);
            yield return null;
        }
        //yield return new WaitForSeconds(3);
        //Debug.Log("done move");
        yield break;
    }
}
