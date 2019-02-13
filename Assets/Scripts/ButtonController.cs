using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonController : MonoBehaviour
{
    public void NewGameBtn(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }
}
