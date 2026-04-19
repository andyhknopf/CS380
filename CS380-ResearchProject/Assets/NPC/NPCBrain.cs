using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Opinion;

public class NPCBrain : MonoBehaviour
{
  [HideInInspector]
  public GridNode gridLocation;
  [HideInInspector] 
  public Dictionary<News, Opinion> opinions = new Dictionary<News, Opinion>();
  public List<News> knownNewsList = new List<News>();

  [HideInInspector]
  public News lastReceivedNews = null;

  const int UNINITIALIZED_VALUE = -999;
  public static float _loyaltyToKing = UNINITIALIZED_VALUE; // random value between -1 and 1


  private void Awake()
  {
    // 3 test opinionInfluencers

    // Loyalty to king

    // Distance from subject

    // 

    if (_loyaltyToKing == UNINITIALIZED_VALUE)
      _loyaltyToKing = Random.Range(-1, 2);
  }

  private void Update()
  {
    // Color code the NPC if they have received news
    if (lastReceivedNews != null)
      ColorCodeNPC();
  }

  public float CalculateOpinionHeuristic(News news)
  {
    // Get access to the current cities data
    //string currentCityName = SceneManager.GetActiveScene().name;
    //CityNewsRegistry.CityData cityData = CityNewsRegistry.Instance._registry[currentCityName];
    //cityData.cityPositions

    float opinion = 0.0f;

    // TODO: Get dynamically from world data
    Vector3 subjectWorldPos = news.subject.location;

    // Calculate heuristic based on opinion influencer
    switch (news.influencer)
    {
      // How far away are we from the subject in question?
      case News.OpinionInfluencer.DISTANCE_FROM_SUBJECT:
        float maxDistance = 100.0f; // TODO: Replace with total map size
        float dist = Vector3.Distance(subjectWorldPos, subjectWorldPos);
        float normalizedDist = dist / maxDistance;
        opinion = 1.0f - normalizedDist;
        break;

      // How loyal are we to the king?
      case News.OpinionInfluencer.LOYALTY_TO_KING:
        opinion = _loyaltyToKing;
        break;

      // How far north / south are we?
      case News.OpinionInfluencer.LATTITUDE:

        CityInner cityInner = FindFirstObjectByType<CityInner>();
        Debug.Assert(cityInner != null, "This should never be null!");
        CityNewsRegistry.CityData data = CityNewsRegistry.Instance._registry[cityInner.cityId];

        float lattitude = data.location.z;
        float topMap = 25;
        float bottomMap = -25f;

        if (lattitude > topMap)
          opinion = 1;
        else if (lattitude < topMap && lattitude > bottomMap)
          opinion = 0;
        else if (lattitude < bottomMap)
          opinion = -1;

        break;
    }

    return opinion;
  }

  public string SetOpinionString(News news, float heuristicValue)
  {
    string opinionString = string.Empty;

    // Get the opinon based on heuristic and action positivity (positive or negative from perspective of if NPC likes the subject)
    float weightedOpinion = heuristicValue * news.action.value;

    // Randomly choose a string chunk to say
    int randIndex = Random.Range(0, news.subject.negativeOpinionStrings.Count);

    if (weightedOpinion < 0) // Negative opinion
    {
      opinionString += news.subject.negativeOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)heuristicValue, news.influencer);
    }
    else if (weightedOpinion == 0) // Neutral opinion
    {
      opinionString += news.subject.neutralOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)heuristicValue, news.influencer);
    }
    else if (weightedOpinion > 0) // Positive opinion
    {
      opinionString += news.subject.positiveOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)heuristicValue, news.influencer);
    }

    return opinionString;
  }

  public string GetExplanationOfInfluence(int heuristicValue, News.OpinionInfluencer opInfluencer)
  {
    string explanation = string.Empty;

    if (opInfluencer == News.OpinionInfluencer.DISTANCE_FROM_SUBJECT)
    {
      switch (heuristicValue)
      {
        case -1:
            explanation += "I live very far away.";
          break;
        case 0:
            explanation += "that's not too far away from here.";
          break;
        case 1:
            explanation += "that's within walking distance!";
          break;
      }
    }
    else if (opInfluencer == News.OpinionInfluencer.LOYALTY_TO_KING)
    {
      switch (heuristicValue)
      {
        case -1:
            explanation += "I hate the king!";
          break;
        case 0:
            explanation += "politics make me uncomfy so I don't think about it.";
          break;
        case 1:
            explanation += "I love the king, I don't care what files his name is mentioned in!";
          break;
      }
    }
    else if (opInfluencer == News.OpinionInfluencer.LATTITUDE)
    {
      switch (heuristicValue)
      {
        case -1:
            explanation += "I was born and raised down here in the south.";
          break;
        case 0:
            explanation += "I live in the sweet-spot between north and south.";
          break;
        case 1:
            explanation += "I live here in the frigid north.";
          break;
      }
    }
    else
    {
      Debug.Assert(true == false, "Something is wrong, this code should never be reached.");
    }

    return explanation;
  }


  public void AddToKnownNews(News news)
  {
    knownNewsList.Add(news);
    lastReceivedNews = news;


    CityInner cityInner = FindFirstObjectByType<CityInner>();
    Debug.Assert(cityInner != null, "This should never be null!");
    cityInner.OnNPCLearnedNews(news);
  }

  public bool NewsAlreadyKnown(News news)
  {
    foreach (var item in knownNewsList)
    {
      if (news.GetID() == item.GetID())
        return true;
    }
    

    return false;
  }

  // Color the NPC the same color as the associated piece of last received news
  public void ColorCodeNPC()
  { 
    GetComponent<Renderer>().material.color = lastReceivedNews.GetID();

  }
}
