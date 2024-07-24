using UnityEngine;
using UnityEngine.UI;

public class BotonesPuzzle : MonoBehaviour
{
    public Canvas puzzleCanvas; // Canvas del juego del puzzle
    public Canvas menuCanvas; // Canvas del menú
    public Image puzzleImage; // Imagen completa del puzzle
    public Canvas winCanvas;
    public PuzzleCreator puzzleCreator; // Referencia al script PuzzleCreator
    public GameObject overlayPanel; // Panel transparente para detectar clics fuera de la imagen
    public Text timerText; // Texto UI para mostrar el temporizador
    public AudioSource buttonAudioSource; // AudioSource para los sonidos de los botones
    public AudioClip buttonClickSound; // Clip de sonido para el click del botón

    public Text movementText;

    private float elapsedTime = 0f; // Tiempo transcurrido en segundos
    private bool isTimerRunning = false; // Estado del temporizador

    private float winTime;

    public void Start()
    {
        overlayPanel.SetActive(false); // Asegurarse de que el panel esté inicialmente desactivado
        timerText.text = "Time: 0m 0s"; // Inicializar el texto del temporizador
        movementText.text = "Moves: 0";
    }

    public void Update()
    {
        movementText.text = "Moves: " + DraggablePiece.movementCounter;
        if (puzzleCanvas.isActiveAndEnabled && !winCanvas.isActiveAndEnabled)
        {
            isTimerRunning = true;
        }
        else if (winCanvas.isActiveAndEnabled)
        {
            isTimerRunning = false;
            winTime = elapsedTime;
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
        isTimerRunning = false; // Detener el temporizador
    }

    public void ReorganizarPiezas()
    {
        PlayButtonClickSound();
        puzzleCreator.CreatePuzzleWithShuffle(puzzleCreator.selectedImage, puzzleCreator.gridSize); // Reorganizar las piezas del puzzle
        ResetTimer(); // Reiniciar el temporizador
    }

    public void IniciarJuego()
    {
        PlayButtonClickSound();
        puzzleCanvas.gameObject.SetActive(true); // Activar el canvas del juego
        menuCanvas.gameObject.SetActive(false); // Desactivar el canvas del menú
        ResetTimer(); // Reiniciar el temporizador
        isTimerRunning = true; // Iniciar el temporizador
        DraggablePiece.movementCounter = 0; // Reiniciar el contador de movimientos
        movementText.text = "Moves: 0"; // Reiniciar el texto del contador de movimientos
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
    }

    // Método para reproducir el sonido de click del botón
    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}
