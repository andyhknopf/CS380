/*
 * DialogueManager.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using JetBrains.Annotations;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

// Responsible for managing the flow of dialogue prompts and responses
// TODO: Make this so that all dialogue lines are being stored in an abstracted
//       buffer for this object, like the prompts and responses. This object
//       should be handling the entire back and forth between the player & NPC.
public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance; // singleton
  public DialogueBox dialogueBox;
  public DialogueOptionBox dialogueOptionBox;
  DialoguePrompt _currentPrompt;
  [HideInInspector] public bool onLastPrompt;

  private void Awake()
  {
    // Set the instance of the singleton manager
    SetInstance();
    GetComponents();

    // Shouldn't start with a current prompt
    // _currentPrompt = null;
    _currentPrompt = FindFirstObjectByType<DialoguePrompt>();
  }

  private void Update()
  {
    // TODO: Replace with where you interact with NPCs
    if (Input.GetKeyDown(KeyCode.T))
      StartConversation(FindFirstObjectByType<DialoguePrompt>().gameObject);

    // On user click
    OnUserClick();
  }

  private void OnUserClick()
  {
    if (Input.GetMouseButtonDown(0))
    {
      // If we are on the last line of dialogue, end the conversation
      if (_currentPrompt != null && dialogueBox.onLastLine && _currentPrompt.responses.Length == 0)
        EndConversation();
    }
  }

  private void GetComponents()
  {
    // Find the required components if user forgot to serialize them
    if (dialogueBox == null)
      dialogueBox = FindFirstObjectByType<DialogueBox>(FindObjectsInactive.Include);
    if (dialogueOptionBox == null)
      dialogueOptionBox = FindFirstObjectByType<DialogueOptionBox>(FindObjectsInactive.Include);

    // Check for errors
    Debug.Assert(dialogueBox != null && dialogueOptionBox != null, "There isn't a dialogue box or dialogue option box in the scene!");
  }

  private void SetInstance()
  {
    // If there is already an instance and it's not me
    if (Instance != null && Instance != this)
      Destroy(this); // Destroy myself
    else
      Instance = this;
  }

  
  // Move to the next dialogue prompt
  public void MoveToNextPrompt(GameObject nextPromptObject)
  {
    // Check for errors
    Debug.Assert(nextPromptObject != null, "Passed prompt GameObject should never be NULL!");

    // Hold the current prompt in a buffer to destroy after reassignment
    DialoguePrompt oldPrompt = _currentPrompt;
    DialoguePrompt nextPrompt = nextPromptObject.GetComponent<DialoguePrompt>();
    Debug.Assert(nextPrompt != null, "The passed dialogue prompt GameObject doesn't contain a DialoguePrompt component!");
    _currentPrompt = nextPrompt;

    // Start the new dialogue
    dialogueBox.StartDialogue(_currentPrompt.dialogueLines);

    // Destroy the current dialogue options and add the new ones
    dialogueOptionBox.DestroyCurrentDialogueOptions();
    foreach (GameObject response in _currentPrompt.responses)
      dialogueOptionBox.AddDialogueOption(response);
  }

  public void EndConversation()
  {
    // Clean up all related data, then deactivate UI objects
    _currentPrompt = null;
    dialogueBox.EndDialogue();
    dialogueOptionBox.gameObject.SetActive(false);
  }

  public void StartConversation(GameObject initialPromptObject)
  {
    // Check for errors
    Debug.Assert(initialPromptObject != null, "The intial conversation prompt GameObject should NEVER be null!");

    // Enable the dialogue box
    dialogueBox.StartDialogue(_currentPrompt.dialogueLines);

    // Enable the player response box
    dialogueOptionBox.gameObject.SetActive(true);

    // Assign the initial prompt
    _currentPrompt = initialPromptObject.GetComponent<DialoguePrompt>();

    // Check for errors
    if (_currentPrompt == null) 
      InitialPromptErrorCheck(initialPromptObject);

    // Add the new responses to the dialogue box
    foreach (GameObject response in _currentPrompt.responses)
      dialogueOptionBox.AddDialogueOption(response);
  }

  // Stop the editor when this happens
  private void InitialPromptErrorCheck(GameObject initialPromptObject)
  {
    string errorString = "Dialogue prompt object: " + initialPromptObject.name + " doesn't have a DialoguePrompt component assigned to it!";
    Debug.Assert(_currentPrompt != null, errorString);
    Debug.Break();
  }
}
