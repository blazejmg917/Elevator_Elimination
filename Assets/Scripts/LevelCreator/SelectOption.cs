using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectOption : MonoBehaviour
{
    [SerializeField, Tooltip("the button component")]
    private Button button;

    [SerializeField, Tooltip("the holder for this select option")]
    private CustomLevelSelect holder;

    [SerializeField, Tooltip("the text to display for this option")]
    private TMP_Text textDisplay;

    [SerializeField, Tooltip("the filepath of the level this option holds")]
    private string filepath = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        Debug.Log("selecting");
        button.interactable = false;
        holder.SetNewOption(this);
    }

    public void Deselect()
    {
        button.interactable = true;
    }

    public void SetOption(string file, string level, string creator, CustomLevelSelect owner)
    {
        textDisplay.text = level.Trim() + "\nBY: " + creator.Trim();
        holder = owner;
        filepath = file.Trim();
    }

    public string GetLevel()
    {
        return filepath;
    }
}
