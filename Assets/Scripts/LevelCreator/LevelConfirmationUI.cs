using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelConfirmationUI : MonoBehaviour
{
    [SerializeField, Tooltip("the save confirmation UI")]
    GameObject saveConfirmPanel;
    [SerializeField, Tooltip("all of the buttons to deactivate while showing ui")]private List<Button> deactivateButtons = new List<Button>();
    [SerializeField, Tooltip("all of the input fields to deactivate while showing ui")]private List<TMP_InputField> deactivateInputs = new List<TMP_InputField>();

    [SerializeField, Tooltip("the mouse following script to deactivate while showing ui")]
    private GameObject deactivateMouseFollow;

    [SerializeField, Tooltip("text field to display confirmation dialog")]
    private TMP_Text confirmText;
    // Start is called before the first frame update
    void Start()
    {
        if (!saveConfirmPanel)
        {
            saveConfirmPanel = gameObject;
            
        }
        saveConfirmPanel.SetActive(false);
        if (!deactivateMouseFollow)
        {
            deactivateMouseFollow = FindObjectOfType<MouseFollowCreationScene>(true).gameObject;
        }

    }

    public void DisplayConfirmationDialog(string fileName)
    {
        saveConfirmPanel.SetActive(true);
        deactivateMouseFollow.SetActive(false);
        foreach (Button button in deactivateButtons)
        {
            button.interactable = false;
        }

        foreach (TMP_InputField input in deactivateInputs)
        {
            input.interactable = false;
        }

        confirmText.text = "Level with the name " + fileName + " already exists. Would you like to overwrite this file?";
    }

    public void HideConfirmationDialog()
    {
        saveConfirmPanel.SetActive(false);
        deactivateMouseFollow.SetActive(true);
        foreach (Button button in deactivateButtons)
        {
            button.interactable = true;
        }

        foreach (TMP_InputField input in deactivateInputs)
        {
            input.interactable = true;
        }

        confirmText.text = "";
    }
}
