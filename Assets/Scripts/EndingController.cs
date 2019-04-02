using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        DialogTrigger dialogTrigger;
        GameObject panel = GameObject.Find("Panel");
        GameObject playButton = GameObject.Find("PlayButton");
        GameObject quitButton = GameObject.Find("QuitButton");
        GameObject title = GameObject.Find("Title");

        int itemCollectedNum = Inventory.instance.getItemNumber();

        itemCollectedNum = 9;

        switch (itemCollectedNum)
        {
            case int n when (n == 0):
                dialogTrigger = GameObject.Find("Bad Ending Talk").GetComponent<DialogTrigger>();
                title.GetComponent<TextMeshProUGUI>().SetText("[Meaningless] Attempt");
                break;
            case int n when (n <= 4):
                dialogTrigger = GameObject.Find("Normal Ending Talk").GetComponent<DialogTrigger>();
                title.GetComponent<TextMeshProUGUI>().SetText("Divide And [Not] Conquer");
                break;
            case int n when (n <= 8):
                dialogTrigger = GameObject.Find("Good Ending Talk").GetComponent<DialogTrigger>();
                title.GetComponent<TextMeshProUGUI>().SetText("[Good] Ending");
                break;
            case int n when (n == 9):
                dialogTrigger = GameObject.Find("Hidden Ending Talk").GetComponent<DialogTrigger>();
                title.GetComponent<TextMeshProUGUI>().SetText("[Unbroken] Circle");
                break;
            // just in case
            default:
                dialogTrigger = GameObject.Find("Normal Ending Talk").GetComponent<DialogTrigger>();
                title.GetComponent<TextMeshProUGUI>().SetText("Divide And [Not] Conquer");
                break;
        }

        dialogTrigger.TriggerDialog();
        StartCoroutine(EnablePanel(new GameObject[] { panel, playButton, quitButton, title }));
    }

    IEnumerator EnablePanel(GameObject []gameObjects)
    {

        DialogManager dialogManager = DialogManager.instance;

        for(int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(false);
        }

        // While dialog is playing
        while (dialogManager.getDialogPlayStatus())
        {
            yield return new WaitForSeconds(0.1f);
        }

        // set panel to activated
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(true);
        }
    }
}