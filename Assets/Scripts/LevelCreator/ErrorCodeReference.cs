using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorCodeReference : MonoBehaviour
{
    [System.Serializable]
    private struct ErrorCode
    {
        public int code;
        public string message;
    }
    [SerializeField, Tooltip("all error codes and their definitions")]private List<ErrorCode> errorCodes = new List<ErrorCode>();
    Dictionary<int, string> errorCodeMap = new Dictionary<int, string>();
    // Start is called before the first frame update
    void Start()
    {
        PopulateMap();
    }

    public string GetErrorMessage(int errorCode)
    {
        if (errorCodeMap.Count <= 0)
        {
            PopulateMap();
        }

        string errorMessage;
        if (errorCodeMap.TryGetValue(errorCode, out errorMessage))
        {
            return errorMessage;
        }
        else
        {
            return "";
        }
    }

    private void PopulateMap()
    {
        errorCodeMap = new Dictionary<int, string>();
        foreach (ErrorCode error in errorCodes)
        {
            errorCodeMap.Add(error.code, error.message);
        }
    }
}
