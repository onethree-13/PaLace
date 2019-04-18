using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSFX : MonoBehaviour
{
    protected Player player;
    protected Vector3 waterPos;
    protected float waterRadius;
    public float soundMargin = 5f;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        waterPos = transform.parent.position;
        waterRadius = transform.parent.localScale.x + soundMargin;
        transform.position = new Vector3(waterPos.x, waterPos.y + transform.parent.localScale.y, waterPos.z);
        Debug.Log(waterRadius);
    }

    private void FixedUpdate()
    {
        float audioPosX = player.transform.position.x;
        // constrain position to circle around water
        if (Mathf.Abs(audioPosX - waterPos.x) > waterRadius)
        {
            audioPosX = waterPos.x + Mathf.Sign(audioPosX - waterPos.x) * waterRadius;
        }

        transform.position = new Vector3(audioPosX, transform.position.y, transform.position.z);
    }
}
