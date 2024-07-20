using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour
{
    public RawImage rawImage;
    public Slider imageSlider;
    public Button loadImageButton;
    public Button button3x3;
    public Button button4x4;
    public Button button5x5;
    public PuzzleManager puzzleManager;
    public string imagesPath = "/Imagenes"; // Cambia esto a la ruta de tu carpeta de imágenes
    private List<Texture2D> textures = new List<Texture2D>();

    void Start()
    {
        LoadImagesFromFolder();
        SetupSlider();

        loadImageButton.onClick.AddListener(() => LoadImage());
        button3x3.onClick.AddListener(() => puzzleManager.DivideImage(3, 3));
        button4x4.onClick.AddListener(() => puzzleManager.DivideImage(4, 4));
        button5x5.onClick.AddListener(() => puzzleManager.DivideImage(5, 5));
    }

    void LoadImagesFromFolder()
    {
        string[] files = Directory.GetFiles(imagesPath, "*.png");
        foreach (string file in files)
        {
            byte[] fileData = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            textures.Add(texture);
        }
    }

    void SetupSlider()
    {
        imageSlider.maxValue = textures.Count - 1;
        imageSlider.onValueChanged.AddListener(delegate { UpdateImage(); });
        UpdateImage();
    }

    void UpdateImage()
    {
        int index = (int)imageSlider.value;
        rawImage.texture = textures[index];
    }

    void LoadImage()
    {
        rawImage.texture = textures[(int)imageSlider.value];
    }
}


