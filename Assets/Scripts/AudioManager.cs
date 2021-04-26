using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource soundSource;
    public AudioClip[] connectionClips;

    // Start is called before the first frame update
    public void PlayConnectionEffect()
    {
        soundSource.clip = connectionClips[Random.Range(0, connectionClips.Length)];
        soundSource.Play();
    }
}
