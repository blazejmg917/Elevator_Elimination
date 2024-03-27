using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{

    public FMOD.Studio.EventInstance testingEventInstance;
    public FMODUnity.EventReference testMusic;


    public FMODUnity.EventReference stab;
    public FMODUnity.EventReference recordScratch;

    // NEW SFX
    public FMODUnity.EventReference push;
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




    private static AudioScript _instance;
    public static AudioScript Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioScript>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<AudioScript>();
                    Debug.Log("Generating new music script");
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // testing FMOD stuff
        //testingEventInstance = FMODUnity.RuntimeManager.CreateInstance(testMusic); // old way, can't do it like this bc the callback class makes the 
        testingEventInstance = TestingTimelineCallback.Instance.GetMusicInstance();

        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(testingEventInstance, transform); // ig we don't need to do this bc 2D game?
        //testingEventInstance.start(); // not starting it here anymore


        //normalSource.volume = normalVolume;
        //normalSource.clip = normalMusic;
        //normalSource.loop = true;
        //normalSource.Play();

        //alteredSource.volume = 0;
        //alteredSource.clip = alteredMusic;
        //alteredSource.loop = true;
        //alteredSource.Play();

        //currentSource = normalSource;
        //currentVolume = normalVolume;

    }


    public void PauseAdjust()
    {
        //StartCoroutine(FadeAudioSource.StartFade(currentSource, .1f, currentVolume / 2));

    }

    public void UnpauseAdjust()
    {
        //StartCoroutine(FadeAudioSource.StartFade(currentSource, .1f, currentVolume));
    }

    public void StabbyStabby()
    {
        testingEventInstance.setParameterByName("fade", 20f);

        OneShotSFX(stab);
        //normalSource.volume = 0;
        //sfx.PlayOneShot(stab);
        //StartCoroutine(FadeAudioSource.StartFade(alteredSource, .25f, alteredVolume)); // middle value is how long to fade in
        //currentSource = alteredSource;
        //currentVolume = alteredVolume;
    }

    public void MischiefManaged(bool playRecordScratch = true)
    {
        testingEventInstance.setParameterByName("fade", 0f);

        if (playRecordScratch)
        {
            OneShotSFX(recordScratch);
        }

        //alteredSource.volume = 0;
        //if(playRecordScratch){
        //    sfx.PlayOneShot(recordScratch, 0.25f);
        //}
        //StartCoroutine(FadeAudioSource.StartFade(normalSource, .65f, normalVolume)); // middle value is how long to fade in
        //currentSource = normalSource;
        //currentVolume = normalVolume;
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
        Debug.Log("took step");
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

