using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCreationTestingText : MonoBehaviour
{
    [SerializeField, Tooltip("the text area")] private TMP_Text textArea;
    [SerializeField, Tooltip("the base text")] private string baseText = "Testing\n";
    [SerializeField, Tooltip("the input field for level name")] private TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        if(textArea == null)
        {
            textArea = GetComponentInChildren<TMP_Text>();
        }
    }

    public void DisplayText()
    {
        textArea.text = baseText + inputField.text;
    }
}
