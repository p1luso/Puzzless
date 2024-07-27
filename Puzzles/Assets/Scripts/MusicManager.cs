using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource para reproducir la música
    public AudioClip[] musicClips; // Array de clips de música
    public float fadeDuration = 1.0f; // Duración del fade in/out en segundos

    private int currentTrackIndex = 0; // Índice de la canción actual
    private bool isMusicActive = true; // Flag para controlar el estado de reproducción

    private void Start()
    {
        if (musicClips.Length > 0)
        {
            StartCoroutine(PlayMusic());
        }
    }

    private IEnumerator PlayMusic()
    {
        while (true)
        {
            if (isMusicActive)
            {
                currentTrackIndex = GetRandomTrackIndex();
                audioSource.clip = musicClips[currentTrackIndex];
                audioSource.Play();
                yield return StartCoroutine(FadeIn(audioSource, fadeDuration));

                yield return new WaitForSeconds(audioSource.clip.length - fadeDuration);

                yield return StartCoroutine(FadeOut(audioSource, fadeDuration));
            }
            else
            {
                yield return null;
            }
        }
    }

    private int GetRandomTrackIndex()
    {
        return Random.Range(0, musicClips.Length);
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;

        while (audioSource.volume < 0.2f)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 0.2f;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 0;
    }

    public void PauseMusic()
    {
        isMusicActive = false;
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        isMusicActive = true;
        audioSource.UnPause();
        if (!audioSource.isPlaying)
        {
            StartCoroutine(PlayMusic());
        }
    }

    public bool IsMusicActive()
    {
        return isMusicActive;
    }
}
