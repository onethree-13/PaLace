using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStoneTrigger : MonoBehaviour
{
    private RollingStone rollingStone;
    public RollingStoneSpawn rsSpawn;

    private void Start()
    {
        Reset();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            rollingStone.Fall();
            gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        if (rollingStone != null)
        {
            rollingStone.Destructor();
        }
        rollingStone = rsSpawn.Spawn().GetComponent<RollingStone>();
    }
}
