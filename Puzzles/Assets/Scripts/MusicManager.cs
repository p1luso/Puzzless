using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource para reproducir la música
    public AudioClip[] musicClips; // Array de clips de música
    public float fadeDuration = 1.0f; // Duración del fade in/out en segundos

    private int currentTrackIndex = 0; // Índice de la canción actual

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
            audioSource.clip = musicClips[currentTrackIndex];
            audioSource.Play();
            yield return StartCoroutine(FadeIn(audioSource, fadeDuration));

            yield return new WaitForSeconds(audioSource.clip.length - fadeDuration);

            yield return StartCoroutine(FadeOut(audioSource, fadeDuration));

            currentTrackIndex = (currentTrackIndex + 1) % musicClips.Length;
        }
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
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
}