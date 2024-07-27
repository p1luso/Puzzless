using UnityEngine;
using UnityEngine.UI;

public class BotonesPuzzle : MonoBehaviour
{
    public Canvas puzzleCanvas; // Canvas del juego del puzzle
    public Canvas menuCanvas; // Canvas del menú
    public Image puzzleImage; // Imagen completa del puzzle
    public Canvas winCanvas; // Canvas de victoria
    public Canvas selecCanvas; // Canvas de selección

    public PuzzleCreator puzzleCreator; // Referencia al script PuzzleCreator
    public GameObject overlayPanel; // Panel transparente para detectar clics fuera de la imagen
    public Text timerText; // Texto UI para mostrar el temporizador
    public Text movementText; // Texto UI para mostrar los movimientos
    public Text winMovesText; // Texto UI para mostrar los movimientos en la pantalla de victoria
    public Text winTimeText; // Texto UI para mostrar el tiempo en la pantalla de victoria
    public Text winScoreText; // Texto UI para mostrar el puntaje total en la pantalla de victoria
    public AudioSource buttonAudioSource; // AudioSource para los sonidos de los botones
    public AudioClip buttonClickSound; // Clip de sonido para el click del botón
    public MusicManager musicManager;
    public Button musicToggleButton; // Botón para controlar la música de fondo
    public Sprite musicOnSprite; // Imagen del botón cuando la música está encendida
    public Sprite musicOffSprite; // Imagen del botón cuando la música está apagada

    private bool isMusicOn = true; // Estado de la música
    private float elapsedTime = 0f; // Tiempo transcurrido en segundos
    private bool isTimerRunning = false; // Estado del temporizador

    private void Start()
    {
        overlayPanel.SetActive(false); // Asegurarse de que el panel esté inicialmente desactivado
        timerText.text = "Time: 0m 0s"; // Inicializar el texto del temporizador
        movementText.text = "Moves: 0";
        musicToggleButton.onClick.AddListener(ToggleMusic); // Agregar el listener para el botón de música
        winCanvas.gameObject.SetActive(false); // Asegurarse de que el canvas de victoria esté inicialmente desactivado
    }

    private void Update()
    {
        if (puzzleCanvas.isActiveAndEnabled && !winCanvas.isActiveAndEnabled)
        {
            isTimerRunning = true;
            UpdateMovementText(); // Ensure movement text is updated while the puzzle is active
        }
        else if (winCanvas.isActiveAndEnabled)
        {
            isTimerRunning = false;
        }

        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            timerText.text = $"Time: {minutes}m {seconds}s";
        }
    }

    public void MostrarImagenCompleta()
    {
        PlayButtonClickSound();
        if (overlayPanel.activeSelf)
        {
            OcultarImagenCompleta(); // Ocultar la imagen completa si ya está visible
        }
        else
        {
            puzzleImage.sprite = puzzleCreator.fullPuzzleImage; // Usar la imagen completa almacenada en PuzzleCreator
            puzzleImage.gameObject.SetActive(true); // Mostrar la imagen completa
            overlayPanel.SetActive(true); // Activar el panel transparente
        }
    }

    public void VolverAlMenu()
    {
        PlayButtonClickSound();
        puzzleCanvas.gameObject.SetActive(false); // Desactivar el canvas del juego
        menuCanvas.gameObject.SetActive(true); // Activar el canvas del menú
        winCanvas.gameObject.SetActive(false);
        isTimerRunning = false; // Detener el temporizador
        ResetTimer(); // Reiniciar el temporizador y contador de movimientos al volver al menú
    }

    public void VolverAlMenuSeleccion()
    {
        PlayButtonClickSound();
        menuCanvas.gameObject.SetActive(false); // Desactivar el canvas del juego
        selecCanvas.gameObject.SetActive(true); // Activar el canvas del menú de selección
        winCanvas.gameObject.SetActive(false);
        isTimerRunning = false; // Detener el temporizador
        ResetTimer(); // Reiniciar el temporizador y contador de movimientos al volver al menú
    }

    public void ReorganizarPiezas()
    {
        PlayButtonClickSound();
        puzzleCreator.CreatePuzzleWithShuffle(puzzleCreator.selectedImage, puzzleCreator.gridSize); // Reorganizar las piezas del puzzle
        // No reiniciar el temporizador ni el contador de movimientos al hacer shuffle
    }

    public void IniciarJuego()
    {
        PlayButtonClickSound();
        puzzleCanvas.gameObject.SetActive(true); // Activar el canvas del juego
        menuCanvas.gameObject.SetActive(false); // Desactivar el canvas del menú
        ResetTimer(); // Reiniciar el temporizador y contador de movimientos al iniciar el juego
        isTimerRunning = true; // Iniciar el temporizador
    }

    // Método para ocultar la imagen completa cuando no se necesite
    public void OcultarImagenCompleta()
    {
        puzzleImage.gameObject.SetActive(false); // Ocultar la imagen completa
        overlayPanel.SetActive(false); // Desactivar el panel transparente
    }

    private void ResetTimer()
    {
        elapsedTime = 0f; // Reiniciar el tiempo transcurrido
        timerText.text = "Time: 0m 0s"; // Reiniciar el texto del temporizador
        DraggablePiece.movementCounter = 0; // Reiniciar el contador de movimientos
        movementText.text = "Moves: 0"; // Reiniciar el texto del contador de movimientos
    }

    // Método para actualizar el texto de movimientos
    public void UpdateMovementText()
    {
        movementText.text = "Moves: " + DraggablePiece.movementCounter;
    }

    // Método para reproducir el sonido de click del botón
    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Método para alternar la música de fondo
    private void ToggleMusic()
    {
        if (musicManager != null)
        {
            if (isMusicOn)
            {
                musicManager.PauseMusic();
                musicToggleButton.image.sprite = musicOffSprite; // Cambiar a imagen de música apagada
            }
            else
            {
                musicManager.ResumeMusic();
                musicToggleButton.image.sprite = musicOnSprite; // Cambiar a imagen de música encendida
            }
            isMusicOn = !isMusicOn;
        }
    }

    public void ShowWinCanvas()
    {
        winCanvas.gameObject.SetActive(true); // Activar el canvas de victoria
        isTimerRunning = false; // Detener el temporizador

        // Mostrar los movimientos y el tiempo en la pantalla de victoria
        winMovesText.text = "Moves: " + DraggablePiece.movementCounter;

        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        winTimeText.text = $"Time: {minutes}m {seconds}s";

        // Mostrar el puntaje total
        winScoreText.text = "Total Score: " + ScoreManager.Instance.TotalScore;
    }
}
