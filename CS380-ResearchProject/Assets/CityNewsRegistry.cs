using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CityNewsRegistry : MonoBehaviour
{
    public static CityNewsRegistry Instance;


    public Dictionary<string, CityData> _registry = new();
    public List<string> citySceneNames;


    [SerializeField] private List<Transform> _locations;

    //private List<Vector3> cityPositions
    //public Dictionary<string, Vector3> cityPositions = new();


    [System.Serializable]
    public class CityData
    {
        public Vector3 location; //World position on big map
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

    private void Start()
    {
        for (int i = 0; i < citySceneNames.Count; i++)
        {
            //cityPositions[citySceneNames[i]] = new CityData();
            CityData cd = GetOrCreate(citySceneNames[i]);
            cd.location = _locations[i].position;
        }
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
