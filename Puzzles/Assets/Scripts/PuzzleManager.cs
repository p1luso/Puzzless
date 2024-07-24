using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleCreator : MonoBehaviour
{
    public Transform puzzleParent; // Parent object to hold the puzzle pieces
    public GameObject puzzlePiecePrefab; // Prefab for a single puzzle piece
    public float gapThickness = 0.01f; // Gap between puzzle pieces
    public Sprite selectedImage; // Imagen seleccionada para el puzzle
    public int gridSize; // Tamaño de la grilla del puzzle
    public GameObject _winCanvas;
    public AudioSource winSoundSource;
    public AudioClip winClip;
    public AudioSource swapPiece; // AudioSource para el sonido de movimiento
    public AudioClip swapClip; // AudioClip para el sonido de movimiento
    public GameObject _canvas;
    public MusicManager musicManager; // Referencia al MusicManager
    public BotonesPuzzle puzzleButtons;
    [SerializeField] private List<DraggablePiece> pieces = new List<DraggablePiece>();
    [SerializeField] private List<Vector3> correctPositions = new List<Vector3>();
    private List<Sprite> correctSlices = new List<Sprite>();
    public Sprite fullPuzzleImage; // Imagen completa del puzzle

    public List<DraggablePiece> Pieces => pieces;

    private int pieceWidth; // Ancho para las piezas del rompecabezas
    private int pieceHeight; // Alto para las piezas del rompecabezas

    private bool puzzleCompleted = false; // Estado del rompecabezas

    private void Start()
    {
        _winCanvas.SetActive(false);
    }

    // Method to create puzzle without shuffling pieces
    public void CreatePuzzleWithoutShuffle(Sprite puzzleImage, int gridSize)
    {
        this.gridSize = gridSize;
        SetPieceDimensions(gridSize); // Ajustar las dimensiones de las piezas según el tamaño de la cuadrícula
        this.selectedImage = puzzleImage; // Guardar la imagen seleccionada
        this.fullPuzzleImage = ResizeSprite(puzzleImage, pieceWidth * gridSize, pieceHeight * gridSize); // Guardar la imagen completa redimensionada
        ClearPuzzle(); // Clear any existing puzzle pieces
        correctPositions.Clear(); // Clear correct positions
        correctSlices.Clear();
        pieces.Clear(); // Clear pieces list

        // Slice the puzzle image for correct positions
        SlicePuzzleImage(fullPuzzleImage, gridSize);

        // Create puzzle pieces and store correct positions
        for (int y = gridSize - 1; y >= 0; y--) // Start from bottom to top
        {
            for (int x = 0; x < gridSize; x++) // Left to right
            {
                // Create a new piece
                GameObject pieceObject = Instantiate(puzzlePiecePrefab, puzzleParent);
                DraggablePiece piece = pieceObject.AddComponent<DraggablePiece>();
                piece.Initialize(this);

                // Set the piece sprite
                Image pieceImage = piece.GetComponent<Image>();
                pieceImage.sprite = correctSlices[y * gridSize + x];

                // Set the piece position
                RectTransform pieceRect = piece.GetComponent<RectTransform>();
                pieceRect.sizeDelta = new Vector2(pieceWidth, pieceHeight);
                pieceRect.localPosition = new Vector2(-pieceWidth * gridSize / 2 + x * (pieceWidth + gapThickness) + pieceWidth / 2,
                                                      -pieceHeight * gridSize / 2 + y * (pieceHeight + gapThickness) + pieceHeight / 2);

                // Store initial correct position
                correctPositions.Add(pieceRect.localPosition);

                // Add a BoxCollider2D for mouse interaction
                pieceObject.AddComponent<BoxCollider2D>().size = pieceRect.sizeDelta;

                pieces.Add(piece);
            }
        }

        puzzleCompleted = false; // Reset puzzle completion state
    }

    // Method to create puzzle with shuffled pieces
    public void CreatePuzzleWithShuffle(Sprite puzzleImage, int gridSize)
    {
        CreatePuzzleWithoutShuffle(puzzleImage, gridSize);
        ShufflePieces();
    }

    void SlicePuzzleImage(Sprite puzzleImage, int gridSize)
    {
        float pieceWidth = puzzleImage.texture.width / gridSize;
        float pieceHeight = puzzleImage.texture.height / gridSize;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Sprite slice = Sprite.Create(puzzleImage.texture,
                                             new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight),
                                             new Vector2(0.5f, 0.5f));
                correctSlices.Add(slice);
            }
        }
    }

    void ClearPuzzle()
    {
        // Destroy all existing puzzle pieces
        foreach (Transform child in puzzleParent)
        {
            Destroy(child.gameObject);
        }
        pieces.Clear();
        correctPositions.Clear();
    }

    void ShufflePieces()
    {
        System.Random random = new System.Random();
        List<Vector3> shuffledPositions = new List<Vector3>(correctPositions);

        // Shuffle positions
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = random.Next(shuffledPositions.Count);
            Vector3 tempPosition = shuffledPositions[i];
            shuffledPositions[i] = shuffledPositions[randomIndex];
            shuffledPositions[randomIndex] = tempPosition;
        }

        // Assign shuffled positions to pieces
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].GetComponent<RectTransform>().localPosition = shuffledPositions[i];
        }
    }

    void Update()
    {
        if (_canvas.activeSelf && !puzzleCompleted)
        {
            CheckCompletion();
        }
    }

    public void CheckCompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            // Ensure the piece is close enough to its correct position
            if (Vector3.Distance(pieces[i].GetComponent<RectTransform>().localPosition, correctPositions[i]) > 0.1f)
            {
                return; // Not completed yet
            }
        }
        Debug.Log("Puzzle completed!");
        winSoundSource.PlayOneShot(winClip);
        puzzleCompleted = true; // Set puzzle completion state to true
        if (musicManager != null)
        {
            musicManager.PauseMusic(); // Pause the background music
        }

        StartCoroutine(DisablePuzzleInteractionWithDelay(0.5f)); // Desactivar interacciones del puzzle después de un retraso
    }

    private IEnumerator DisablePuzzleInteractionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var piece in pieces)
        {
            piece.GetComponent<DraggablePiece>().enabled = false;
            puzzleButtons.ShowWinCanvas();

        }
    }

    public void UpdatePiecePositions(DraggablePiece piece1, DraggablePiece piece2)
    {
        // Update references in the list
        int index1 = pieces.IndexOf(piece1);
        int index2 = pieces.IndexOf(piece2);
        pieces[index1] = piece2;
        pieces[index2] = piece1;
    }

    public void PlaySlideSound()
    {
        if (swapPiece != null && swapClip != null)
        {
            swapPiece.PlayOneShot(swapClip);
        }
    }

    private void SetPieceDimensions(int gridSize)
    {
        switch (gridSize)
        {
            case 3:
                pieceWidth = 285;
                pieceHeight = 400;
                break;
            case 4:
                pieceWidth = 214;
                pieceHeight = 300;
                break;
            case 5:
                pieceWidth = 171;
                pieceHeight = 240;
                break;
            default:
                Debug.LogError("Unsupported grid size");
                break;
        }
    }

    private Sprite ResizeSprite(Sprite originalSprite, int targetWidth, int targetHeight)
    {
        // Create a new texture with the desired dimensions
        Texture2D newTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
        
        // Create a RenderTexture and set it as active
        RenderTexture renderTexture = RenderTexture.GetTemporary(targetWidth, targetHeight);
        RenderTexture.active = renderTexture;
        
        // Render the original sprite texture to the RenderTexture
        Graphics.Blit(originalSprite.texture, renderTexture);
        
        // Read the pixels from the RenderTexture to the new texture
        newTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        newTexture.Apply();
        
        // Clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);

        // Create a new sprite from the resized texture
        return Sprite.Create(newTexture, new Rect(0, 0, targetWidth, targetHeight), new Vector2(0.5f, 0.5f));
    }
}
