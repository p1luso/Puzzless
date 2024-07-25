using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public GameObject _canvasCategories;

    public GameObject _canvasSelection;

    public ContentManager _contentManager;
    // Métodos para botones de categorías
    public void ShowAnimals()
    {
        _contentManager.LoadImagesFromAddressables("Animals");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);

    }

    public void ShowAbstract()
    {
        _contentManager.LoadImagesFromAddressables("Abstract");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);

    }

    public void ShowFantasy()
    {
        _contentManager.LoadImagesFromAddressables("Fantasy");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);

    }

    public void ShowLandscapes()
    {
        _contentManager.LoadImagesFromAddressables("Landscapes");
        _canvasCategories.SetActive(false);
        _canvasSelection.SetActive(true);

    }
}