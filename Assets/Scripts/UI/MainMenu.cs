using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Inventory.instance.Reset();
        SceneManager.LoadScene("wharf");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        Application.Quit();
    }
}
