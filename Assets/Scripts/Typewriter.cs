using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class Typewriter : MonoBehaviour
{
    
    [Tooltip("Background color, used for hiding text")]
    public Color bgColor = Color.grey;
    //the string for starting the background color
    string colorString;
    //the string for ending the background color
    string colorEnd = "</color>";
    //the stringbuilder to use
    StringBuilder sb;
    //used to tell task to stop
    bool stop = false;
    //used to tell task that it should skip to the end of input
    bool skip = false;

    //whether the typewriter is currently typing or not
    bool typing = false;
    //program framerate
    int fps = 30;
    [Tooltip("should this run the test method")]
    public bool test;
    [Tooltip("the textbox to use")]
    public TMP_Text defaultTextBox;
    [Tooltip("test text")]
    public string textToType;
    [Tooltip("charPerSec")]
    public float charPerSec = 5f;
    // Start is called before the first frame update
    void Awake()
    {
        colorString = "<color=#" + ColorUtility.ToHtmlStringRGBA(bgColor) + ">";
        sb = new StringBuilder();
        fps = Application.targetFrameRate;
    }

    void Start(){
        if(test){
            StartCoroutine(Typewrite(defaultTextBox, textToType, charPerSec));
        }
    }


    //-----methods to start typing the text with various different sets of inputs.
    //type a message to a specific text box at a specific speed
    public void StartTyping(string twText, float cps, TMP_Text textbox){
        StartCoroutine(Typewrite(textbox, twText, cps));
    }

    //type to the default textbox at a particular speed
    public void StartTyping(string twText, float cps){
        StartCoroutine(Typewrite(defaultTextBox, twText, cps));
    }

    //type to a particulat textbox at the default speed
    public void StartTyping(string twText, TMP_Text textbox){
        StartCoroutine(Typewrite(textbox, twText, charPerSec));
    }

    //type to the default textbox at the default speed.
    public void StartTyping(string twText){
        StartCoroutine(Typewrite(defaultTextBox, twText, charPerSec));
    }



    public bool IsTyping()
    {
        return typing;
    }

    public void SkipText(){
        if(!typing){
            return;
        }
        StopText();
        skip = true;
    }

    public void setDefaultTextbox(TMP_Text newTextBox){
        defaultTextBox = newTextBox;
    }

    public bool TextBoxSet(){
        return defaultTextBox;
    }



    public void StopText()
    {
        stop = true;
    }

    //write text as typewriter
    public IEnumerator Typewrite(TMP_Text textBox, string text, float charPerSec)
    {
        //Debug.Log("typewriting!");
        if(textBox == null){
            textBox = defaultTextBox;
        }
        if(textBox == null){
            Debug.LogError("NO TEXTBOX SET FOR TYPEWRITER. CANNOT TYPE");
        }
        textBox.text = string.Empty;
        stop = false;
        typing = true;
        int count = 0;
        sb.Clear();
        while(count < text.Length)
        {
            sb.Clear();
            if (stop)
            {
                stop = false;
                if(skip){
                    sb.Append(colorString);
                    sb.Append(text);
                    sb.Append(colorEnd);
                    skip = false;
                }

                break;
            }
            
            sb.Append(text.Substring(0, count + 1));
            sb.Append(colorString);
            sb.Append(text.Substring(count));
            sb.Append(colorEnd);
            textBox.text = sb.ToString();
            count++;
            yield return new WaitForSeconds((1 / charPerSec));
        }
        textBox.text = text;
        typing = false;
        stop = false;
        skip = false;
    }

    //clear text
    public void Clear(Text textbox)
    {
        textbox.text = string.Empty;
    }

    public void Clear()
    {
        defaultTextBox.text = string.Empty;
    }
}
