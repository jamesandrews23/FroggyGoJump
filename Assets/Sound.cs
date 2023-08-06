using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioSource source;

    public void PlayLegendarySound()
    {
        source.Play();
    }
}
