using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//FIXME:: audio control is not finished yet.
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void LoadMenu()
    {
        Debug.Log("Load menu.");
        Time.timeScale = 1f;
        // SceneManager.LoadScene("wharf");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");
        Time.timeScale = 1f;
        gameController.ResetGameCtrl();
        SceneManager.LoadScene("start_menu");
    }
}