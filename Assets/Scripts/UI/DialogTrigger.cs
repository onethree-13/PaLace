using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogTrigger : MonoBehaviour
{
    public string dialogName;
    public Dialog[] dialogs;
    public bool triggerOnlyOnce;

    public void TriggerDialog()
    {
        foreach (Dialog dialog in dialogs)
        {
            DialogManager.instance.StartDialogue(dialog);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If dialog should trigger only one time and is already triggered
        GameController gameController = GameObject.FindGameObjectWithTag("GameCtrl").GetComponent<GameController>();

        if (triggerOnlyOnce == true && gameController.IsDialogNotTriggered(dialogName) == false)
        {
            return;
        }

        // gameObject.SetActive(false);
        TriggerDialog();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    void OnMouseDown()
    {
        // gameObject.SetActive(false);
        Debug.Log("Mouse Over.");
        TriggerDialog();
        // gameObject.SetActive(true);
    }
    
}
