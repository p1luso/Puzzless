using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour
{
    [Header("Content Viewport")]
    public Image contentDisplay;
    public Transform contentParent;
    public GameObject contentPanelPrefab;

    [Header("Pagination Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Puzzle Buttons")]
    public Button button3x3;
    public Button button4x4;
    public Button button5x5;

    [Header("Page Settings")]
    public bool useTimer = false;
    public bool isLimitedSwipe = false;
    public float autoMoveTime = 5f;
    private float timer;
    public int currentIndex = 0;
    public float swipeThreshold = 50f;
    private Vector2 touchStartPos;

    [Header("Canvas")] 
    [SerializeField] private GameObject _canvasSelection;
    [SerializeField] private GameObject _canvasPuzzle;
    [SerializeField] private GameObject _canvasCategories;

    // Reference to the RectTransform of the content area
    public RectTransform contentArea;

    // Reference to the selected image
    public Sprite selectedImage;

    // Reference to the PuzzleCreator
    public PuzzleCreator puzzleCreator;

    [Header("Audio")]
    public AudioSource buttonAudioSource; // AudioSource para los sonidos de los botones
    public AudioClip buttonClickSound; // Clip de sonido para el click del botón

    //Score manager
    public ScoreManager scoreManager;
    // Lista para almacenar los paneles de contenido
    private List<GameObject> contentPanels = new List<GameObject>();

    void Start()
    {
        _canvasSelection.SetActive(false);
        _canvasPuzzle.SetActive(false);
        _canvasCategories.SetActive(true);

        // Vincular métodos que reproducen sonido a los eventos onClick de los botones
        nextButton.onClick.AddListener(() => { PlayButtonClickSound(); NextContent(); });
        prevButton.onClick.AddListener(() => { PlayButtonClickSound(); PreviousContent(); });
        button3x3.onClick.AddListener(() => { PlayButtonClickSound(); StartPuzzle(3); scoreManager.InitializeScore(1); });
        button4x4.onClick.AddListener(() => { PlayButtonClickSound(); StartPuzzle(4); scoreManager.InitializeScore(2);});
        button5x5.onClick.AddListener(() => { PlayButtonClickSound(); StartPuzzle(5); scoreManager.InitializeScore(3);});

        // Display initial content
        ShowContent();

        // Start auto-move timer if enabled
        if (useTimer)
        {
            timer = autoMoveTime;
            InvokeRepeating("AutoMoveContent", 1f, 1f); // Invoke every second to update the timer
        }
    }

    void Update()
    {
        // Detect swipe input only within the content area
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 touchEndPos = Input.mousePosition;
            float swipeDistance = touchEndPos.x - touchStartPos.x;

            // Check if the swipe is within the content area bounds
            if (Mathf.Abs(swipeDistance) > swipeThreshold && IsTouchInContentArea(touchStartPos))
            {
                if (isLimitedSwipe && ((currentIndex == 0 && swipeDistance > 0) || (currentIndex == contentPanels.Count - 1 && swipeDistance < 0)))
                {
                    // Limited swipe is enabled, and at the edge of content
                    return;
                }

                if (swipeDistance > 0)
                {
                    PreviousContent();
                }
                else
                {
                    NextContent();
                }
            }
        }
    }

    // Check if the touch position is within the content area bounds
    bool IsTouchInContentArea(Vector2 touchPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(contentArea, touchPosition);
    }

    void AutoMoveContent()
    {
        timer -= 1f; // Decrease timer every second

        if (timer <= 0)
        {
            timer = autoMoveTime;
            NextContent();
        }
    }

    void NextContent()
    {
        currentIndex = (currentIndex + 1) % contentPanels.Count;
        ShowContent();
    }

    void PreviousContent()
    {
        currentIndex = (currentIndex - 1 + contentPanels.Count) % contentPanels.Count;
        ShowContent();
    }

    void ShowContent()
    {
        // Activate the current panel and deactivate others
        for (int i = 0; i < contentPanels.Count; i++)
        {
            bool isActive = i == currentIndex;
            contentPanels[i].SetActive(isActive);

            if (isActive)
            {
                // Update the selected image
                selectedImage = contentPanels[i].GetComponentInChildren<Image>().sprite;

                // Reset timer and fill amount when the content is swiped
                timer = autoMoveTime;
            }
        }
    }

    public void SetCurrentIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < contentPanels.Count)
        {
            currentIndex = newIndex;
            ShowContent();
        }
    }

    void StartPuzzle(int gridSize)
    {
        if (selectedImage != null && puzzleCreator != null)
        {
            _canvasSelection.SetActive(false);
            _canvasPuzzle.SetActive(true);
            // Pass the selected image and grid size to the PuzzleCreator
            puzzleCreator.CreatePuzzleWithShuffle(selectedImage, gridSize);
        }
    }

    // Método para reproducir el sonido de click del botón
    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Método para cargar imágenes desde una carpeta
    internal void LoadImagesFromFolder(string folderPath)
    {
        // Asegurarse de que el canvas de selección esté activo
        _canvasSelection.SetActive(true);

        // Limpiar los paneles actuales
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        contentPanels.Clear(); // Clear the list of panels

        // Obtener archivos de imagen en la carpeta
        string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

        foreach (string file in imageFiles)
        {
            // Crear un nuevo panel
            GameObject newPanel = Instantiate(contentPanelPrefab, contentParent);

            // Cargar la imagen
            byte[] imageData = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Asignar la imagen al panel
            Image imageComponent = newPanel.GetComponentInChildren<Image>();
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Ajustar tamaño y opacidad
            RectTransform rectTransform = imageComponent.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(793, 816); // Ajustar tamaño
            imageComponent.color = new Color(1f, 1f, 1f, 1f); // 100% opacidad

            imageComponent.enabled = true; // Asegurarse de que el componente Image esté activado

            // Añadir el panel a la lista
            contentPanels.Add(newPanel);
        }

        // Mostrar el primer contenido
        ShowContent();
    }

    // Métodos para botones de categorías
    public void ShowAnimals()
    {
        LoadImagesFromFolder(Path.Combine(Application.dataPath, "Imagenes/Animals"));
        _canvasCategories.SetActive(false);
    }

    public void ShowAbstract()
    {
        LoadImagesFromFolder(Path.Combine(Application.dataPath, "Imagenes/Abstract"));
        _canvasCategories.SetActive(false);
    }

    public void ShowFantasy()
    {
        LoadImagesFromFolder(Path.Combine(Application.dataPath, "Imagenes/Fantasy"));
        _canvasCategories.SetActive(false);
    }

    public void ShowLandscapes()
    {
        LoadImagesFromFolder(Path.Combine(Application.dataPath, "Imagenes/Landscapes"));
        _canvasCategories.SetActive(false);
    }
}
