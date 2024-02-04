using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCreationUiHandler : MonoBehaviour
{
    [SerializeField, Tooltip("the text box to display error messages from")]
    private TMP_Text errorText;
    [SerializeField, Tooltip("the text box to display starting level names")]
    private TMP_InputField nameText;
    [SerializeField, Tooltip("the text box to display starting creator names")]
    private TMP_InputField creatorText;
    [SerializeField, Tooltip("the text box to display starting floor counts")]
    private TMP_InputField floorCountText;

    [SerializeField, Tooltip("If the level has been changed since it was last saved")]
    private bool levelChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LevelLoaded(string levelName, string creatorName, int floorCount)
    {
        nameText.text = levelName;
        creatorText.text = creatorName;
        floorCountText.text = floorCount.ToString();
        TileManager.Instance.SetFloorCount(floorCount);
    }

    public void LevelChanged()
    {
        errorText.text = string.Empty;
    }

    public void UpdateLevelName(string levelName)
    {
        TileManager.Instance.SetName(levelName);
        LevelChanged();
    }

    public void UpdateCreatorName(string creatorName)
    {
        TileManager.Instance.SetCreatorName(creatorName);
        LevelChanged();
    }

    public void UpdateFloorCount(string floorCount)
    {
        TileManager.Instance.SetFloorCount(int.Parse(floorCount));
        LevelChanged();
    }

    public void TrySaveLevel()
    {
        int errorCode = TileManager.Instance.SaveLevelToFile();
        if (errorCode == 0)
        {
            levelChanged = false;
            errorText.text = "Level Saved succesfully";
            return;
        }
        errorText.text = "Failed to save level, error code: " + errorCode;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void DisplayError(string error)
    {
        errorText.text = error;
    }

    public void SetUIActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}
