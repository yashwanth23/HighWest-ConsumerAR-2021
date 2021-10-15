using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchCharacter : MonoBehaviour
{
    public AudioSource playSound;
    GameObject HitObject;

    public AudioClip CF_voice, AP_voice, DR_voice, RR_voice;
    private bool touch_check; // To see if the object is touched atleast once

    private int layerMask;
    void Start()
    {
        touch_check = false;
        layerMask = LayerMask.GetMask("Highlight Objects");
    }


    void Update()
    {
        //If any character is highlighted then look for touch raycast 
        if (HighlightObject.selectedCharacter != 0)
        {
#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray_mouse = Camera.main.ScreenPointToRay(Input.mousePosition);
                OnTouchPlay(ray_mouse);
            }
#else
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                Ray ray_touch = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                OnTouchPlay(ray_touch);
                
            }
#endif
        }
        else
        {
            //If the character is not highlighted the existing sound will stop playing
            playSound.Stop();
            touch_check = false;
            Debug.Log("Stopping play");
        }

    }

    private void OnTouchPlay(Ray ray)
    {
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit, layerMask) && !touch_check)
        {
            HitObject = hit.transform.gameObject;

            switch (HitObject.tag)
            {
                //If the character is touched once the sound starts plays and will not enter the if condition again 
                case "CF":
                    playAudio(CF_voice);
                    break;
                case "AP":
                    playAudio(AP_voice);
                    break;
                case "RR":
                    playAudio(RR_voice);
                    break;
                case "DR":
                    playAudio(DR_voice);
                    break;
                default:
                    break;
            }
        }
    }
    private void playAudio(AudioClip clip)
    {
        playSound.clip = clip;
        playSound.Play();
        touch_check = true;
    }
}
