using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController audioController;

    private void Awake()
    {
        if (audioController == null)
        {
            audioController = this;
            DontDestroyOnLoad(audioController);
        }
        else if (audioController != this)
        {
            Destroy(audioController);
        }
    }
}
