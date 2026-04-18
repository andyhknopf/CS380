using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    float opinion = 0.0f;
    
    // Calculate heuristic based on opinion influencer
    switch (news.influencer)
    {
      case News.OpinionInfluencer.DISTANCE_FROM_SUBJECT: // Further away 

        float maxDistance = 100.0f; // TODO: Replace with total map size
        float dist = Vector3.Distance(news.subject.gridLocation.worldPos, gridLocation.worldPos);
        float normalizedDist = dist / maxDistance;
        opinion = 1.0f - normalizedDist;
        break;
      case News.OpinionInfluencer.LOYALTY_TO_KING:
        opinion = _loyaltyToKing;
        break;
      case News.OpinionInfluencer.LATTITUDE:
        float lattitude = news.subject.gridLocation.worldPos.y;
        float topMap = 100f;
        float bottomMap = -100f;
        opinion = (topMap / bottomMap) + lattitude;
        break;
     
    }

    return opinion;
  }

  public string SetOpinionString(News news, float opinionValue)
  {
    string opinionString = string.Empty;

    // Negative opinion
    if (opinionValue <= -1)
    {
      int randIndex = Random.Range(0, news.subject.negativeOpinionStrings.Count);
      opinionString += news.subject.negativeOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)opinionValue, news.influencer);
    }
    else if (opinionValue == 0) // Neutral opinion
    {
      int randIndex = Random.Range(0, news.subject.neutralOpinionStrings.Count);
      opinionString += news.subject.neutralOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)opinionValue, news.influencer);
    }
    else if (opinionValue > 0) // Positive opinion
    {
      int randIndex = Random.Range(0, news.subject.positiveOpinionStrings.Count);
      opinionString += news.subject.positiveOpinionStrings[randIndex] + " because ";
      opinionString += GetExplanationOfInfluence((int)opinionValue, news.influencer);
    }

    return opinionString;
  }

  public string GetExplanationOfInfluence(int opinionValue, News.OpinionInfluencer opInfluencer)
  {
    string explanation = string.Empty;
    if (opinionValue <= -1)
    {
      if (opInfluencer == News.OpinionInfluencer.DISTANCE_FROM_SUBJECT)
      {
        explanation += "I live very far away.";
      }
      else if (opInfluencer == News.OpinionInfluencer.LOYALTY_TO_KING)
      {
        explanation += "I hate the king!";
      }
      else if (opInfluencer == News.OpinionInfluencer.LATTITUDE)
      {
        explanation += "I was born and raised down here in the south.";
      }
    }
    else if (opinionValue == 0)
    {
      if (opInfluencer == News.OpinionInfluencer.DISTANCE_FROM_SUBJECT)
      {
        explanation += "that's not too far away from here.";
      }
      else if (opInfluencer == News.OpinionInfluencer.LOYALTY_TO_KING)
      {
        explanation += "politics make me uncomfy so I don't think about it.";
      }
      else if (opInfluencer == News.OpinionInfluencer.LATTITUDE)
      {
        explanation += "I live in the sweet-spot between north and south.";
      }
    }
    else if (opinionValue > 0)
    {
      if (opInfluencer == News.OpinionInfluencer.DISTANCE_FROM_SUBJECT)
      {
        explanation += "that's within walking distance!";
      }
      else if (opInfluencer == News.OpinionInfluencer.LOYALTY_TO_KING)
      {
        explanation += "I love the king, I don't care what files his name is mentioned in!";
      }
      else if (opInfluencer == News.OpinionInfluencer.LATTITUDE)
      {
        explanation += "I live here in the frigid north.";
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
  }

  public bool NewsAlreadyKnown(News news)
  {
    if (knownNewsList.Contains(news))
      return true;

    return false;
  }

  // Color the NPC the same color as the associated piece of last received news
  public void ColorCodeNPC()
  { 
    GetComponent<Renderer>().material.color = lastReceivedNews.GetID();

  }
}
