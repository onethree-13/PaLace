using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : MonoBehaviour
{
    public Rigidbody2D rb;
    public float gravityThreshold;

    // SFX Raleted
    protected AudioSource m_stoneSFXSource;
    public AudioClip rollingSFXClip;
    public float stopCountDown = 1f;
    float stopTimer = 0f;
    bool isRolling = false;

    public void Awake()
    {
        m_stoneSFXSource = GetComponent<AudioSource>();

        // Rolling sfx
        m_stoneSFXSource.clip = rollingSFXClip;
        m_stoneSFXSource.loop = true;
    }

    public void FixedUpdate()
    {
        // Stop rolling SFX when stone stops moving for stopCountDown

        if (Mathf.Abs(rb.velocity.x) > 0.05f && Mathf.Abs(rb.velocity.y) > 0.05f)
            stopTimer = stopCountDown;

        if (stopTimer <= 0f && isRolling) {
            m_stoneSFXSource.Stop();
        }

        if (stopTimer > 0f)
            stopTimer -= Time.fixedDeltaTime;
    }

    public void Fall()
    {
        rb.gravityScale = gravityThreshold;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.name);
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(1);
            Invoke("Destructor", 0.5f);
        }
    }

    public void Destructor()
    {
        isRolling = false;
        Destroy(gameObject);
    }

    public void StartRollingSFX()
    {
        isRolling = true;
        stopTimer = stopCountDown;
        m_stoneSFXSource.Play();
    }
}