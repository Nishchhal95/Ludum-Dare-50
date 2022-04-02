using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyFlowGames.General
{
    /// <summary>
    /// Plays music, continuously triggering next at random, as soon as one is completed
    /// </summary>
    public class GeneralMusic : MonoBehaviour
    {
        public AudioSource AudioSource;
        public AudioClip[] AudioClips;

        void Awake()
        {
        }

        void Update()
        {
            if (!AudioSource.isPlaying)
            {
                if (AudioClips.Length > 0)
                {
                    AudioSource.clip = AudioClips[Random.Range(0, AudioClips.Length)];
                    AudioSource.Play();
                }
            }
        }
    }
}