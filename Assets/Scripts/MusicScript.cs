using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{

    public AudioSource normalSource; // has its clip stored inside
    public AudioSource alteredSource;

    private AudioSource currentSource;
    private float currentVolume;

    public AudioSource sfx;

    public AudioClip normalMusic;
    [SerializeField]
    private float normalVolume;

    public AudioClip alteredMusic;
    [SerializeField]
    private float alteredVolume;


    // all sfx will be played as oneshots through sfx
    public AudioClip stab;          // Specifically is incharge of switching from normal to altered
    public AudioClip recordScratch; // switches altered back to normal

    public AudioClip push;
    public AudioClip tap;
    public AudioClip rotate;
    public AudioClip scream;
    public AudioClip bell;
    public AudioClip huh;
    public AudioClip guh;
    public AudioClip pandaStatic;
    public AudioClip exitDoor;

    public List<AudioClip> steps;

    private static MusicScript _instance;
    public static MusicScript Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicScript>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<MusicScript>();
                    Debug.Log("Generating new music script");
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        normalSource.volume = normalVolume;
        normalSource.clip = normalMusic;
        normalSource.loop = true;
        normalSource.Play();

        alteredSource.volume = 0;
        alteredSource.clip = alteredMusic;
        alteredSource.loop = true;
        alteredSource.Play();

        currentSource = normalSource;
        currentVolume = normalVolume;

    }


    public void PauseAdjust()
    {
        StartCoroutine(FadeAudioSource.StartFade(currentSource, .1f, currentVolume / 2));
    }

    public void UnpauseAdjust()
    {
        StartCoroutine(FadeAudioSource.StartFade(currentSource, .1f, currentVolume));
    }

    public void StabbyStabby()
    {
        normalSource.volume = 0;
        sfx.PlayOneShot(stab);
        StartCoroutine(FadeAudioSource.StartFade(alteredSource, .25f, alteredVolume)); // middle value is how long to fade in
        currentSource = alteredSource;
        currentVolume = alteredVolume;
    }

    public void MischiefManaged(bool playRecordScratch = true)
    {
        alteredSource.volume = 0;
        if(playRecordScratch){
            sfx.PlayOneShot(recordScratch, 0.25f);
        }
        StartCoroutine(FadeAudioSource.StartFade(normalSource, .65f, normalVolume)); // middle value is how long to fade in
        currentSource = normalSource;
        currentVolume = normalVolume;
    }


    public void PushSFX()
    {
        sfx.PlayOneShot(push);
    }
    public void TapSFX()
    {
        sfx.PlayOneShot(tap, 0.8f);
    }
    public void RotateSFX()
    { 
        sfx.PlayOneShot(rotate, 0.25f);
    }
    public void ScreamSFX()
    {
        sfx.PlayOneShot(scream);
    }
    public void BellSFX()
    {
        sfx.PlayOneShot(bell);
    }
    public void HuhSFX()
    {
        sfx.PlayOneShot(huh, .6f);
    }
    public void GuhSFX()
    {
        sfx.PlayOneShot(guh, .6f);
    }
    public void PandaStaticSFX()
    {
        sfx.PlayOneShot(pandaStatic);
    }
    public void ExitDoorSFX()
    {
        sfx.PlayOneShot(exitDoor);
    }
    public void StepSFX()
    {
        sfx.PlayOneShot(steps[UnityEngine.Random.Range(0, steps.Count)], 0.6f);
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