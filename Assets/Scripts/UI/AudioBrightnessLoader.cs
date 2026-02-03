using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioBrightnessLoader : MonoBehaviour
{
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
}
