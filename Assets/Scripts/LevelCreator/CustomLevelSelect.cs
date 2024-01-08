using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject optionPrefab;

    [SerializeField, Tooltip("the IO script")]
    private ElevatorIO IO;

    [SerializeField, Tooltip("the parent transform for all of the created Prefabs")]
    private Transform holderTransform;

    [SerializeField, Tooltip("the button to move onto playing/editing the selected level")]
    private Button finalButton;

    [SerializeField, Tooltip("true if this level selector will be used for the level editor, false if it will be used for playing levels")]
    private bool editMode = false;

    [SerializeField, Tooltip("the default option")]
    private SelectOption defaultOption;

    [SerializeField, Tooltip("the currently selected option")]
    private SelectOption currentOption;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevels()
    {

        List<string[]> validLevels = IO.GetCustomFiles(editMode);
        foreach (string[] level in validLevels)
        {
            GameObject thisObj = Instantiate(optionPrefab, holderTransform);
            SelectOption thisOption = thisObj.GetComponent<SelectOption>();
            thisOption.SetOption(level[1], level[2], this);
        }

        if (defaultOption != null)
        {
            defaultOption.Select();
        }
    }

    public void SetNewOption(SelectOption option)
    {
        if(currentOption == option) return;
        currentOption.Deselect();
        currentOption = option;
    }
}
