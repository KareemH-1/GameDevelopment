//This script is used to play the ship sound on loop

using UnityEngine;

public class ShipSoundManager : MonoBehaviour
{
    public AudioSource shipSound;

    void Start()
    {
        if (shipSound != null && !shipSound.isPlaying)
        {
            shipSound.loop = true; 
            shipSound.Play();
        }
    }
}
