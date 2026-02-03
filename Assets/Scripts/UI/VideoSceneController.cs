using UnityEngine;

/// <summary>
/// Controlador para la escena Video
/// Maneja la navegación desde la escena Video hacia otras escenas
/// </summary>
public class VideoSceneController : MonoBehaviour
{
    // Referencia al GameSceneManager
    public GameSceneManager gameSceneManager;

    void Start()
    {
        // Asegurar que el volumen se aplique al cargar la escena de Video
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        AudioListener.volume = volume;

        // Búsqueda robusta de VideoPlayer para forzar el volumen
        UnityEngine.Video.VideoPlayer[] players = FindObjectsOfType<UnityEngine.Video.VideoPlayer>();
        foreach (var player in players)
        {
            if (player.audioOutputMode == UnityEngine.Video.VideoAudioOutputMode.Direct)
            {
                player.SetDirectAudioVolume(0, volume);
            }
            else if (player.audioOutputMode == UnityEngine.Video.VideoAudioOutputMode.AudioSource)
            {
                AudioSource source = player.GetComponent<AudioSource>();
                if (source != null) source.volume = volume;
            }
        }

        // Si no se asignó en el Inspector, intentar encontrarlo en la escena
        if (gameSceneManager == null)
        {
            gameSceneManager = FindObjectOfType<GameSceneManager>();
            
            if (gameSceneManager == null)
            {
                Debug.LogError("No se encontró GameSceneManager en la escena Video. Asegúrate de agregarlo al GameObject.");
            }
        }
    }

    void Update()
    {
        // Enfoque "Fuerza Bruta": Asegurar que el volumen se respete siempre, incluso si el video carga tarde
        // Esto es muy útil si el VideoPlayer se inicializa asíncronamente
        float targetVolume = AudioListener.volume;
        
        UnityEngine.Video.VideoPlayer[] players = FindObjectsOfType<UnityEngine.Video.VideoPlayer>();
        foreach (var player in players)
        {
            if (player.audioOutputMode == UnityEngine.Video.VideoAudioOutputMode.Direct)
            {
                // Solo asignar si es diferente para no spammear el setter
                if (player.GetDirectAudioVolume(0) != targetVolume)
                {
                    player.SetDirectAudioVolume(0, targetVolume);
                }
            }
            else if (player.audioOutputMode == UnityEngine.Video.VideoAudioOutputMode.AudioSource)
            {
                AudioSource source = player.GetComponent<AudioSource>();
                if (source != null && source.volume != targetVolume) 
                {
                    source.volume = targetVolume;
                }
            }
        }
    }

    /// <summary>
    /// Método para regresar al menú principal desde la escena Video
    /// Este método debe ser llamado por el botón "Exit"
    /// </summary>
    public void ExitToMainMenu()
    {
        if (gameSceneManager != null)
        {
            Debug.Log("Regresando al MainMenu desde Video...");
            gameSceneManager.LoadMainMenu();
        }
        else
        {
            Debug.LogError("No se puede regresar al MainMenu: GameSceneManager no está asignado.");
        }
    }

    /// <summary>
    /// Método alternativo para cargar cualquier escena desde Video
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"No se puede cargar la escena {sceneName}: GameSceneManager no está asignado.");
        }
    }
}
