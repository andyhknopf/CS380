using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float incrementDelta;

    public List<GameObject> targets;

    private GridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed /= 100.0f;
        incrementDelta /= 100.0f;
        gridManager = FindFirstObjectByType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //BASIC MOVEMENT
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            pos.z += moveSpeed;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            pos.z -= moveSpeed;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            pos.x -= moveSpeed;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            pos.x += moveSpeed;


        //SPEED CONTROLS
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = Mathf.Clamp(moveSpeed + incrementDelta, 0, 0.5f);
        if (Input.GetKey(KeyCode.LeftControl))
            moveSpeed = Mathf.Clamp(moveSpeed - incrementDelta, 0, 0.4f);

        //TEMPORARY: CREATE NEWS FROM CURRENT POS
        if (Input.GetKeyDown(KeyCode.Z))
            gridManager.PlantNewsAtWorldPosition(pos);

        if (Input.GetKey(KeyCode.V))
            gridManager.EditMap(pos);

        if (targets.Count > 0)
        {
            GameObject target = GetClosestTarget();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interact with " + target.name);

                if (target.tag == "NPC")
                    DialogueManager.Instance.StartConversation(target.GetComponentInChildren<DialoguePrompt>().gameObject, target.GetComponent<NPCBrain>());
                else if (target.tag == "City")
                    EnterCity(target);

            }

        }


        transform.position = pos;
    }

    private void EnterCity(GameObject target)
    {
        City cityScript = target.GetComponent<City>();
        Debug.Assert(cityScript != null, "City script doesnt exist");

        SceneManager.LoadScene(cityScript.citySceneName);

        //throw new NotImplementedException();
    }

    public GameObject GetClosestTarget()
    {
        GameObject target = targets[0];
        for (int i = 0; i < targets.Count; i++) //chooses the closest NPC to talk to
        {
            if (Vector3.Distance(transform.position, targets[i].transform.position) < Vector3.Distance(transform.position, target.transform.position))
            {
                target = targets[i];
            }
        }
        return target;
    }

}


