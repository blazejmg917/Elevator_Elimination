using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject optionPrefab;

    [SerializeField, Tooltip("the IO script")]
    private ElevatorIO IO;

    [SerializeField, Tooltip("the main menu custom ui script")]
    private MainMenuCustomUI UI;

    [SerializeField, Tooltip("the parent transform for all of the created Prefabs")]
    private Transform holderTransform;

    [SerializeField, Tooltip("the button to move onto playing/editing the selected level")]
    private Button finalButton;

    [SerializeField, Tooltip("the text on the final button")]
    private TMP_Text buttonText;

    [SerializeField, Tooltip("place to display error messages")]
    private TMP_Text errorText;

    [SerializeField, Tooltip("true if this level selector will be used for the level editor, false if it will be used for playing levels")]
    private bool editMode = false;

    [SerializeField, Tooltip("the default option")]
    private SelectOption defaultOption;

    [SerializeField, Tooltip("the currently selected option")]
    private SelectOption currentOption;

    [SerializeField]private List<SelectOption> selectedOptions;
    // Start is called before the first frame update
    void Start()
    {
        if (!IO)
        {
            IO = GetComponent<ElevatorIO>();
            if (!IO)
            {
                IO = gameObject.AddComponent<ElevatorIO>();
            }
        }
        //selectedOptions = new List<SelectOption>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevels()
    {
        ClearOptions();
        if (selectedOptions == null)
        {
            selectedOptions = new List<SelectOption>();
        }
        List<string[]> validLevels = IO.GetCustomFiles(editMode);
        foreach (string[] level in validLevels)
        {
            GameObject thisObj = Instantiate(optionPrefab, holderTransform);
            SelectOption thisOption = thisObj.GetComponent<SelectOption>();
            thisOption.SetOption(level[0],level[1], level[2], this);
            selectedOptions.Add(thisOption);
        }
        Debug.Log("loaded Levels: " + selectedOptions.Count);

        if (defaultOption != null)
        {
            defaultOption.Select();
        }
    }

    public void SetNewOption(SelectOption option)
    {
        Debug.Log("setting new option");
        if(currentOption == option) return;
        if (currentOption)
        {
            currentOption.Deselect();
        }

        currentOption = option;
        if (!currentOption || currentOption == defaultOption)
        {
            errorText.text = "";
            if (!editMode)
            {
                finalButton.interactable = false;
            }
            else if(buttonText)
            {
                buttonText.text = "New";
            }
        }
        else
        {

            int error = 0;
            if (IO.ReadFileFullCheck(option.GetLevel(), out error, editMode))
            {
                errorText.text = "";
                if (editMode)
                {
                    buttonText.text = "Edit";
                }
                else
                {
                    finalButton.interactable = true;
                    
                }
            }
            else
            {
                if (editMode)
                {
                    buttonText.text = "Edit";
                }

                finalButton.interactable = false;
                errorText.text = "Cannot load level " + option.GetLevel() + ": error code " + error + ": " + GameManager.Instance.GetErrorCodeMessage(error);
            }
            
        }
    }

    public void ClearOptions()
    {
        if (selectedOptions == null)
        {
            return;
        }
        while (selectedOptions.Count > 0)
        {
            Destroy(selectedOptions[0].gameObject);
            selectedOptions.RemoveAt(0);
        }
        Debug.Log("clearing options");
    }

    public void StartCustomEdit()
    {
        if (!editMode)
        {
            Debug.LogError("TRYING TO START EDITING FROM NON-EDIT-MODE SELECTION");
            return;
        }

        if (!currentOption)
        {
            UI.LoadCustomLevelToEditor("");
        }
        UI.LoadCustomLevelToEditor(currentOption.GetLevel());
    }

    public void StartCustomPlay()
    {
        if (editMode)
        {
            Debug.LogError("TRYING TO START PLAYING FROM EDIT MODE SELECTION");
            return;
        }

        if (!currentOption || currentOption == defaultOption)
        {
            Debug.LogWarning("TRYING TO PLAY WITH NO LEVEL SELECTED");
            return;
        }
        UI.LoadCustomLevelToRun(currentOption.GetLevel());
    }
}
