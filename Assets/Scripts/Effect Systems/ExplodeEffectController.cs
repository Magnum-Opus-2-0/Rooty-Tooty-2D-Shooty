using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffectController : MonoBehaviour
{
    private ParticleSystem ps;
    private AudioSource noise;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        noise = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Play the particle system and the audio clip.
    /// </summary>
    public void Play()
    {
        ps.Play();
        noise.Play();
    }

    /// <summary>
    /// Stop the particle system and the audio clip.
    /// </summary>
    public void Stop()
    {
        ps.Stop();
        noise.Stop();
    }
}
