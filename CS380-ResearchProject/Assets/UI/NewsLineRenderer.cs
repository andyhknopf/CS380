using UnityEngine;

public class NewsLineRenderer : MonoBehaviour
{
  [HideInInspector] public GameObject sender, receiver;
  LineRenderer lineRenderer;
  float randomHeightOffset = 0.0f;
  private void Awake()
  {
    lineRenderer = GetComponent<LineRenderer>();
    randomHeightOffset = Random.Range(-1f, 1f);
  }



  // Update is called once per frame
  void Update()
  {
    Vector3 sendPos = new Vector3(sender.transform.position.x, sender.transform.position.y + randomHeightOffset, sender.transform.position.z);
    Vector3 receivePos = new Vector3(receiver.transform.position.x, receiver.transform.position.y + randomHeightOffset, receiver.transform.position.z);
    lineRenderer.SetPosition(0, sendPos);
    lineRenderer.SetPosition(1, receivePos);
  }
}
