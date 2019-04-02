using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneScript : MonoBehaviour
{
    public PlayableDirector timeline;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        timeline.Play();
    }
}
