using System.Collections.Generic;
using UnityEngine;
using static CityNewsRegistry;

public class CityInner : MonoBehaviour
{
    public string cityId;
    public List<GameObject> npcs;

    private CityData _data;

    public string cityHomeSceneName = "DemoScene";

    private GridManager gm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = FindFirstObjectByType<GridManager>();

        NPCBrain[] foundNPCs = FindObjectsByType<NPCBrain>(FindObjectsSortMode.None);
        npcs = new List<GameObject>();
        foreach (NPCBrain npc in foundNPCs)
            npcs.Add(npc.gameObject);


        if (CityNewsRegistry.Instance == null)
        {
            CityNewsRegistry registry = FindFirstObjectByType<CityNewsRegistry>();
            if (registry == null)
            {
                Debug.LogError("Cannot find CityNewsData anywhere in scene!");
                return;
            }
            _data = registry.GetOrCreate(cityId);
            _data.totalNPCs = npcs.Count;
            Debug.LogWarning("CityNewsData.Instance was null, found it via FindFirstObjectByType");
            return;
        }

        _data = CityNewsRegistry.Instance.GetOrCreate(cityId);
        _data.totalNPCs = npcs.Count;


        SpreadPendingNews();
        gm.HideGrid();
    }
    void SpreadPendingNews()
    {
        foreach (News n in _data.receivedNews)
        {
            if (_data.npcKnowledgeMap.ContainsKey(n.GetID())) continue; // already seeded

            int numToKnow = Random.Range(1, 2 * (npcs.Count / 3));
            for (int i = 0; i < numToKnow; i++)
                npcs[i].GetComponent<BasicNPC>().LearnNews(n);

            _data.npcKnowledgeMap[n.GetID()] = numToKnow;
        }
    }

    //andy call this when they learn something from dialogue
    public void OnNPCLearnedNews(News n)
    {
        var id = n.GetID();
        _data.npcKnowledgeMap.TryGetValue(id, out int current);
        _data.npcKnowledgeMap[id] = current + 1;

        if (!_data.HasReceived(n))
            _data.receivedNews.Add(n);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
