using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPersistence : MonoBehaviour
{
    [SerializeField] private string noAudioSceneName = "Video";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == noAudioSceneName)
        {
            Destroy(gameObject);
        }
    }
}
