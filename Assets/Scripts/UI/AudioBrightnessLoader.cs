using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioBrightnessLoader : MonoBehaviour
{
    void Start()
    {
        // Cargar valores guardados (si no existen, usa 1f por defecto)
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        float brightness = PlayerPrefs.GetFloat("brightness", 1f);

        // Aplicar volumen global
        AudioListener.volume = volume;
        // Aplicar brillo global
        BrightnessManager.SetBrightness(brightness);

#if UNITY_EDITOR
        // Implementación de música desde Assets/Audio (Solo Editor)
        // Solo reproducir si estamos en el MainMenu
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            GameObject musicObj = GameObject.Find("BackgroundMusic");
            if (musicObj == null)
            {
                musicObj = new GameObject("BackgroundMusic");
                AudioSource audioSource = musicObj.AddComponent<AudioSource>();

                // Cargar archivo específico sin moverlo
                AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/musica.wav");

                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.loop = true;
                    audioSource.volume = volume;
                    audioSource.Play();

                    // Agregar el script que controla la persistencia inteligente
                    // Este script se encargará de "DontDestroyOnLoad" y de destruir el objeto si sale de las escenas permitidas
                    musicObj.AddComponent<MusicPersistence>();
                }
                else
                {
                    Debug.LogWarning("No se encontró 'Assets/Resources/Audio/usica.wav'.");
                }
            }
        }
#endif

        // Aplicar brillo global (luz ambiental)
        UpdateBrightness(brightness);
    }

    public void UpdateBrightness(float brightness)
    {
        // Delegar toda la lógica al Manager centralizado para evitar colisiones
        BrightnessManager.SetBrightness(brightness);
    }
}


/*{
    [Header("Música (Resources)")]
    [SerializeField] private string musicResourcePath = "Audio/musica";
    [SerializeField] private string musicObjectName = "BackgroundMusic";

    [Header("Escena SIN audio")]
    [SerializeField] private string noAudioSceneName = "Video";

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        float brightness = PlayerPrefs.GetFloat("brightness", 1f);

        // Si estamos en Video → no hay audio
        if (SceneManager.GetActiveScene().name == noAudioSceneName)
        {
            AudioListener.volume = 0f;
            DestroyExistingMusicIfAny();
            UpdateBrightness(brightness);
            return;
        }

        // En todas las demás escenas
        AudioListener.volume = volume;
        EnsureBackgroundMusicExists();
        UpdateBrightness(brightness);
    }

    private void EnsureBackgroundMusicExists()
    {
        if (GameObject.Find(musicObjectName) != null) return;

        GameObject musicObj = new GameObject(musicObjectName);
        AudioSource audioSource = musicObj.AddComponent<AudioSource>();

        AudioClip clip = Resources.Load<AudioClip>(musicResourcePath);
        if (clip == null)
        {
            Debug.LogWarning($"No se encontró Assets/Resources/{musicResourcePath}.wav");
            Destroy(musicObj);
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = 1f; // volumen real lo controla AudioListener
        audioSource.Play();

        musicObj.AddComponent<MusicPersistence>();
    }

    private void DestroyExistingMusicIfAny()
    {
        GameObject musicObj = GameObject.Find(musicObjectName);
        if (musicObj != null) Destroy(musicObj);
    }

    public void UpdateBrightness(float brightness)
    {
        BrightnessManager.SetBrightness(brightness);
    }
}*/
