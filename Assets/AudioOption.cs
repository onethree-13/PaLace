using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOption : MonoBehaviour
{
    

    public Slider BGMSlider;
    public Slider SFXSlider;

    // Start is called before the first frame update
    void Start()
    {
        BGMSlider.value = AudioController.audioController.GetBGMVolume();
        SFXSlider.value = AudioController.audioController.GetSFXVolume();
    }

    public void BGMSliderChanged(float vol)
    {
        AudioController.audioController.SetBGMVolume(vol);
        Debug.Log(string.Format("Current BGM Volume: {0}", AudioController.audioController.GetBGMVolume()));
    }

    public void SFXSliderChanged(float vol)
    {
        AudioController.audioController.SetSFXVolume(vol);
        Debug.Log(string.Format("Current SFX Volume: {0}", AudioController.audioController.GetSFXVolume()));
    }
}
