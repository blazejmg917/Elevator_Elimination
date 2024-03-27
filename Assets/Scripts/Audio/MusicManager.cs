//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// timeline markers in your game code. 
//
// Timeline markers can be implicit - such as beats and bars. Or they 
// can be explicity placed by sound designers, in which case they have 
// a sound designer specified name attached to them.
//
// Timeline markers can be useful for syncing game events to sound
// events.
//
// The script starts a piece of music and then displays on the screen
// the current bar and the last marker encountered.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
// For information on using FMOD example code in your own programs, visit
// https://www.fmod.com/legal
//
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    /*
     * Keeps track off where we are in the timeline
     */
    class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public int CurrentMusicBeat = 0;
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
    }

    static TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    public FMODUnity.EventReference MusicEventName;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    /*
     * Singleton code
     */
    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<MusicManager>();
                    Debug.Log("Generating new music manager");
                }
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    void Reset() // maybe we can get rid of this later
    {
        MusicEventName = FMODUnity.EventReference.Find("event:/Temp Music");
    }
#endif

    void Start()
    {
        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        musicInstance = FMODUnity.RuntimeManager.CreateInstance(MusicEventName);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(musicInstance, transform);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }

    void OnGUI()
    {
        //GUILayout.Box(String.Format("Current Bar = {0}.{1}, Last Marker = {2}", timelineInfo.CurrentMusicBar, timelineInfo.CurrentMusicBeat, (string)timelineInfo.LastMarker));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.CurrentMusicBar = parameter.bar;
                        timelineInfo.CurrentMusicBeat = parameter.beat;

                        // Code for bobbing here
                        AnimateBobbing();
                       
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }

    private static void AnimateBobbing()
    {
        PersonHolder personHolder = PersonManager.Instance.GetPHolder();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerMechanics>().OnBob(timelineInfo.CurrentMusicBeat % 2 == 1);
        }
        if (personHolder != null)
        {
            int number = personHolder.transform.childCount;
            for (int i = 0; i < number; i++)
            {
                Person p = personHolder.transform.GetChild(i).GetComponent<Person>();
                p.OnBob(timelineInfo.CurrentMusicBeat % 2 == 1);
            }
        }
    }

    public FMOD.Studio.EventInstance GetEventInstance()
    {
        return musicInstance;
    }
}