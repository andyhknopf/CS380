/*
 * DialogueOption.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class DialogueOption : MonoBehaviour
{
  // Note: How do we go to the next dialogue?
  // 1 way? 
  [SerializeField] GameObject _nextPrompt = null; // Takes you to the next dialogue prompt
  public string text;
  TextMeshProUGUI _textMesh;


  [Header("News")]
  public bool isNewsOption;
  public News news;


  private void Awake()
  {
    _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    // _nextPrompt = null;
  }

  private void Update()
  {
    // If news node has not been touched
    if (isNewsOption)
    {
      string newsString = "Did you hear that " + news.newsString + "?"; // Add the question mark at the end
      
    }
      
    _textMesh.text = text;
  }
  public void Select()
  {
    // TODO: Add logic that happens when the player selects this dialogue option
    // -Updating an NPCs knowledge
    // -Changing an NPCs opinion
    // -Moving a global event state etc...

    // Move to the next dialogue prompt
    if (_nextPrompt == null)
    {
      DialogueManager.Instance.onLastPrompt = true;
      return;
    }

    // If its a news reaction, set the new dialogue
    NewsReaction newsReaction = _nextPrompt.GetComponent<NewsReaction>();
    if (newsReaction != null )
    {
      DialoguePrompt dialoguePrompt = _nextPrompt.GetComponent<DialoguePrompt>();
      Debug.Assert(dialoguePrompt != null);

      // Clear the current dialogue lines
      dialoguePrompt.dialogueLines.Clear();

      // React to the new news
      newsReaction.ReactToNews(news);
    }

    DialogueManager.Instance.MoveToNextPrompt(_nextPrompt);
  }
}
