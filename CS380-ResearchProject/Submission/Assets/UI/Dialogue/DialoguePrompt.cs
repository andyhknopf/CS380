/*
 * DialoguePrompt.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialoguePrompt : MonoBehaviour
{
  public GameObject[] responses; // List of the dialogue options the player can respond with
  public List<string> dialogueLines; // The lines of dialogue this prompt has
}
