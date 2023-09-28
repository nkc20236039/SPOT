using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioFile = new AudioClip[4];

    private enum SoundID
    {
        Jump = 0,
        LightPlace = 1,
        LightPick = 2,
        LightSlide = 3,
    }

    private void PlaySound(SoundID soundName)
    {
        Debug.Log(audioFile.Length);
        audioSource.clip = audioFile[(int)soundName];
        audioSource.Play();
    }
}
