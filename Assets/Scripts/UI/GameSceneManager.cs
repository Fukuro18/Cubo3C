using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // Método para cargar cualquier escena por nombre
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Cargando escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Método específico para cargar la escena del cubo (opcional, si lo quieres mantener)
    public void LoadCubeScene()
    {
        LoadScene("Cube");
    }

    // Método para volver al menú principal
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    // Método para salir del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}