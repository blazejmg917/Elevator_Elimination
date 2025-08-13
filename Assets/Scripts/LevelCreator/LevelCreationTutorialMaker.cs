using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelCreationTutorialMaker : MonoBehaviour
{
    [SerializeField, Tooltip("if the tutorial message should be remade")] private bool overwriteMessage;
    [SerializeField, Tooltip("the file extension where this should be saved under. Appended to persistent data path")] private string dataPath;
    [SerializeField, TextArea, Tooltip("the text of the demo file")] private string fileText;


    private void Start()
    {
        MakeTutorialFile();
    }

    void MakeTutorialFile(bool overwrite = false)
    {
        string filepath = Application.persistentDataPath + dataPath;
        if(!overwrite && !overwriteMessage && System.IO.File.Exists(filepath))
        {
            return;
        }

        try
        {
            StreamWriter sw = new StreamWriter(filepath, false);
            sw.Write(fileText);
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("UNKNOWN ERROR WRITING TO FILE " + filepath + ": "+ e.Message);
        }
    }
}
