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
    [SerializeField, Tooltip("the top row. will use the last entry to fill all non-corner entries")]public List<Sprite> topRowSprites = new List<Sprite>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
