/*
 * DialogueOptionBox.cs
 * Author: Andrew Knopf (andrew.knopf@digipen.edu)
 * 4 April 2026
 * DigiPen Institute of Technology 2026 (C)
 */
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptionBox : MonoBehaviour
{
  public GridLayoutGroup contentContainer; // The parent object of the new options for formatting

  private void Awake()
  {
    // Just in case we forget to serialize
    FindContentContainer();
  }

  private void Start()
  {
    // Just in case we forget to serialize
    FindContentContainer();
  }

  // Just in case we forget to serialize
  void FindContentContainer()
  {
    if (contentContainer == null)
    {
      Debug.LogWarning("You forgot to assign the contentContainer in the inspector!");
      contentContainer = GetComponentInChildren<GridLayoutGroup>();
    }
  }


  public void AddDialogueOption(GameObject newOption)
  {
    // Make a new dialogue option inside the content container
    Instantiate(newOption, contentContainer.transform);
  }

  public void DestroyCurrentDialogueOptions()
  {
    // Find all the old dialogue options and remove them
    DialogueOption[] currentOptions = contentContainer.GetComponentsInChildren<DialogueOption>();
    foreach (DialogueOption option in currentOptions)
      Destroy(option.gameObject);
  }

}
