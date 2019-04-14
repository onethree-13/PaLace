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
        timeline.stopped += OnPlayableDirectorStopped;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.EnableControl = false;
        timeline.Play();
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (timeline == aDirector)
        {
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
            Debug.Log(player.transform.position);
            player.EnableControl = true;
        }
    }
}
