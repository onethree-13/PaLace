using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Singleton
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
            Destroy(gameObject);
        }
    }
    #endregion

    protected AudioSource BGMSource;

    public float defaultBGMVolume = 0.5f;
    public float defaultSFXVolume = 1.0f;

    private void Start()
    {
        BGMSource = GetComponent<AudioSource>();
        BGMSource.ignoreListenerVolume = true;

        // initial volume 
        SetBGMVolume(defaultBGMVolume);
        SetSFXVolume(defaultSFXVolume);
    }

    public void SetBGMVolume(float vol)
    {
        BGMSource.volume = vol;
    }

    public float GetBGMVolume()
    {
        return BGMSource.volume;
    }

    public void SetSFXVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    public float GetSFXVolume()
    {
        return AudioListener.volume;
    }
}
