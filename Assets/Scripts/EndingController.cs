using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += PlayEnding;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= PlayEnding;
    }

    private void PlayEnding(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Ending scene loaded");
        Debug.Log(Inventory.instance.getItemNumber());
    }
}
