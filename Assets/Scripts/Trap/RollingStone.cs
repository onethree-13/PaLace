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
    public AudioClip stopRollingSFXClip;
    public AudioClip hitSFXClip;
    bool isRolling = false;
    float last_speed = 0f;

    void Awake()
    {
        m_stoneSFXSource = GetComponent<AudioSource>();

        // Rolling sfx
        m_stoneSFXSource.clip = rollingSFXClip;
        m_stoneSFXSource.loop = true;
    }

    void FixedUpdate()
    {
        if (isRolling)
        {
            // Stop rolling SFX when stone stops moving for stopCountDown
            float current_speed = rb.velocity.sqrMagnitude;

            if (current_speed < last_speed && current_speed < 0.01f)
            {
                m_stoneSFXSource.Stop();
                m_stoneSFXSource.PlayOneShot(stopRollingSFXClip);
            }

            last_speed = current_speed;
        }
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
            m_stoneSFXSource.Stop();
            m_stoneSFXSource.PlayOneShot(hitSFXClip);

            player.Damage(1);
            Invoke("Destructor", 0.5f);
        }
    }

    public void Destructor()
    {
        Destroy(gameObject);
    }

    public void StartRollingSFX()
    {
        isRolling = true;
        m_stoneSFXSource.Play();
    }
}