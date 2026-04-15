using System.Collections.Generic;
using UnityEngine;

public class NewsReaction : MonoBehaviour
{
  DialoguePrompt _parentPrompt;
  NPCBrain _npcBrain;

  public string[] alreadyHeardStrings;
  public string[] neverHeardStrings;

  private void Awake()
  {
    GetComponentReferences();
  }

  void GetComponentReferences()
  {
    _parentPrompt = GetComponent<DialoguePrompt>();
    _npcBrain = DialogueManager.Instance.currentNpc;
  }
  public void ReactToNews(News news)
  {
    GetComponentReferences();

    // [Confirm/Denial statement] that [event]. [Event Reaction] because [reaction modifier]
    string promptString = string.Empty;
    int randIndex = 0;

    // If they've heard or not
    if (_npcBrain.NewsAlreadyKnown(news))
    {
      randIndex = Random.Range(0, alreadyHeardStrings.Length);
      promptString += alreadyHeardStrings[randIndex];
    }
    else
    {
      // Add the news
      _npcBrain.AddToKnownNews(news);
      randIndex = Random.Range(0, neverHeardStrings.Length);
      promptString += neverHeardStrings[randIndex];
    }

    
    promptString += " that ";

    // News event
    promptString += news.subject.name + " " + news.action.text + ".";

    // Reaction
    // Just cast to int for now
    float opinion = _npcBrain.CalculateOpinionHeuristic(news);
    promptString += _npcBrain.SetOpinionString(news, opinion);

    // Explanation
    promptString += _npcBrain.GetExplanationOfInfluence((int)opinion, news.influencer);

    // Add the whole string
    _parentPrompt.dialogueLines.Add(promptString);
  }

  // Update is called once per frame
  void Update()
  {
    
  }
}
