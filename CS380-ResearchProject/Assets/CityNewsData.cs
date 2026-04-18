using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CityNewsData : MonoBehaviour
{
    public static CityNewsData Instance;


    private Dictionary<string, CityData> _registry = new();
    public List<string> citySceneNames;


    [System.Serializable]
    public class CityData
    {
        public List<News> receivedNews = new();
        public List<News> sentNews = new();
        public Dictionary<Color, int> npcKnowledgeMap = new(); // newsId -> count
        public int totalNPCs; // set by the inner scene on load

        public bool HasReceived(News n) {
            return receivedNews.Exists(r => r.GetID() == n.GetID());
        }

        public bool HasSent(News n) {
            return sentNews.Exists(s => s.GetID() == n.GetID());

        }

        public int GetKnownCount(News n) { 
            return npcKnowledgeMap.TryGetValue(n.GetID(), out int c) ? c : 0;
        }
    }

    //set to map index to a list

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CityData GetOrCreate(string cityId)
    {
        if (!_registry.ContainsKey(cityId))
            _registry[cityId] = new CityData();
        return _registry[cityId];
    }

}
