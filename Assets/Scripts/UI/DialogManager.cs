using UnityEngine.UI;
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


    private Queue<string> sentences;
    private GameObject menu;
    private GameObject dialogBox;
    private Text nameText;
    private Text dialogText;
    private Animator animator;
    private Image avatar;
    private GameObject floatMenu;
    private Button continueButton;
    private bool isDialogPlaying;         // for getting status of co-routine

    void Start()
    {
        sentences = new Queue<string>();
    }

    private void LoadMenu(Scene current, Scene next)
    {
        Debug.Log("Menu loaded.");
        menu = GameObject.FindGameObjectWithTag("Menu");
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
        animator.SetBool("IsOpen", true);
        nameText.text = dialog.name;
        avatar.sprite = dialog.image;
        sentences.Clear();
        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }

        // Set not playing to false
        isDialogPlaying = true;

        DisplayNextSentence();
    }
    
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialog();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
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
        floatMenu.SetActive(true);
    }

    // Get dialog playing status
    public bool getDialogPlayStatus()
    {
        return isDialogPlaying;
    }
}
