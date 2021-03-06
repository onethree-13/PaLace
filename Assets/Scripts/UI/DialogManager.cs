﻿using UnityEngine.UI;
using System;
using System.Collections ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DialogManager : MonoBehaviour
{
    #region Singleton
    public static DialogManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += LoadMenu;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    
    private Queue<Dialog> dialogs;
    private GameObject menu;
    private GameObject dialogBox;
    private Text nameText;
    private Text dialogText;
    private Animator animator;
    private Image avatar;
    private GameObject floatMenu;
    private Button continueButton;
    private bool isDialogPlaying;         // for getting status of co-routine

    Player player;

    void Start()
    {
        dialogs = new Queue<Dialog>();
        
    }

    private void LoadMenu(Scene current, Scene next)
    {
        Debug.Log("Menu loaded.");
        menu = GameObject.FindGameObjectWithTag("Menu");
        try
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("Warning: no player found, check if not start scence");
        }
        if (menu == null)
            return;
        dialogBox = menu.transform.Find("DialogBox").gameObject;
        floatMenu = menu.transform.Find("FloatingMenu").gameObject;
        animator = menu.transform.Find("DialogBox").GetComponent<Animator>();
        nameText = dialogBox.transform.Find("Name").GetComponent<Text>();
        dialogText = dialogBox.transform.Find("Dialog").GetComponent<Text>();
        avatar = dialogBox.transform.Find("Image").GetComponent<Image>();
        continueButton = dialogBox.transform.Find("Continue").GetComponent<Button>();
        if (floatMenu == null || animator == null || avatar == null || dialogText == null || nameText == null || continueButton == null)
        {
            Debug.LogWarning("DialogManager: Initialization failed.");
        }
        continueButton.onClick.AddListener(delegate () { DisplayNextSentence(); });
    }

    public void StartDialogue (Dialog dialog) {

        floatMenu.SetActive(false);
        player.EnableControl = false;

        animator.SetBool("IsOpen", true);
        foreach (string sentence in dialog.sentences)
        {
            dialogs.Enqueue(new Dialog(dialog.name, sentence, dialog.image));
        }

        if (!isDialogPlaying) {
            // Set not playing to false
            isDialogPlaying = true;
            DisplayNextSentence();
        }
    }
    
    public void DisplayNextSentence()
    {
        if (dialogs.Count == 0)
        {
            EndDialog();
            return;
        }
        Dialog dialog = dialogs.Dequeue();
        nameText.text = dialog.name;
        avatar.sprite = dialog.image;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialog.sentences[0]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return null;
        }
    }

    public void EndDialog()
    {
        // Set dialog playing to false
        isDialogPlaying = false;

        animator.SetBool("IsOpen", false);
        player.EnableControl = true;
        floatMenu.SetActive(true);
    }

    // Get dialog playing status
    public bool getDialogPlayStatus()
    {
        return isDialogPlaying;
    }
}
