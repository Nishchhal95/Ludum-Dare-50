using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyFlowGames.General
{
    /// <summary>
    /// Simple audio one-shot manager
    /// Requires: HappyFlowGames.General.DebugAndLog
    /// </summary>
    public class GeneralAudioOneShot : MonoBehaviour
    {
        [Header("Base")]
        public AudioSource PlayerAudioSource;

        [Header("Audio Clips")]
        public AudioClip[] AudioClip;
        public string[] AudioClipNames;

        // To avoid multiple times level end SFX (which might happen in Main Game Loop)
        [Header("Level End")]
        public string LevelEndClipName;
        private bool AlreadyPlayedLevelEndSFX;

        #region Init
        public void Awake()
        {
            DnT.Assert(AudioClip.Length == AudioClipNames.Length, "GeneralAudioOneShot.Awake", "Audio Clips and Names have different length");
        }

        public void InitLevel()
        {
            AlreadyPlayedLevelEndSFX = false;
        }
        #endregion Init

        #region Core
        /// <summary>
        /// Play
        /// </summary>
        /// <param name="clip"></param>
        public void PlayerSFX(string clip)
        {
            if (clip == LevelEndClipName)
            {
                if (AlreadyPlayedLevelEndSFX)
                    return;
                AlreadyPlayedLevelEndSFX = true;
            }

            int clipIndex = Array.IndexOf(AudioClipNames, clip);
            if (clipIndex >= 0)
                PlayerAudioSource.PlayOneShot(AudioClip[(int)clipIndex]);
        }
        #endregion Core
    }
}