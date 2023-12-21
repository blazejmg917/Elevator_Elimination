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
    public static int NONEXISTENTFILE = 5;
    public static int INVALIDLEVELNAME = 6;
    public static int INVALIDCREATORNAME = 7;
    public static int INVALIDTILE = 8;
    public static int INVALIDDIRECTION = 9;
    public static int NOTARGETS = 10;
    public static int BLOCKEDDOORWAY = 11;


    [SerializeField, Tooltip("the filepath where the custom levels are stored. Added onto the end of the default application filepath")]
    private string folderFilePath = "/Custom/Levels";

    [SerializeField, Tooltip("the Tile prefab")]
    private GameObject tilePrefab;

    [SerializeField, Tooltip("the symbol that represents an empty tile")]
    private string EmptyTileMarker = "X";
    //marked true if actively reading from or writing to a file to avoid duplicates
    private bool handingReadWrite = false;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + folderFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("FAILED TO CREATE DIRECTORY " + Application.persistentDataPath + folderFilePath + ", " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool WriteToFile(LevelStructure level, out int errorCode, string fileName, bool overwrite = false)
    {
        int numTargets = 0;
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

        string filepath = Application.persistentDataPath + folderFilePath + "/" + fileName;
        Debug.Log("trying to write to filepath: " + filepath);
        if (!overwrite && System.IO.File.Exists(filepath))
        {
            errorCode = DUPLICATEFILENAME;
            Debug.LogWarning("DUPLICATE FILENAME WITHOUT OVERRIDE");
            return false;
        }

        string levelName = level.GetLevelName();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            return false;
        }

        string creatorName = level.GetCreator();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            return false;
        }

        string levelString;
        //try writing levelstructure to a string
        try
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(levelName);
            sb.Append("\n");
            sb.Append(creatorName);
            List<TileManager.ListWrapper<Tile>> tiles = level.GetTileList();
            if (tiles.Count != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                return false;
            }
            //check for open doorway
            if (tiles[3][6].GetPerson() != null)
            {
                errorCode = BLOCKEDDOORWAY;
                Debug.LogError("LEVEL HAS DOORWAY BLOCKED");
                return false;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Count != 7)
                {
                    errorCode = INVALIDLEVELSTRUCTURE;
                    Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                    return false;
                }
                sb.Append("\n");
                for (int j = 0; j < tiles[i].Count; j++)
                {
                    Tile tile = tiles[(tiles[i].Count - 1) - j][i];
                    if (!tile)
                    {
                        sb.Append(EmptyTileMarker);
                        sb.Append(";");
                        continue;
                    }

                    Person person = tile.GetPerson();
                    tile.GetPerson();
                    if (!person)
                    {
                        sb.Append(EmptyTileMarker);
                        sb.Append(";");
                        continue;
                    }

                    

                    if (string.IsNullOrWhiteSpace(person.GetKey()) || person.GetKey().Contains(";")  || person.GetKey().Contains(",") || person.GetKey().Contains("\n") || person.GetKey().Contains("<") || person.GetKey().Contains(">"))
                    {
                        sb.Clear();
                        errorCode = INVALIDPERSON;
                        Debug.LogError("INVALID PERSON AT POSITION " + j + "," + i);
                        return false;
                    }

                    if (person.IsTarget())
                    {
                        numTargets++;
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
            Debug.Log("writing level string: " + levelString);
            sw.Write(levelString);
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("UNKNOWN ERROR WRITING TO FILE " + e.Message);
            errorCode = UNKNOWNERROR;
            return false;
        }


        //check for valid targets
        if (numTargets < 1)
        {
            errorCode = NOTARGETS;
            Debug.LogError("LEVEL HAS NO TARGETS");
            return false;
        }

        return true;


    }

    public bool ReadFromFile(string filename, LevelStructure level, out int errorCode, bool editMode = false)
    {
        int numTargets = 0;
        errorCode = 0;
        if (filename == null || string.IsNullOrWhiteSpace(filename))
        {
            errorCode = INVALIDFILENAME;
            Debug.Log("can't read invalid filename of " + filename);
            return false;
        }

        string filepath = Application.persistentDataPath + folderFilePath + "/" + filename;
        Debug.Log("trying to read from filepath: " + filepath);
        if (!System.IO.File.Exists(filepath))
        {
            errorCode = NONEXISTENTFILE;
            Debug.Log("COULD NOT FIND FILE");
            return false;
        }

        //try reading from file
        List<TileManager.ListWrapper<Tile>> tileList = level.GetTileList();
        string fileString;
        try
        {
            StreamReader sr = new StreamReader(filepath);
            fileString = sr.ReadToEnd();
            sr.Close();
        }
        catch (Exception e)
        {
            errorCode = UNKNOWNERROR;
            Debug.Log("UNKNOWN ERROR TRYING TO READ FROM FILE");
            return false;
        }

        //try parsing data
        string[] fileStrings = fileString.Split("\n");

        if (fileStrings.Length != 9)
        {
            errorCode = INVALIDLEVELSTRUCTURE;
            Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
            return false;
        }

        string levelName = fileStrings[0].Trim();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            return false;
        }
        level.SetLevelName(levelName);

        string creatorName = fileStrings[1].Trim();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            return false;
        }
        level.SetCreatorName(creatorName);

        GameObject owner = level.gameObject;
        for (int i = 2; i < fileStrings.Length; i++)
        {
            string[] levelRow = fileStrings[i].Trim().Split(";");
            if (levelRow.Length != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                return false;
            }
            for (int j = 0; j < levelRow.Length; j++)
            {
                if (levelRow[j].ToUpper().Trim() == EmptyTileMarker)
                {
                    continue;
                }
                string[] tileStrings = levelRow[j].Split(",");
                if (tileStrings.Length != 2)
                {
                    errorCode = INVALIDTILE;
                    Debug.LogError("INVALID TILE STRING FOUND");
                    return false;
                }

                Tile tile = tileList[j][((fileStrings.Length - 1) - i)];
                string personId = TileManager.Instance.ConvertPersonKeyToID(tileStrings[1].Trim());
                if (personId == null)
                {
                    errorCode = INVALIDPERSON;
                    Debug.LogError("COULD NOT FIND ID TO MATCH KEY: " + tileStrings[0].Trim());
                    return false;
                }
                tile.SetPersonId(personId);
                if (TileManager.Instance.GetPersonFromID(personId).GetComponent<Person>().IsTarget())
                {
                    numTargets++;
                }

                Person.Direction dir;
                if (!GetDirection(tileStrings[1], out dir))
                {
                    errorCode = INVALIDTILE;
                    Debug.LogError("INVALID TILE DIRECTION FOUND");
                    return false;
                }
                tile.SetDirection(dir);
            }
        }

        //if level is to be loaded in to be played, ensure there is at least one target
        if (!editMode && numTargets <= 0)
        {
            errorCode = NOTARGETS;
            Debug.LogError("LEVEL HAS NO TARGETS");
            return false;
        }

        return true;
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
