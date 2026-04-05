using UnityEngine;

public class Interaction : MonoBehaviour
{
    Player playerscript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerscript = transform.parent.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "NPC")
        {
            if (!playerscript.targets.Contains(collision.gameObject))
            {
                playerscript.targets.Add(collision.gameObject);
            }
            Transform E = collision.transform.GetChild(0);
            E.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "NPC")
        {
            if (playerscript.targets.Contains(collision.gameObject))
            {
                playerscript.targets.Remove(collision.gameObject);
            }

            Transform E = collision.transform.GetChild(0);
            E.gameObject.SetActive(false);
        }
    }

}
