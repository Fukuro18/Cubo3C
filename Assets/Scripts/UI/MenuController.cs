using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Referencias a los paneles
    public GameObject menuPanel;
    public GameObject exitConfirmationPanel;

    // Referencia al script GameSceneManager
    public GameSceneManager gameSceneManager;

    void Start()
    {
        // Asegurarse de que los paneles estén ocultos al inicio
        if (menuPanel != null) menuPanel.SetActive(true);
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(false);
    }

    // Método para mostrar el panel de confirmación de salida
    public void ShowExitConfirmation()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(true);
    }

    // Método para volver al menú principal
    public void ReturnToMainMenu()
    {
        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    // Método para salir del juego
    public void QuitGame()
    {
        if (gameSceneManager != null)
        {
            gameSceneManager.QuitGame();
        }
        else
        {
            Debug.LogError("No se encontró GameSceneManager en la escena.");
        }
    }

    // Método para cargar la escena del cubo
    public void LoadCubeScene()
    {
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadScene("Cube");
        }
        else
        {
            Debug.LogError("No se encontró GameSceneManager en la escena.");
        }
    }

    // Método para cargar la escena de Video
    public void LoadVideoScene()
    {
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadScene("Video");
        }
        else
        {
            Debug.LogError("No se encontró GameSceneManager en la escena.");
        }
    }
}