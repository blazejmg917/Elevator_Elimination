using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpritesSetup : MonoBehaviour
{
    // public enum SpriteType{
    //     ONETILE,
    //     WHOLECOLUMN,
    //     WHOLEROW,
    //     DEFAULT
    // }
    // [System.Serializable]public struct tileStore{
    //     [Tooltip("the type of placement this sprite uses")]public SpriteType placeMentType;
    //     [Tooltip("the x vals to use for this sprite")]public int xPos;
    //     [Tooltip("the y vals to use for this sprite")]public int yPos;
    //     [Tooltip("the actual sprite")]public Sprite sprite;
    // }
    [SerializeField,Tooltip("top left sprite")] public Sprite tl;
    [SerializeField,Tooltip("top right sprite")] public Sprite tr;
    [SerializeField,Tooltip("bottom left sprite")] public Sprite bl;
    [SerializeField,Tooltip("bottom right sprite")] public Sprite br;
    [SerializeField, Tooltip("the top row default sprite")]public Sprite topRowSprites;
    [SerializeField, Tooltip("the top row leftmost sprite")]public Sprite topRowLeftSprite;
    [SerializeField, Tooltip("the top row rightmost sprite")]public Sprite topRowRightSprite;
    [SerializeField, Tooltip("the bottom row default sprite")]public Sprite bottomRowSprites;
    [SerializeField, Tooltip("the bottom row leftmost sprite")]public Sprite bottomRowLeftSprite;
    [SerializeField, Tooltip("the bottom row rightmost sprite")]public Sprite bottomRowRightSprite;
    [SerializeField, Tooltip("the left column  default sprite")]public Sprite leftColSprites;
    [SerializeField, Tooltip("the left column topmost sprite")]public Sprite leftColTopSprite;
    [SerializeField, Tooltip("the left column bottommost sprite")]public Sprite leftColBottomSprite;
    [SerializeField, Tooltip("the right column default sprite")]public Sprite rightColSprites;
    [SerializeField, Tooltip("the right column topmost sprite")]public Sprite rightColTopSprite;
    [SerializeField, Tooltip("the right column bottommost sprite")]public Sprite rightColBottomSprite;
    [SerializeField, Tooltip("the default sprite")]public Sprite defaultSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSprites(List<TileManager.ListWrapper<Tile>> allTiles){
        int maxX = allTiles.Count - 1;
        int maxY = allTiles[0].Count - 1;
        for(int i = 0; i < allTiles.Count; i++){
            for(int j = 0; j < allTiles[i].Count;j++){
                Sprite newTileSprite = defaultSprite;
                //corners
                if(i == 0 && j == 0){
                    newTileSprite = bl;
                }
                else if(i == 0 && j == maxY){
                    newTileSprite = tl;
                }
                else if(i == maxX && j == 0){
                    newTileSprite = br;
                }
                else if(i == maxX && j == maxY){
                    newTileSprite = tr;
                }
                //bottom row
                else if(i == 1 && j == 0){
                    newTileSprite = bottomRowLeftSprite;
                }
                else if(i == maxX - 1 && j == 0){
                    newTileSprite = bottomRowRightSprite;
                }
                else if(j == 0){
                    newTileSprite = bottomRowSprites;
                    allTiles[i][j].transform.rotation = Quaternion.Euler(0,0,-90);
                }
                //top row
                else if(i == 1 && j == maxY){
                    newTileSprite = topRowLeftSprite;
                }
                else if(i == maxX - 1 && j == maxY){
                    newTileSprite = topRowRightSprite;
                }
                else if(j == maxY){
                    newTileSprite = topRowSprites;
                }
                //left
                else if(i == 0 && j == 1){
                    newTileSprite = leftColBottomSprite;
                }
                else if(i == 0 && j == maxY - 1){
                    newTileSprite = leftColTopSprite;
                    allTiles[i][j].transform.rotation = Quaternion.Euler(0,0,90);
                }
                else if(i == 0){
                    newTileSprite = leftColSprites;
                    allTiles[i][j].transform.rotation = Quaternion.Euler(0,0,90);
                    Debug.Log("tile rotated: " + allTiles[i][j].transform.rotation.eulerAngles + ", " + allTiles[i][j].gameObject.name);
                }
                //right
                else if(i == maxX && j == 1){
                    newTileSprite = rightColBottomSprite;
                }
                else if(i == maxX && j == maxY - 1){
                    newTileSprite = rightColTopSprite;

                }
                else if(i == maxX){
                    newTileSprite = rightColSprites;
                }
                allTiles[i][j].SetSprite(newTileSprite);
            }
        }
    }
}
