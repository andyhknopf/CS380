using UnityEngine;

public class InteractionPopup : MonoBehaviour
{
    Vector3 initialPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      //transform.position = initialPos;
      transform.rotation = Quaternion.identity;
      // transform.LookAt(Camera.main.transform);
    }
}
