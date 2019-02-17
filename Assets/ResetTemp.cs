using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTemp : MonoBehaviour
{
    public RollingStoneTrigger rsReset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("RollingStone reset.");
            rsReset.Reset();
        }
    }
}
