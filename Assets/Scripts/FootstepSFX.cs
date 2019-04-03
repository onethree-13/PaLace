using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController2D))]
public class FootstepSFX : MonoBehaviour
{
    public AudioClip stepSFXClip;

    protected AudioSource m_Audio;
    protected CharacterController2D m_Controller;

    private bool step = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Audio = GetComponent<AudioSource>();
        m_Controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!step && m_Controller.isGrounded && Mathf.Abs(m_Controller.GetVelocity().x) > 0.5f)
        {
            m_Audio.clip = stepSFXClip;
            m_Audio.Play();
            StartCoroutine(WaitForFootStepPlay(stepSFXClip.length));
        }
    }

    private IEnumerator WaitForFootStepPlay(float second)
    {
        step = true;
        yield return new WaitForSeconds(second);
        step = false;
    }
}
