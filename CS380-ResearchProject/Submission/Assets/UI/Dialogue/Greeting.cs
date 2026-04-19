using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Greeting : MonoBehaviour
{
  [Header("Dialogue for first time meeting")]
  public List<string> firstTimeDialogueLines;

  [Header("Dialogue for subsequent meetings")]
  public List<string> repeatDialogue;

  DialoguePrompt _greetingPrompt;

  private void Awake()
  {
    _greetingPrompt = GetComponent<DialoguePrompt>();
  }
}
