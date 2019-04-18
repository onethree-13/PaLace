using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 
    private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
    }

    public void PlayGame()
    {
        Inventory.instance.Reset();
        gameController.ResetGameCtrl();
        SceneManager.LoadScene("wharf");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        Application.Quit();
    }
}
