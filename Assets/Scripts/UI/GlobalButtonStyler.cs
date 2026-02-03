using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalButtonStyler : MonoBehaviour
{
    // Esta etiqueta hace que el método se ejecute automáticamente al inicio del juego
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        // Verificar si ya existe para evitar duplicados
        if (FindObjectOfType<GlobalButtonStyler>() != null) return;

        // Crear un GameObject persistente
        GameObject host = new GameObject("GlobalButtonStyler");
        DontDestroyOnLoad(host);
        host.AddComponent<GlobalButtonStyler>();
    }

    void Awake()
    {
        // Suscribirse al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Se llama cada vez que se carga una escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyStyleToAllButtons();
    }

    // Se llama al inicio (para la primera escena)
    void Start()
    {
        ApplyStyleToAllButtons();
    }

    public void ApplyStyleToAllButtons()
    {
        // Buscar todos los botones en la escena (incluso los inactivos si es posible, pero FindObjectsOfType halla activos)
        // Usamos Resources.FindObjectsOfTypeAll para encontrar todo, y filtramos los que están en la escena
        // O más simple: FindObjectsOfType<Button>() para los activos.
        
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (var btn in buttons)
        {
            // Ignorar si el botón es parte de un prefab en assets (no debería pasar con FindObjectsOfType)
            if (btn.gameObject.scene.name == null) continue;

            // Verificar si ya tiene el componente ButtonHighlighter
            if (btn.gameObject.GetComponent<ButtonHighlighter>() == null)
            {
                // Agregarlo
                btn.gameObject.AddComponent<ButtonHighlighter>();
            }
        }
    }
}
