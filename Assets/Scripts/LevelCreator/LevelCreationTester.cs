using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelCreationTester : MonoBehaviour
{
    [System.Serializable]public class LevelTestEvent : UnityEvent{};

    public LevelTestEvent startLevelTest = new LevelTestEvent();

    public LevelTestEvent endLevelTest = new LevelTestEvent();

    [SerializeField] private string savedLevelString;
    [SerializeField, Tooltip("the level creation ui handler")]private LevelCreationUiHandler uiHandler;
    [SerializeField] bool testing;
    [SerializeField, Tooltip("the testing text area script")] private LevelCreationTestingText testingText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting");
        
    }

    public void StartTesting()
    {
        Debug.Log("start test " + testing);
        if (testing)
        {
            return;
        }
        
        string levelString;
        int errorCode;
        if (!TileManager.Instance.TrySaveToString(out levelString, out errorCode))
        {
            uiHandler.DisplayError("Can't test level, error code " + errorCode + ": " + GameManager.Instance.GetErrorCodeMessage(errorCode));
            return;
        }
        uiHandler.DisplayError("");
        Debug.Log("testing begun on " + levelString);
        savedLevelString = levelString;
        testing = true;
        startLevelTest.Invoke();
    }
    

    public void StopTesting()
    {
        Debug.Log("Stop testing " + testing);
        if (!testing)
        {
            return;
        }

        if (!string.IsNullOrEmpty(savedLevelString))
        {
            int errorCode;
            if (!TileManager.Instance.TryLoadFromString(savedLevelString, out errorCode, true))
            {
                uiHandler.DisplayError("Can't load in level, error code " + errorCode + ": " + GameManager.Instance.GetErrorCodeMessage(errorCode));
            }
        }
        else
        {
            Debug.Log("exited test with an empty string");
        }

        savedLevelString = "";
        testing = false;
        endLevelTest.Invoke();
    }

    public void ResetTest()
    {
        Debug.Log("Resetting test");
        if (!testing)
        {
            return;
        }

        if (!string.IsNullOrEmpty(savedLevelString))
        {
            int errorCode;
            if (!TileManager.Instance.TryLoadFromString(savedLevelString, out errorCode, true))
            {
                uiHandler.DisplayError("Can't load in level, error code " + errorCode + ": " + GameManager.Instance.GetErrorCodeMessage(errorCode));
                StopTesting();
                return;
            }
            startLevelTest.Invoke();
        }
        else
        {
            StopTesting();
        }
    }
}
