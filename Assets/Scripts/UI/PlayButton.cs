using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayButton : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "Cube";

    // Componentes
    private Button buttonComponent;

    void Start()
    {
        // Obtener componentes
        buttonComponent = GetComponent<Button>();

        // Asegurar que tenga el efecto visual estandarizado
        if (GetComponent<ButtonHighlighter>() == null)
        {
            gameObject.AddComponent<ButtonHighlighter>();
        }

        // Configurar listener del botón
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnClick);
        }
    }

    // Lógica de carga de escena
    public void OnClick()
    {
        Debug.Log($"Botón PLAY presionado. Intentando cargar escena '{sceneToLoad}'...");
        
        GameSceneManager sceneManager = FindObjectOfType<GameSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("No se encontró GameSceneManager en la escena. Asegúrate de que existe un objeto con este script.");
        }
    }

     public void Press()
    {
        Debug.Log($"Botón PLAY presionado. Intentando cargar escena '{sceneToLoad}'...");

        GameSceneManager sceneManager = FindObjectOfType<GameSceneManager>();

        if (sceneManager != null)
            sceneManager.LoadScene(sceneToLoad);
        else
            Debug.LogError("No se encontró GameSceneManager en la escena.");
    }


}
