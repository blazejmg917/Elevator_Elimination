using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ElevatorIO : MonoBehaviour
{
    //error codes for reading/writing
    public static int UNKNOWNERROR = -1;
    public static int CLEAN = 0;
    public static int DUPLICATEFILENAME = 1;
    public static int INVALIDFILENAME = 2;
    public static int INVALIDLEVELSTRUCTURE = 3;
    public static int INVALIDPERSON = 4;


    [SerializeField, Tooltip("the filepath where the custom levels are stored. Added onto the end of the default application filepath")]
    private string folderFilePath = "/Custom/Levels/";
    //marked true if actively reading from or writing to a file to avoid duplicates
    private bool handingReadWrite = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool WriteToFile(LevelStructure level, out int errorCode, string fileName = null, bool overwrite = false)
    {
        errorCode = 0;
        if (!level)
        {
            errorCode = INVALIDLEVELSTRUCTURE;
            Debug.LogError("INVALID LEVEL STRUCTURE");
            return false;
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            errorCode = INVALIDFILENAME;
            Debug.LogError("LEVEL NAME IS EMPTY");
            return false;
        }

        string filepath = Application.persistentDataPath + folderFilePath + fileName;
        Debug.Log("trying to write to filepath: " + filepath);
        if (!overwrite && System.IO.File.Exists(filepath))
        {
            errorCode = DUPLICATEFILENAME;
            Debug.LogWarning("DUPLICATE FILENAME WITHOUT OVERRIDE");
            return false;
        }

        
        string levelString;
        //try writing levelstructure to a string
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(level.GetLevelName());
            sb.Append("\n");
            sb.Append(level.GetCreator());
            Tile[,] tiles = level.GetTiles();
            if (tiles.GetLength(0) != 7 || tiles.GetLength(1) != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                return false;
            }

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                sb.Append("\n");
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    Tile tile = tiles[i, j];
                    if (!tile)
                    {
                        sb.Append(" ;");
                        continue;
                    }

                    Person person = tile.GetPerson();
                    tile.GetPerson();
                    if (!person)
                    {
                        sb.Append(" ;");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(person.GetKey()) || person.GetKey().Contains(";")  || person.GetKey().Contains(",") || person.GetKey().Contains("\n") || person.GetKey().Contains("<") || person.GetKey().Contains(">"))
                    {
                        sb.Clear();
                        errorCode = INVALIDPERSON;
                        Debug.LogError("INVALID PERSON AT POSITION " + i + "," + j);
                        return false;
                    }

                    sb.Append(person.GetKey());
                    sb.Append(",");
                    sb.Append(GetDirectionSignifier(person.GetDirection()));
                    sb.Append(";");


                }
                
            }
            levelString = sb.ToString();
            sb.Clear();
        }
        catch (Exception e)
        {
            errorCode = UNKNOWNERROR;
            Debug.LogError("UNKNOWN ERROR BUILDING STRING");
            return false;
        }

        //now that the string has been written, write it to a file
        try
        {
            StreamWriter sw = new StreamWriter(filepath, false);
            sw.Write(levelString);
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("UNKNOWN ERROR WRITING TO FILE");
            errorCode = UNKNOWNERROR;
            return false;
        }

        return true;


    }

    public bool ReadFromFile(string filepath, LevelStructure level, out int errorCode)
    {
        errorCode = 0;
        return false;
        //levelTiles = new Tile[7,7];
    }

    public string GetDirectionSignifier(Person.Direction dir)
    {
        switch (dir)
        {
            case Person.Direction.DOWN:
                return "D";
            case Person.Direction.UP:
                return "U";
            case Person.Direction.LEFT:
                return "L";
            case Person.Direction.RIGHT:
                return "R";
            default:
                return "N";
        }
    }

    public bool GetDirection(string dirStr, out Person.Direction direction)
    {
        direction = Person.Direction.NONE;
        switch (dirStr.ToUpper())
        {
            case "D":
                direction = Person.Direction.DOWN;
                return true;
            case "U":
                direction = Person.Direction.UP;
                return true;
            case "L":
                direction = Person.Direction.LEFT;
                return true;
            case "R":
                direction = Person.Direction.RIGHT;
                return true;
            case "N":
                direction = Person.Direction.NONE;
                return true;
            default:
                return false;
        }
    }
}
