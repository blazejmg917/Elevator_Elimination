using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class TileManager : MonoBehaviour
{
    [System.Serializable]
    public class TurnChangeEvent : UnityEvent<int>{};
    [System.Serializable]
    public class ListWrapper<T>{
        public List<T> list = new List<T>();
        public T this[int key]
        {
            get
            {
                return list[key];
            }
            set
            {
                list[key] = value;
            }
        }
        public int Count{
            get
            {
                return list.Count;
            }
        }
        public void Add(T val){
            list.Add(val);
        }
    }
    [Header("Tiles")]
    [SerializeField, Tooltip("the array of all current tiles")] private Tile[,] tiles;
    [SerializeField, Tooltip("the lists of all current tiles")]private List<ListWrapper<Tile>> tilesList = new List<ListWrapper<Tile>>();
    [Header("other objects")]
    [SerializeField, Tooltip("the tile prefab")] private GameObject tilePrefab;
    [SerializeField, Tooltip("tile holder object")] private GameObject tileHolder;
    [SerializeField, Tooltip("person holder object")]private GameObject personHolder;
    [SerializeField, Tooltip("the tile sprite setup script")]private TileSpritesSetup spritesSetup;
    [Header("tile fields")]
    [SerializeField, Tooltip("the size of each tile object. only used if there's no collider on tiles")] private Vector3 tileSize;
    private Vector3 realTileSize;
    [SerializeField, Tooltip("the starting position of the first tile")] private Vector3 tileStart;
    [SerializeField, Tooltip("the offset for objects over tiles")]private Vector3 tileOffset;
    [SerializeField, Tooltip("the tile structure file to load in")] private LevelStructure baseLevel;
    [SerializeField, Tooltip("the starting tile for the player to enter and exit from")] private Tile startTile;
    [SerializeField, Tooltip("event played every time a turn changes")]private TurnChangeEvent turnChangeEvent = new TurnChangeEvent();
    

    private static TileManager _instance;
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TileManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<TileManager>();
                    Debug.Log("Generating new game manager");
                }
            }
            return _instance;
        }
    }

    public void Start(){
        realTileSize = tileSize;
        if(tilePrefab && tilePrefab.GetComponent<BoxCollider2D>()){
            realTileSize = tilePrefab.GetComponent<BoxCollider2D>().size;
        }
        if(!spritesSetup){
            spritesSetup = FindObjectOfType<TileSpritesSetup>();
        }
    }

    public void LoadLevelList(LevelStructure levelStructure, bool setStructureAsTileHolder = false)
    {
        
        
        if (!setStructureAsTileHolder)
        {
            Debug.LogWarning("tried to overwrite data in inspector");
            return;
            // if(baseLevel){
            //     DestroyImmediate(baseLevel.gameObject);
            // }
            // tileHolder = Instantiate(levelStructure.gameObject, transform);
            // baseLevel = tileHolder.GetComponent<LevelStructure>();

        }
        ClearLevel();
        tileHolder = Instantiate(levelStructure.gameObject, transform);
        tileHolder.transform.position = new Vector3(transform.position.x,transform.position.y, tileHolder.transform.position.z);
        baseLevel = tileHolder.GetComponent<LevelStructure>();
        LoadLevel(baseLevel.GetTileList());
        GameManager.Instance.SetFloors(baseLevel.GetFloors(), baseLevel.GetFloors());
    }
    public void LoadLevel(List<ListWrapper<Tile>> tileList)
    {
        this.tilesList = tileList;
        VerifyLevel();
        LinkTileList();
        GetTilePeople();
        turnChangeEvent.Invoke(baseLevel.GetFloors());
    }

    public void VerifyLevel(){
        for (int i = 0; i < tilesList.Count; i++)
        {
            for (int j = 0; j < tilesList[i].Count; j++)
            {
                if(!tilesList[i][j]){
                    tilesList[i][j] = tileHolder.transform.GetChild((7 * i) + j).GetComponent<Tile>();
                }
            }
        }
    }

    public void GetTilePeople(){
        //Debug.Log("looking for tile people");
        //personHolder = PersonManager.Instance.GetPHolder().gameObject;
        //personHolder.transform.parent = transform;
        personHolder = PersonManager.Instance.GetPHolder().gameObject;
        PersonHolder pHolder = personHolder.GetComponent<PersonHolder>();
        pHolder.UpdateMap();
        //Debug.Log("tile lists " + tilesList.Count +", "+ tilesList[0].Count);
        for(int i = 0; i < tilesList.Count; i++){
            for(int j = 0; j < tilesList[i].Count; j++){
                Tile thisTile = tilesList[i][j];
                if(thisTile.GetPersonId() != null && thisTile.GetPersonId() != ""){
                    //Debug.Log("looking for person " + thisTile.GetPersonId());
                    
                    GameObject personPrefab = pHolder.GetPersonById(thisTile.GetPersonId());
                    if(personPrefab){
                        GameObject thisPerson = Instantiate(personPrefab, personHolder.transform);
                        //Person personScript = thisPerson.GetComponent<Person>()
                        //thisPerson.transform.parent = personHolder.transform;
                        thisPerson.transform.position = thisTile.GetPersonLocation() + new Vector3(0,0,-.001f * (tilesList[i].Count - j));
                        Person thisPersonScript = thisPerson.GetComponent<Person>();
                        thisPersonScript.SetCurrentTile(thisTile);
                        thisPersonScript.SetDirection(thisTile.GetComponent<Tile>().GetDirection());

                        thisTile.SetPerson(thisPerson.GetComponent<Person>());
                    }
                }
                
                //tilesList[i][j]
            }
        }
    }
    
    public void LinkTileList()
    {
       // Debug.Log("linking tiles: " + tilesList.Count + ", " + tilesList[0].Count);
        for (int i = 0; i < tilesList.Count; i++)
        {
            for (int j = 0; j < tilesList[i].Count; j++)
            {
                if(i > 0)
                {
                    //Debug.Log("top: "+ i +", " + j);
                    tilesList[i][j].SetLeft(tilesList[i - 1][ j]);
                }
                if (i < tilesList.Count - 1) 
                {
                    //Debug.Log("bottom: "+ i +", " + j);
                    tilesList[i][j].SetRight(tilesList[i + 1][ j]);
                }

                if (j > 0)
                {
                    //Debug.Log("left: "+ i +", " + j);
                    tilesList[i][j].SetBottom(tilesList[i][ j-1]);
                }
                if (j < tilesList[i].Count - 1)
                {
                    //Debug.Log("right: "+ i +", " + j);
                    tilesList[i][j].SetTop(tilesList[i][ j+1]);

                }
                tilesList[i][j].SetOffset(tileOffset);
            }
        }
        startTile = tilesList[tilesList.Count / 2][ tilesList[0].Count-1];
        SetupTileSprites();
    }

    private Tile GetTile(int x, int y)
    {
        return tilesList[x][y];
    }
    private Tile GetTile(int[] coords)
    {
        return GetTile(coords[0], coords[1]);
    }

    public void SetupElevatorList(int xSize, int ySize, bool overridePrevVals)
    {
        if(tilePrefab && tilePrefab.GetComponent<BoxCollider2D>()){
            realTileSize = tilePrefab.GetComponent<BoxCollider2D>().size;
        }
        else{
            realTileSize = tileSize;
        }
        if (tilesList != null)
        {
            if (!overridePrevVals)
            {
                Debug.LogWarning("Trying to override previous values through inspector");
                return;
            }
            else
            {
                ClearLevel();
            }
        }

        if (!tileHolder)
        {
            tileHolder = new GameObject("Tile Holder");
            tileHolder.transform.parent = transform;
        }

        tilesList = new List<ListWrapper<Tile>>();
        for(int i = 0; i < xSize; i++)
        {
            tilesList.Add(new ListWrapper<Tile>());
            for(int j = 0; j < ySize; j++)
            {
                GameObject newTileObj = Instantiate(tilePrefab, new Vector3(tileStart.x + realTileSize.x * i, tileStart.y + realTileSize.y * j, tileStart.z), tilePrefab.transform.rotation, tileHolder.transform);
                newTileObj.name = "Tile " + i + ", " + j;
                Tile newTile = newTileObj.GetComponent<Tile>();
                if (!newTile)
                {
                    newTile = newTileObj.AddComponent<Tile>();
                }
                tilesList[i].Add(newTile);
            }
        }
        Debug.Log("completed list");
        SaveLevelToStructureList(overridePrevVals);
        LinkTileList();
    }

    private void SaveLevelToStructureList(bool overridePrevVals)
    {
        if (!tileHolder)
        {
            tileHolder = new GameObject();
            baseLevel = tileHolder.GetComponent<LevelStructure>();
            if (!baseLevel)
            {
                baseLevel = tileHolder.AddComponent<LevelStructure>();
            }
        }
        if (!baseLevel)
        {
            baseLevel = tileHolder.GetComponent<LevelStructure>();
            if (!baseLevel)
            {
                baseLevel = tileHolder.AddComponent<LevelStructure>();
            }
        }
        if (baseLevel.HasTiles() && !overridePrevVals)
        {
            
            Debug.LogWarning("Trying to override previous values through inspector");
            return;
            
        }
        else
        {
            baseLevel.SetTiles(tilesList);
        }
    #if UNITY_EDITOR
    EditorUtility.SetDirty(baseLevel);
    #endif
    }

    private void ClearLevel()
    {
        if (tiles != null && tiles.Length > 0)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j])
                    {
                        DestroyImmediate(tiles[i, j].gameObject);
                    }
                }
            }
        }
        if(tilesList != null){
            for(int i = 0; i < tilesList.Count; i++){
                for(int j = 0; j < tilesList[i].Count; j++){
                    if(tilesList[i][j]){
                        //Debug.Log("destroying: " + tilesList[i][j].name);
                        if(!tilesList[i][j].GetComponent<TileSpritesSetup>() && tilesList[i][j].name != "Tile Sprites"){
                            DestroyImmediate(tilesList[i][j],false);
                        }
                    }
                }
                
            }
        }

        //les = null;
        if (tileHolder)
        {
            foreach (Transform t in tileHolder.transform.GetComponentsInChildren<Transform>())
            {
                if(t && !t.GetComponent<TileSpritesSetup>() && t.name != "Tile Sprites"){
                    //Debug.Log("destroying: " + t.name);
                    DestroyImmediate(t.gameObject);
                }
            }
            if(tileHolder&& !tileHolder.GetComponent<TileSpritesSetup>() && tileHolder.name != "Tile Sprites"){
                DestroyImmediate(tileHolder,true);
            }
            
        }
        if(personHolder){
            foreach (Transform t in personHolder.transform.GetComponentsInChildren<Transform>())
            {
                if(t && t != personHolder.transform){
                    DestroyImmediate(t.gameObject,true);
                }
            }
            
        }
        foreach (Transform t in transform)
            {
                if(t && !t.GetComponent<PersonManager>() && !t.GetComponent<TileManager>() && !t.GetComponent<TileSpritesSetup>()){
                    DestroyImmediate(t.gameObject,true);
                }
            }
        
        #if UNITY_EDITOR
        if(baseLevel){
            EditorUtility.SetDirty(baseLevel);
        }
    #endif
    }

    public bool UpdateLevel()
    {
        bool gameStillRunning = true;
        //Debug.Log(tilesList.Count);
        for (int i = 0; i < tilesList.Count; i++)
        {
            //Debug.Log(i);
            for (int j = 0; j < tilesList[i].Count; j++)
            {
                //Debug.Log(j);
                Person tilePerson = tilesList[i][j].GetPerson();
                if (tilePerson)
                {
                    if (!tilePerson.OnFloorChange())
                    {
                        gameStillRunning = false;
                    }
                    
                }

            }
        }
        int floor = GameManager.Instance.ChangeFloor();
        if(floor >= 0){
            turnChangeEvent.Invoke(floor);
        }
        return gameStillRunning;
    }

    public Tile GetStartTile()
    {
        return startTile;
    }

    private void SetupTileSprites(){
        if(!spritesSetup){
            spritesSetup = GetComponentInChildren<TileSpritesSetup>();
            if(!spritesSetup){
                return;
            }
        }
        spritesSetup.UpdateSprites(tilesList);
    }
}
