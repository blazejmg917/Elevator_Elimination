using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{

    public FMOD.Studio.EventInstance testingEventInstance;
    public FMODUnity.EventReference testMusic;


    public FMODUnity.EventReference stab;

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
        testingEventInstance = FMODUnity.RuntimeManager.CreateInstance(testMusic);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(testingEventInstance, transform);
        testingEventInstance.start();


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
        testingEventInstance.setParameterByName("fade", 0f);
        //normalSource.volume = 0;
        //sfx.PlayOneShot(stab);
        //StartCoroutine(FadeAudioSource.StartFade(alteredSource, .25f, alteredVolume)); // middle value is how long to fade in
        //currentSource = alteredSource;
        //currentVolume = alteredVolume;
    }

    public void MischiefManaged(bool playRecordScratch = true)
    {
        testingEventInstance.setParameterByName("fade", 1f);

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
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(push);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void TapSFX()
    {
        //sfx.PlayOneShot(tap, 0.8f);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(tap);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void RotateSFX()
    {
        //sfx.PlayOneShot(rotate, 0.25f);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(rotate);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void ScreamSFX()
    {
        //sfx.PlayOneShot(scream);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(scream);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void BellSFX()
    {
        //sfx.PlayOneShot(bell);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(bell);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void HuhSFX()
    {
        //sfx.PlayOneShot(huh, .6f);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(huh);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void GuhSFX()
    {
        //sfx.PlayOneShot(guh, .6f);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(guh);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void PandaStaticSFX()
    {
        //sfx.PlayOneShot(pandaStatic);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(pandaStatic);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void ExitDoorSFX()
    {
        //sfx.PlayOneShot(exitDoor);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(exitDoor);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }
    public void StepSFX()
    {
        //sfx.PlayOneShot(steps[UnityEngine.Random.Range(0, steps.Count)], 0.6f);
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(step);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();

        Debug.Log("took step");
    }

    public void ButtonHover()
    {
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(buttonHover);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }

    public void ButtonClick()
    {
        FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(buttonClick);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(temp, transform);
        temp.start();
        temp.release();
    }



}



// code borrowed from online, fades audio out
// use:
// StartCoroutine(FadeAudioSource.StartFade(AudioSource audioSource, float duration, float targetVolume));
public static class FadeAudioSource
{
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

}