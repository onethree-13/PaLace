using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Ballista : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject ArrowPrefab;
    public int ArrowNumber = 3;
    public float ArrowInterval = 0.2f;

    // SFX related
    public AudioClip shootSFXClip;
    protected AudioSource m_BallistaSFxSource;
    
    

    public void Start()
    {
        m_BallistaSFxSource = GetComponent<AudioSource>();
        if (m_BallistaSFxSource != null)
            m_BallistaSFxSource.clip = shootSFXClip;
    }

    public void Shoot()
    {
        for(int i = 0; i < ArrowNumber; i++)
        {
            Invoke("ShootOnce", ArrowInterval * i);
        }
    }

    void ShootOnce()
    {
        Instantiate(ArrowPrefab, FirePoint.position, FirePoint.rotation);
        m_BallistaSFxSource.Play();
    }
}
