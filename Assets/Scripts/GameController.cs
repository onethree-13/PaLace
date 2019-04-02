using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private HashSet<string> dialogTriggeredSet;
    public static GameController instance;
    public string lastLoadedScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialogTriggeredSet = new HashSet<string>();
    }

    // Remove an Object from scense
    public void KillObject(GameObject obj)
    {
        Destroy(obj);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Check whether a dialog has been triggered
    public bool IsDialogNotTriggered(string dialogName)
    {
        return dialogTriggeredSet.Add(dialogName);
    }
}
