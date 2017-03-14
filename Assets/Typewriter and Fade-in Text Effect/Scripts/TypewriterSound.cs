//-------------------------------------
//  Typewriter & Fade-in Text Effect
//  Copyright © 2014 Kalandor Studio
//-------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An optional component that extends the Typewriter by playing sound effects while the effect is in progress.
/// </summary>
public class TypewriterSound : MonoBehaviour
{
    #region Fields exposed to the Editor

    /// <summary>
    /// The clips to be played. In case more than one is specified, they will be randomized.
    /// </summary>
    public List<SoundEntry> soundClips = new List<SoundEntry>();

    #endregion  Fields exposed to the Editor

    #region Fields and properties

    private AudioSource audioSource;

    private bool inProgress = false;

    #endregion  Fields and properties

    #region Helpers

    [System.Serializable]
    public class SoundEntry
    {
        public AudioClip clip;

        public float pitchRandomFactor = 0;

        public SoundEntry(AudioClip clip, float pitchRandomFactor = 0)
        {
            this.clip = clip;
            this.pitchRandomFactor = pitchRandomFactor;
        }
    }

    #endregion Helpers

    #region Unity calls

    void Awake()
    {
        // Setting the audio source and its parameters
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;

        if (soundClips.Count > 0)
        {
            audioSource.clip = RandomizeSound().clip;
        }
    }

    void Update()
    {
        // If the current sound effect has finished, let's play the next one
        if (inProgress && !audioSource.isPlaying)
        {
            PlayNextSound();
        }
    }

    #endregion  Unity calls

    #region Public methods

    /// <summary>
    /// Starts to play the sound effect.
    /// </summary>
    public void PlaySound()
    {
        inProgress = true;
    }

    /// <summary>
    /// Stops playing the sound effect.
    /// </summary>
    public void StopSound()
    {
        audioSource.Stop();
        inProgress = false;
    }

    #endregion Public methods

    private void PlayNextSound()
    {
        if (inProgress && soundClips.Count > 0)
        {
            SoundEntry entry = RandomizeSound();
            audioSource.clip = entry.clip;
            audioSource.pitch = RandomizePitch(entry.pitchRandomFactor);
            audioSource.Play();
        }
    }

    private SoundEntry RandomizeSound()
    {
        int index = Random.Range(0, soundClips.Count - 1);
        return soundClips[index];
    }

    private float RandomizePitch(float randomFactor)
    {
        float minValue = 1 - randomFactor / 2;
        float maxValue = 1 + randomFactor / 2;
        float val = Random.Range(minValue, maxValue);
        return val;
    }

}
