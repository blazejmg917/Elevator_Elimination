using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuErrorScript : MonoBehaviour
{
    [SerializeField, Tooltip("a list of all buttons that need to be disabled when the error is being displayed.")]private List<Button> disableButtons = new List<Button>();

    [SerializeField, Tooltip("the panel for errors")]
    private GameObject errorPanel;
    [SerializeField, Tooltip("the text field to display errors in")]private TMP_Text errorText;
    // Start is called before the first frame update
    void Start()
    {
        if (!errorPanel)
        {
            errorPanel = gameObject;
        }
    }

    public void DisplayError(string error)
    {
        errorPanel.SetActive(true);
        errorText.text = error;
        foreach (Button button in disableButtons)
        {
            button.interactable = false;
        }
    }

    public void HideError()
    {
        
        foreach (Button button in disableButtons)
        {
            button.interactable = true;
        }
        errorPanel.SetActive(false);
        errorText.text = "";

    }
}
