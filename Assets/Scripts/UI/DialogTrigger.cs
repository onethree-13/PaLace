using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;

    public void TriggerDialog()
    {
        DialogManager.instance.StartDialogue(dialog);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
