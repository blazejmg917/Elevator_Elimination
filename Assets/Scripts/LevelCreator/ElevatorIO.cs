using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Unity.Collections;

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
    public static int INVALIDTURNCOUNT = 12;
    public static int BUSY = -2;


    [SerializeField, Tooltip("the filepath where the custom levels are stored. Added onto the end of the default application filepath")]
    private string folderFilePath = "/Custom/Levels";

    [SerializeField, Tooltip("the Tile prefab")]
    private GameObject tilePrefab;

    [SerializeField, Tooltip("the symbol that represents an empty tile")]
    private string EmptyTileMarker = "X";
    //marked true if actively reading from or writing to a file to avoid duplicates
    private bool handlingReadWrite = false;

    [SerializeField, Tooltip("the personHolder to use for prechecking")]
    private PersonHolder preCheckPHolder;
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

    public bool WriteToString(LevelStructure level, out string levelString, out int errorCode)
    {
        
        levelString = "";
        if (handlingReadWrite)
        {
            errorCode = BUSY;
            Debug.Log("IO OPERATION ALREADY IN PROGRESS");
            return false;
        }

        handlingReadWrite = true;
        string levelName = level.GetLevelName();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            handlingReadWrite = false;
            return false;
        }

        string creatorName = level.GetCreator();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            handlingReadWrite = false;
            return false;
        }

        string turnCount;
        try
        {
            int floors = level.GetFloors();
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + level.GetFloors());
                handlingReadWrite = false;
                return false;
            }
            turnCount = floors.ToString();
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + level.GetFloors());
            handlingReadWrite = false;
            return false;
        }
        int numTargets = 0;
        //string levelString;
        //try writing levelstructure to a string
        try
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(levelName);
            sb.Append("\n");
            sb.Append(creatorName);
            sb.Append("\n");
            sb.Append(turnCount);
            List<TileManager.ListWrapper<Tile>> tiles = level.GetTileList();
            if (tiles.Count != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                handlingReadWrite = false;
                return false;
            }
            //check for open doorway
            if (tiles[3][6].GetPerson() != null)
            {
                errorCode = BLOCKEDDOORWAY;
                Debug.LogError("LEVEL HAS DOORWAY BLOCKED");
                handlingReadWrite = false;
                return false;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Count != 7)
                {
                    errorCode = INVALIDLEVELSTRUCTURE;
                    Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                    handlingReadWrite = false;
                    return false;
                }
                sb.Append("\n");
                for (int j = 0; j < tiles[i].Count; j++)
                {
                    Tile tile = tiles[j][(tiles.Count - 1) - i];
                    if (!tile)
                    {
                        sb.Append(EmptyTileMarker);
                        if (j != tiles[i].Count - 1)
                            sb.Append(";");

                        continue;
                    }

                    Person person = tile.GetPerson();
                    tile.GetPerson();
                    if (!person)
                    {
                        sb.Append(EmptyTileMarker);
                        if (j != tiles[i].Count - 1)
                            sb.Append(";");
                        continue;
                    }



                    if (string.IsNullOrWhiteSpace(person.GetKey()) || person.GetKey().Contains(";") || person.GetKey().Contains(",") || person.GetKey().Contains("\n") || person.GetKey().Contains("<") || person.GetKey().Contains(">"))
                    {
                        sb.Clear();
                        errorCode = INVALIDPERSON;
                        Debug.LogError("INVALID PERSON AT POSITION " + j + "," + i);
                        handlingReadWrite = false;
                        return false;
                    }

                    if (person.IsTarget())
                    {
                        numTargets++;
                    }

                    sb.Append(person.GetKey());
                    sb.Append(",");
                    sb.Append(GetDirectionSignifier(person.GetDirection()));
                    if (j != tiles[i].Count - 1)
                        sb.Append(";");


                }

            }

            //check for valid targets
            if (numTargets < 1)
            {
                errorCode = NOTARGETS;
                Debug.LogError("LEVEL HAS NO TARGETS");
                sb.Clear();
                handlingReadWrite = false;
                return false;
            }

            levelString = sb.ToString();
            sb.Clear();
        }
        catch (Exception e)
        {
            errorCode = UNKNOWNERROR;
            Debug.LogError("UNKNOWN ERROR BUILDING STRING");
            handlingReadWrite = false;
            return false;
        }
        errorCode = 0;
        handlingReadWrite = false;
        return true;
    }

    public bool WriteToFile(LevelStructure level, out int errorCode, string fileName = "", bool overwrite = false)
    {
        if (handlingReadWrite)
        {
            errorCode = BUSY;
            Debug.Log("IO OPERATION ALREADY IN PROGRESS");
            return false;
        }
        handlingReadWrite = true;
        if (fileName == "")
        {
            fileName = level.GetLevelName();
        }
        int numTargets = 0;
        errorCode = 0;
        if (!level)
        {
            errorCode = INVALIDLEVELSTRUCTURE;
            Debug.LogError("INVALID LEVEL STRUCTURE");
            handlingReadWrite = false;
            return false;
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            errorCode = INVALIDFILENAME;
            Debug.LogError("LEVEL NAME IS EMPTY");
            handlingReadWrite = false;
            return false;
        }

        string filepath = Application.persistentDataPath + folderFilePath + "/" + fileName;
        Debug.Log("trying to write to filepath: " + filepath);
        if (!overwrite && System.IO.File.Exists(filepath))
        {
            errorCode = DUPLICATEFILENAME;
            Debug.LogWarning("DUPLICATE FILENAME WITHOUT OVERRIDE");
            handlingReadWrite = false;
            return false;
        }

        string levelName = level.GetLevelName();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            handlingReadWrite = false;
            return false;
        }

        string creatorName = level.GetCreator();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            handlingReadWrite = false;
            return false;
        }

        string turnCount;
        try
        {
            int floors = level.GetFloors();
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + level.GetFloors());
                handlingReadWrite = false;
                return false;
            }
            turnCount = floors.ToString();
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + level.GetFloors());
            handlingReadWrite = false;
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
            sb.Append("\n");
            sb.Append(turnCount);
            List<TileManager.ListWrapper<Tile>> tiles = level.GetTileList();
            if (tiles.Count != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                handlingReadWrite = false;
                return false;
            }
            //check for open doorway
            if (tiles[3][6].GetPerson() != null)
            {
                errorCode = BLOCKEDDOORWAY;
                Debug.LogError("LEVEL HAS DOORWAY BLOCKED");
                handlingReadWrite = false;
                return false;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Count != 7)
                {
                    errorCode = INVALIDLEVELSTRUCTURE;
                    Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
                    handlingReadWrite = false;
                    return false;
                }
                sb.Append("\n");
                for (int j = 0; j < tiles[i].Count; j++)
                {
                    Tile tile = tiles[j][(tiles.Count - 1) - i];
                    if (!tile)
                    {
                        sb.Append(EmptyTileMarker);
                        if(j != tiles[i].Count - 1)
                            sb.Append(";");
                        
                        continue;
                    }

                    Person person = tile.GetPerson();
                    tile.GetPerson();
                    if (!person)
                    {
                        sb.Append(EmptyTileMarker);
                        if (j != tiles[i].Count - 1)
                            sb.Append(";");
                        continue;
                    }

                    

                    if (string.IsNullOrWhiteSpace(person.GetKey()) || person.GetKey().Contains(";")  || person.GetKey().Contains(",") || person.GetKey().Contains("\n") || person.GetKey().Contains("<") || person.GetKey().Contains(">"))
                    {
                        sb.Clear();
                        errorCode = INVALIDPERSON;
                        Debug.LogError("INVALID PERSON AT POSITION " + j + "," + i);
                        handlingReadWrite = false;
                        return false;
                    }

                    if (person.IsTarget())
                    {
                        numTargets++;
                    }

                    sb.Append(person.GetKey());
                    sb.Append(",");
                    sb.Append(GetDirectionSignifier(person.GetDirection()));
                    if (j != tiles[i].Count - 1)
                        sb.Append(";");


                }
                
            }

            //check for valid targets
            if (numTargets < 1)
            {
                errorCode = NOTARGETS;
                Debug.LogError("LEVEL HAS NO TARGETS");
                sb.Clear();
                handlingReadWrite = false;
                return false;
            }

            levelString = sb.ToString();
            sb.Clear();
        }
        catch (Exception e)
        {
            errorCode = UNKNOWNERROR;
            Debug.LogError("UNKNOWN ERROR BUILDING STRING");
            handlingReadWrite = false;
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
            handlingReadWrite = false;
            return false;
        }


        handlingReadWrite = false;

        return true;


    }

    public bool ReadFilePrecheck(string filename, out int errorCode, out string _levelName, out string _creatorName, bool editMode = false)
    {
        _levelName = "";
        _creatorName = "";
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
            Debug.Log("COULD NOT FIND FILE " + filepath);
            return false;
        }

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
        Debug.Log("read: " + fileString);
        //try parsing data
        string[] fileStrings = fileString.Split("\n");
        Debug.Log("split: " + fileStrings.Length);
        if (fileStrings.Length != 10)
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
        _levelName = levelName;

        string creatorName = fileStrings[1].Trim();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            return false;
        }
        _creatorName = creatorName;

        string levelCount = fileStrings[2].Trim();
        if (levelCount == null || string.IsNullOrWhiteSpace(levelCount))
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            return false;
        }

        int floors;
        try
        {
            floors = int.Parse(levelCount);
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + floors);
                return false;
            }
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            return false;
        }
        
        return true;
    }

    public bool ReadFileFullCheck(string filename, out int errorCode, bool editMode = false)
    {
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
        Debug.Log("read: " + fileString);
        //try parsing data
        string[] fileStrings = fileString.Split("\n");
        Debug.Log("split: " + fileStrings.Length);
        if (fileStrings.Length != 10)
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

        string creatorName = fileStrings[1].Trim();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            return false;
        }

        string levelCount = fileStrings[2].Trim();
        if (levelCount == null || string.IsNullOrWhiteSpace(levelCount))
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            return false;
        }

        int floors;
        try
        {
            floors = int.Parse(levelCount);
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + floors);
                return false;
            }
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            return false;
        }

        int numTargets = 0;

        for (int i = 3; i < fileStrings.Length; i++)
        {
            Debug.Log("row: " + fileStrings[i].Trim());
            string[] levelRow = fileStrings[i].Trim().Split(";");
            if (levelRow.Length != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE: " + levelRow.Length);
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

                //Tile tile = tileList[j][((fileStrings.Length - 1) - i)];
                string personId = preCheckPHolder.GetIdByKey(tileStrings[0].Trim());//.Instance.ConvertPersonKeyToID(tileStrings[0].Trim());
                if (personId == null)
                {
                    errorCode = INVALIDPERSON;
                    Debug.LogError("COULD NOT FIND ID TO MATCH KEY: " + tileStrings[0].Trim());
                    return false;
                }
                //tile.SetPersonId(personId);
                if(preCheckPHolder.GetPersonById(personId).GetComponent<Person>().IsTarget())//if (TileManager.Instance.GetPersonFromID(personId).GetComponent<Person>().IsTarget())
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
            }
        }

        //if level is to be loaded in to be played, ensure there is at least one target
        if (!editMode && numTargets <= 0 && !editMode)
        {
            errorCode = NOTARGETS;
            Debug.LogError("LEVEL HAS NO TARGETS");
            return false;
        }

        return true;
    }


    public bool ReadFromString(string levelString, LevelStructure level, out int errorCode, bool editMode = false)
    {
        if (handlingReadWrite)
        {
            errorCode = BUSY;
            Debug.Log("IO OPERATION ALREADY IN PROGRESS");
            return false;
        }
        
        handlingReadWrite = true;
        int numTargets = 0;
        Debug.Log("read: " + levelString);
        //try parsing data
        string[] fileStrings = levelString.Split("\n");
        List<TileManager.ListWrapper<Tile>> tileList = level.GetTileList();
        Debug.Log("split: " + fileStrings.Length);
        if (fileStrings.Length != 10)
        {
            errorCode = INVALIDLEVELSTRUCTURE;
            Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
            handlingReadWrite = false;
            return false;
        }

        string levelName = fileStrings[0].Trim();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            handlingReadWrite = false;
            return false;
        }
        level.SetLevelName(levelName);

        string creatorName = fileStrings[1].Trim();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            handlingReadWrite = false;
            return false;
        }
        level.SetCreatorName(creatorName);

        string levelCount = fileStrings[2].Trim();
        if (levelCount == null || string.IsNullOrWhiteSpace(levelCount))
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            handlingReadWrite = false;
            return false;
        }

        int floors;
        try
        {
            floors = int.Parse(levelCount);
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + floors);
                handlingReadWrite = false;
                return false;
            }
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            handlingReadWrite = false;
            return false;
        }

        level.SetFloors(floors);

        GameObject owner = level.gameObject;
        for (int i = 3; i < fileStrings.Length; i++)
        {
            Debug.Log("row: " + fileStrings[i].Trim());
            string[] levelRow = fileStrings[i].Trim().Split(";");
            if (levelRow.Length != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE: " + levelRow.Length);
                handlingReadWrite = false;
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
                    handlingReadWrite = false;
                    return false;
                }

                Tile tile = tileList[j][((fileStrings.Length - 1) - i)];
                string personId = TileManager.Instance.ConvertPersonKeyToID(tileStrings[0].Trim());
                if (personId == null)
                {
                    errorCode = INVALIDPERSON;
                    Debug.LogError("COULD NOT FIND ID TO MATCH KEY: " + tileStrings[0].Trim());
                    handlingReadWrite = false;
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
                    handlingReadWrite = false;
                    return false;
                }
                tile.SetDirection(dir);
            }
        }

        //if level is to be loaded in to be played, ensure there is at least one target
        if (!editMode && numTargets <= 0 && !editMode)
        {
            errorCode = NOTARGETS;
            Debug.LogError("LEVEL HAS NO TARGETS");
            handlingReadWrite = false;
            return false;
        }

        handlingReadWrite = false;
        errorCode = 0;
        return true;

    }

    public bool ReadFromFile(string filename, LevelStructure level, out int errorCode, bool editMode = false)
    {
        if (handlingReadWrite)
        {
            errorCode = BUSY;
            Debug.Log("IO OPERATION ALREADY IN PROGRESS");
            return false;
        }
        handlingReadWrite = true;
        int numTargets = 0;
        errorCode = 0;
        if (filename == null || string.IsNullOrWhiteSpace(filename))
        {
            errorCode = INVALIDFILENAME;
            Debug.Log("can't read invalid filename of " + filename);
            handlingReadWrite = false;
            return false;
        }

        string filepath = Application.persistentDataPath + folderFilePath + "/" + filename;
        Debug.Log("trying to read from filepath: " + filepath);
        if (!System.IO.File.Exists(filepath))
        {
            errorCode = NONEXISTENTFILE;
            Debug.Log("COULD NOT FIND FILE");
            handlingReadWrite = false;
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
            handlingReadWrite = false;
            return false;
        }
        Debug.Log("read: " + fileString);
        //try parsing data
        string[] fileStrings = fileString.Split("\n");
        Debug.Log("split: " + fileStrings.Length);
        if (fileStrings.Length != 10)
        {
            errorCode = INVALIDLEVELSTRUCTURE;
            Debug.LogError("INVALID LEVEL STRUCTURE SIZE");
            handlingReadWrite = false;
            return false;
        }

        string levelName = fileStrings[0].Trim();
        if (levelName == null || string.IsNullOrWhiteSpace(levelName))
        {
            errorCode = INVALIDLEVELNAME;
            Debug.LogError("INVALID LEVEL NAME: " + levelName);
            handlingReadWrite = false;
            return false;
        }
        level.SetLevelName(levelName);

        string creatorName = fileStrings[1].Trim();
        if (creatorName == null || string.IsNullOrWhiteSpace(creatorName))
        {
            errorCode = INVALIDCREATORNAME;
            Debug.LogError("INVALID CREATOR NAME: " + creatorName);
            handlingReadWrite = false;
            return false;
        }
        level.SetCreatorName(creatorName);

        string levelCount = fileStrings[2].Trim();
        if (levelCount == null || string.IsNullOrWhiteSpace(levelCount))
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            handlingReadWrite = false;
            return false;
        }

        int floors;
        try
        {
            floors = int.Parse(levelCount);
            if (floors < 0 || floors > 99)
            {
                errorCode = INVALIDTURNCOUNT;
                Debug.LogError("INVALID TURN COUNT: " + floors);
                handlingReadWrite = false;
                return false;
            }
        }
        catch (Exception e)
        {
            errorCode = INVALIDTURNCOUNT;
            Debug.LogError("INVALID TURN COUNT: " + levelCount);
            handlingReadWrite = false;
            return false;
        }

        level.SetFloors(floors);

        GameObject owner = level.gameObject;
        for (int i = 3; i < fileStrings.Length; i++)
        {
            Debug.Log("row: " + fileStrings[i].Trim());
            string[] levelRow = fileStrings[i].Trim().Split(";");
            if (levelRow.Length != 7)
            {
                errorCode = INVALIDLEVELSTRUCTURE;
                Debug.LogError("INVALID LEVEL STRUCTURE SIZE: " + levelRow.Length);
                handlingReadWrite = false;
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
                    handlingReadWrite = false;
                    return false;
                }

                Tile tile = tileList[j][((fileStrings.Length - 1) - i)];
                string personId = TileManager.Instance.ConvertPersonKeyToID(tileStrings[0].Trim());
                if (personId == null)
                {
                    errorCode = INVALIDPERSON;
                    Debug.LogError("COULD NOT FIND ID TO MATCH KEY: " + tileStrings[0].Trim());
                    handlingReadWrite = false;
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
                    handlingReadWrite = false;
                    return false;
                }
                tile.SetDirection(dir);
            }
        }

        //if level is to be loaded in to be played, ensure there is at least one target
        if (!editMode && numTargets <= 0 && !editMode)
        {
            errorCode = NOTARGETS;
            Debug.LogError("LEVEL HAS NO TARGETS");
            handlingReadWrite = false;
            return false;
        }

        handlingReadWrite = false;
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

    public List<string[]> GetCustomFiles(bool editMode)
    {
        string directory = Application.persistentDataPath + folderFilePath;
        string[] files = Directory.GetFiles(directory);
        List<string[]> finalFiles = new List<string[]>();
        foreach (string file in files)
        {
            int errorCode;
            string[] thisFile = new string[3];
            thisFile[0] = System.IO.Path.GetFileName(file);
            if (ReadFilePrecheck(thisFile[0], out errorCode, out thisFile[1], out thisFile[2], editMode))
            {
                finalFiles.Add(thisFile);
            }
        }

        return finalFiles;
    }
}
