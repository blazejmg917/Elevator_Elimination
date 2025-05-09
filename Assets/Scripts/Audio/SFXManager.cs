using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public FMODUnity.EventReference stab;
    public FMODUnity.EventReference recordScratch;

    // Basic In-Game SFX
    public FMODUnity.EventReference push;

    // this is not exactly useful but we could do something like it to make the process of adding new sounds easier
    //public FMODUnity.EventReference Push
    //{
    //    get { return push; }
    //    set { OneShotSFX(push); }
    //}
    public FMODUnity.EventReference tap;
    public FMODUnity.EventReference rotate;
    public FMODUnity.EventReference scream;
    public FMODUnity.EventReference bell;
    public FMODUnity.EventReference huh;
    public FMODUnity.EventReference guh;
    public FMODUnity.EventReference pandaStatic;
    public FMODUnity.EventReference exitDoor;
    public FMODUnity.EventReference step;

    // Button SFX
    public FMODUnity.EventReference buttonHover;
    public FMODUnity.EventReference buttonClick;

    /*
     * Singleton code
     */
    private static SFXManager _instance;
    public static SFXManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SFXManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<SFXManager>();
                    Debug.Log("Generating new sfx manager");
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /*
     * Put the name of the SFX to play in the string parameter
     */
    public void PlayUnimplementedSFX(string eventName = "temp")
    {

        Debug.Log("Sound Effect played: " + eventName);
        OneShotSFX(buttonClick);
    }

    FMOD.Studio.EventInstance PausedEvent;

    public void PauseAdjust()
    {
        PausedEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Pause");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(PausedEvent, transform);
        PausedEvent.start();
    }

    public void UnpauseAdjust()
    {
        PausedEvent.setParameterByName("end", 1f);
        PausedEvent.release();
    }

    // music helper functions
    public void StabbyStabby()
    {
        MusicManager.Instance.GetEventInstance().setParameterByName("fade", 20f);
        MusicManager.Instance.GetEventInstance().setParameterByName("going backward", 0f);

        OneShotSFX(stab);
    }

    public void MischiefManaged(bool playRecordScratch = true)
    {
        MusicManager.Instance.GetEventInstance().setParameterByName("going backward", 1f);
        MusicManager.Instance.GetEventInstance().setParameterByName("fade", 0f);

        if (playRecordScratch)
        {
            OneShotSFX(recordScratch);
        }
    }

    // 0 is no megaphone, 1 is all megaphone.
    // pick one of those two values and FMOD does the lerp and everything
    public void SetMegaphone(float megaphoneAmount)
    {
        MusicManager.Instance.GetEventInstance().setParameterByName("megaphone", megaphoneAmount);
    }


    public void PushSFX()
    {
        OneShotSFX(push);
    }
    public void TapSFX()
    {
        OneShotSFX(tap);  //sfx.PlayOneShot(tap, 0.8f);
    }
    public void RotateSFX()
    {
        OneShotSFX(rotate); //sfx.PlayOneShot(rotate, 0.25f);
    }
    public void ScreamSFX()
    {
        OneShotSFX(scream); //sfx.PlayOneShot(scream)
    }
    public void BellSFX()
    {
        OneShotSFX(bell);  //sfx.PlayOneShot(bell);
    }
    public void HuhSFX()
    {
        OneShotSFX(huh); //sfx.PlayOneShot(huh, .6f);
    }
    public void GuhSFX()
    {
        OneShotSFX(guh); //sfx.PlayOneShot(guh, .6f);
    }
    public void PandaStaticSFX()
    {
        OneShotSFX(pandaStatic);
    }
    public void ExitDoorSFX()
    {
        OneShotSFX(exitDoor);
    }
    public void StepSFX()
    {
        OneShotSFX(step); //sfx.PlayOneShot(steps[UnityEngine.Random.Range(0, steps.Count)], 0.6f);
    }
    public void ButtonHover()
    {
        OneShotSFX(buttonHover);
    }
    public void ButtonClick()
    {
        OneShotSFX(buttonClick);
    }

    /**
     * Local function to make a sound event, play it, and then release it
     */
    private void OneShotSFX(FMODUnity.EventReference er) // maybe add a volume parameter later, maybe just do the mixing in FMOD 
    {
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(er);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }


}
