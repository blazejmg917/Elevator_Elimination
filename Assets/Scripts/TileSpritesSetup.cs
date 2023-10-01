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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
