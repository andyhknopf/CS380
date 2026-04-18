using UnityEngine;

public class NewsLineRenderer : MonoBehaviour
{
  [HideInInspector] public GameObject sender, receiver;
  LineRenderer lineRenderer;

  private void Awake()
  {
    lineRenderer = GetComponent<LineRenderer>();
  }



  // Update is called once per frame
  void Update()
  {
    lineRenderer.SetPosition(0, sender.transform.position);
    lineRenderer.SetPosition(1, receiver.transform.position);
  }
}
