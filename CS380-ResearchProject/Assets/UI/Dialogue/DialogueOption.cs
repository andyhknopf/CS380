/*
 * DialogueOption.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using UnityEngine;

[System.Serializable]
public class DialogueOption : MonoBehaviour
{
  // Note: How do we go to the next dialogue?
  // 1 way? 
  [SerializeField] GameObject _nextPrompt = null; // Takes you to the next dialogue prompt

  private void Awake()
  {
    // _nextPrompt = null;
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
    
    DialogueManager.Instance.MoveToNextPrompt(_nextPrompt);
  }
}
