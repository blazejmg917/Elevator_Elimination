using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaSound : MonoBehaviour
{
    FMOD.Studio.EventInstance CRT;

    private void OpenCRT()
    {
        CRT = FMODUnity.RuntimeManager.CreateInstance("event:/SFX Common/Panda/CRT");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(CRT, transform);
        CRT.start();
    }
    public void CloseCRT()
    {
        CRT.setParameterByName("end", 1f);
        CRT.release();
    }
}
