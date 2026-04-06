/*
 * DialogueBox.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
  public static DialogueBox Instance { get; set; }
  // DialogueText
  Text _textComponent;
  DialogueText _dialogueText;
  [Header("Dialogue Settings")]
  public float textSpeed = 0.05f;

  public List<string> lines = new List<string>();
  [HideInInspector] public bool onLastLine;
  [HideInInspector] public bool canEnd;
  int _index = 0;
  bool _isTyping = false;

  // Set this instance of the singleton
  void SetInstance()
  {
    // If theres already an instance, and it's not this specific object
    if (Instance != null && Instance != this)
      Destroy(gameObject); // Delete yourself
    else
      Instance = this; // Assign for the first time
  }

  // Runs on component construction
  private void Awake()
  {
    SetInstance();
    GetComponentReferences();
    onLastLine = false;
  }

  // Get references to proper components
  private void GetComponentReferences()
  {
    // Get a reference to the text component
    _dialogueText = GetComponentInChildren<DialogueText>();
    Debug.Assert(_dialogueText != null, "There should always be a dialogue text on the DialogueBox!");
    _textComponent = _dialogueText.GetComponent<Text>();
  }

  private void Start()
  {
    // Ensure dialogue starts empty and hidden
    // textComponent.text = string.Empty;
    // gameObject.SetActive(false);
    // StartDialogue(lines);
  }

  private void Update()
  {
    // Check for errors
    // TODO: Raise a better warning but only when applicable (this doesn't handle all cases)
    if (!gameObject.activeSelf || lines.Count == 0)
      return;

    // Check if the last line of dialogue is being displayed
    if (_index == lines.Count - 1)
      onLastLine = true;
    // If the user clicks
    if (Input.GetMouseButtonDown(0))
      OnUserClick();
  }

  // All logic when user clicks (for readability)
  private void OnUserClick()
  {
    // Instantly complete the current line
    if (_isTyping)
    {
      StopAllCoroutines();
      _textComponent.text = lines[_index];
      _isTyping = false;
    }

    // Move to the next dialogue line when line is done typing
    else if (!onLastLine) 
      NextLine();
  }

  public void StartDialogue(List<string> newLines)
  {
    
    GetComponentReferences();
    StopAllCoroutines();
    onLastLine = false;
    lines = new List<string>(newLines);
    _index = 0;
    _textComponent.text = string.Empty;
    canEnd = false;
    gameObject.SetActive(true);
    StartCoroutine(TypeLine());
  }

  /* 
   * Description: Add a line of dialogue to the NPC dialogue box
   * line - The line of dialogue to add
   * forceRefresh - Option to delete all current dialogue and start fresh with the line just added
  */
  public void AddDialogueLine(string line, bool forceRefresh)
  {
    // Just append to the end if not refreshing
    if (!forceRefresh)
    {
      lines.Add(line);
      return;
    }

    // Stop any typing or data addition in progress
    StopAllCoroutines();

    // Flip appropriate flags
    onLastLine = false;
    // lines.Clear();

    // Append the next line of dialogue
    lines.Add(line);

    // Free up any remaining data
    _index = 0;
    _textComponent.text = string.Empty;

    // Turn on the UI box then type a line
    gameObject.SetActive(true);
    StartCoroutine(TypeLine());
  }

  // Type a line of dialogue into the box
  private IEnumerator TypeLine()
  {
    // Flip related flags
    canEnd = false;
    _isTyping = true;
    _textComponent.text = string.Empty;

    // Add a character to the dialogue box then wait for textSpeed seconds
    foreach (char c in lines[_index].ToCharArray())
    {
      _textComponent.text += c;
      yield return new WaitForSeconds(textSpeed);
    }

    _isTyping = false;
  }

  // Move to the next line of dialogue
  public void NextLine()
  {
    // If there are still more lines of dialogue
    if (_index < lines.Count - 1)
    {
      // Move to the next dialogue index
      _index++;

      // Stop any typing or data adding in progress
      StopAllCoroutines();

      // Type the next line of dialogue
      StartCoroutine(TypeLine());
    }
  }


  // End the NPC's dialogue session
  public void EndDialogue()
  {
    // Stop all typing and adding of data
    StopAllCoroutines();

    // Free up any related resources
    lines.Clear();
    _textComponent.text = string.Empty;

    // Disable the UI GameObject
    gameObject.SetActive(false);
  }
}
