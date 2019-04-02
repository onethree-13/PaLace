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
        GameObject panel = GameObject.Find("Panel");
        panel.SetActive(false);

        int itemCollectedNum = Inventory.instance.getItemNumber();
        DialogTrigger dialogTrigger;

        switch (itemCollectedNum)
        {
            case int n when (n == 0):
                dialogTrigger = GameObject.Find("Bad Ending Talk").GetComponent<DialogTrigger>();
                break;
            case int n when (n <= 4):
                dialogTrigger = GameObject.Find("Normal Ending Talk").GetComponent<DialogTrigger>();
                break;
            case int n when (n <= 8):
                dialogTrigger = GameObject.Find("Good Ending Talk").GetComponent<DialogTrigger>();
                break;
            case int n when (n == 9):
                dialogTrigger = GameObject.Find("Hidden Ending Talk").GetComponent<DialogTrigger>();
                break;
            // just in case
            default:
                dialogTrigger = GameObject.Find("Normal Ending Talk").GetComponent<DialogTrigger>();
                break;
        }

        dialogTrigger.TriggerDialog();
        StartCoroutine(EnablePanel(panel));
    }

    IEnumerator EnablePanel(GameObject panel)
    {
        DialogManager dialogManager = DialogManager.instance;

        // While dialog is playing
        while(dialogManager.getDialogPlayStatus())
        {
            yield return new WaitForSeconds(0.1f);
        }

        // set panel to activated
        panel.SetActive(true);
    }
}