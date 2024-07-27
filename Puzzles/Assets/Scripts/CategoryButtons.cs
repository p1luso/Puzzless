using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public GameObject _canvasCategories;

    public GameObject _canvasSelection;
    public AudioSource buttonAudioSource; // AudioSource para los sonidos de los botones
    public AudioClip buttonClickSound; // Clip de sonido para el click del botón
    public ContentManager _contentManager;
    // Métodos para botones de categorías
    public void ShowAnimals()
    {
        PlayButtonClickSound();
        _contentManager.LoadImagesFromAddressables("Animals");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);
    }

    public void ShowAbstract()
    {
        PlayButtonClickSound();
        _contentManager.LoadImagesFromAddressables("Abstract");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);
    }

    public void ShowFantasy()
    {
        PlayButtonClickSound();
        _contentManager.LoadImagesFromAddressables("Fantasy");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);
    }

    public void ShowLandscapes()
    {
        PlayButtonClickSound();
        _contentManager.LoadImagesFromAddressables("Landscapes");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);

    }
    
    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}