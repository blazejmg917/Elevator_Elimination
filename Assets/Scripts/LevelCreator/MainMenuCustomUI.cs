using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCustomUI : MonoBehaviour
{
    [SerializeField, Tooltip("the main menu canvas")]
    private GameObject mainMenuCanvas;
    [SerializeField, Tooltip("the custom menu canvas")]
    private GameObject customMenuCanvas;

    [SerializeField, Tooltip("the custom menu file reader script")]
    private CustomLevelSelect customLevels;

    [SerializeField, Tooltip("the custom creation menu canvas")]
    private GameObject customCreationMenuCanvas;

    [SerializeField, Tooltip("the custom creation menu file reader script")]
    private CustomLevelSelect customCreateLevels;

    [SerializeField, Tooltip("the name of the level creation scene")]
    private string levelEditorSceneName = "Level Creation Scene";
    [SerializeField, Tooltip("the name of the main game scene")]
    private string mainGameSceneName = "MainGameScene";
    // Start is called before the first frame update
    public void OpenCustomMenu()
    {
        customMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        customLevels.LoadLevels();
    }

    public void CloseCustomMenu()
    {

        GameManager.Instance.SetCreationLevelFilename("");
        customMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        customLevels.ClearOptions();
    }

    public void CloseCreationMenu()
    {
        customCreationMenuCanvas.SetActive(false);
        customMenuCanvas.SetActive(true);
        customCreateLevels.ClearOptions();
    }

    public void OpenCreationMenu()
    {
        customCreationMenuCanvas.SetActive(true);
        customMenuCanvas.SetActive(false);
        customCreateLevels.LoadLevels();
    }

    public void CreateNewCustomLevel()
    {
        GameManager.Instance.SetCreationLevelFilename("");
        SceneManager.LoadScene(levelEditorSceneName);
    }

    public void LoadCustomLevelToEditor(string filename)
    {
        GameManager.Instance.SetCreationLevelFilename(filename);
        SceneManager.LoadScene(levelEditorSceneName);
    }

    public void LoadCustomLevelToRun(string filename)
    {
        GameManager.Instance.SetCreationLevelFilename(filename);
        GameManager.Instance.SetCurrentLevel(-1);
        SceneManager.LoadScene(mainGameSceneName);
    }

    void Start()
    {
        customCreationMenuCanvas.SetActive(false);
        customMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

}
