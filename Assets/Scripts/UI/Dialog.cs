using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog {
    public string name;

    [TextArea(3, 10)]
    public string[] sentences;

    public Sprite image;

    public Dialog(string name, string sentence, Sprite image)
    {
        this.name = name;
        this.sentences = new string[1] { sentence };
        this.image = image;
    }
}
