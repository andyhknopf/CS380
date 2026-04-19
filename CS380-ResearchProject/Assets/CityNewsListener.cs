using System.Collections.Generic;
using UnityEngine;
using static CityNewsRegistry;

public class CityNewsListener : MonoBehaviour
{
    public string citySceneName; //cityId
    private GridManager gm;
    private CityData _data;

    public int spreadSpeed = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gm = FindFirstObjectByType<GridManager>();
        if (CityNewsRegistry.Instance == null)
        {
            CityNewsRegistry registry = FindFirstObjectByType<CityNewsRegistry>();
            if (registry == null)
            {
                Debug.LogError("Cannot find CityNewsData anywhere in scene!");
                return;
            }
            _data = registry.GetOrCreate(citySceneName);
            Debug.LogWarning("CityNewsData.Instance was null, found it via FindFirstObjectByType");
            return;
        }
        _data = CityNewsRegistry.Instance.GetOrCreate(citySceneName);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForIncomingNews();
        CheckShouldBroadcast();
    }
    void CheckForIncomingNews()
    {
        Vector2Int pos = gm.WorldToGrid(transform.position);
        List<News> gridNews = gm.GetNewsAtNode(pos.x, pos.y);
        if (gridNews == null) return;

        foreach (News n in gridNews)
        {
            if (!_data.HasReceived(n))
            {
                _data.receivedNews.Add(n);
                // Inner scene will pick this up when loaded
            }
        }
    }

    void CheckShouldBroadcast()
    {
        // Only broadcast news the city "grew" internally, not what it just received
        foreach (News n in _data.receivedNews)
        {
            if (_data.HasSent(n)) continue;

            //int known = _data.GetKnownCount(n);
            ////if (_data.totalNPCs > 0 && known > _data.totalNPCs / 2)
            //if (_data.totalNPCs > 0)
            //{
                //change news speed here? taken from city
                n.speed = spreadSpeed;
                gm.PlantGivenNewsAtWorldPosition(transform.position, n);
                Debug.Log($"{gameObject.name} : {transform.position}");
                _data.sentNews.Add(n);
            // }
        }
    }


}
