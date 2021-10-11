using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************************************************
 * This script is for playing background audio when the scene spawns in AR
 ********************************************************************************************************/

public class AudioPlay : MonoBehaviour
{
    private AudioSource ambient_sound;

    void Start()
    {
        //Initiate individual ambient sound when they spawn at the start
        //Play sound as soon as it is enabled
        ambient_sound = GetComponent<AudioSource>();
        ambient_sound.Play();
    }
    
}
