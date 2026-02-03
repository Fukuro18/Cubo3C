using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.PhysicalHands;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Botones del paso actual")]
    public List<PhysicalHandsButton> botonesPaso;

    [Header("Escena siguiente")]
    public string siguienteEscena;

    [Header("Overlay tutorial")]
    public GameObject tutorialOverlay;

    private int pasoActual = 0;

    // Flag que indica que el overlay fue ocultado por este manager (para mantener oculto al cambiar de escena)
    private bool overlayOcultoPorTutorial = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        BloquearTodos();
        ActivarSolo(0);

        // Intentar mostrar overlay (si está asignado o existe en la escena)
        MostrarOverlay();
    }

    public void BloquearTodos()
    {
        if (botonesPaso == null)
            return;

        foreach (var b in botonesPaso)
            b.enabled = false;   // ?? solo desactiva interacción
    }

    public void ActivarSolo(int index)
    {
        BloquearTodos();

        if (botonesPaso == null)
            return;

        if (index < 0 || index >= botonesPaso.Count)
            return;

        botonesPaso[index].enabled = true;  // ? solo este funciona

        // Al desbloquear un botón, ocultar los elementos del overlay inmediatamente
        OcultarOverlay();
    }

    public void SiguientePaso()
    {
        pasoActual++;

        if (BotonesValidos())
        {
            if (pasoActual >= botonesPaso.Count)
            {
                // Ocultar overlay cuando el tutorial termine
                OcultarOverlay();

                // Cargar la siguiente escena
                SceneManager.LoadScene(siguienteEscena);
            }
            else
            {
                ActivarSolo(pasoActual);
            }
        }
        else
        {
            // Si no hay botones configurados, igual podemos finalizar ocultando overlay y cargando escena
            OcultarOverlay();
            SceneManager.LoadScene(siguienteEscena);
        }

        // Ya no destruimos ni buscamos el overlay en cada paso.
    }

    // Métodos para mostrar/ocultar el overlay con comprobaciones seguras
    public void MostrarOverlay()
    {
        if (tutorialOverlay == null)
        {
            // Intentar encontrar por nombre si no fue asignado en el inspector
            tutorialOverlay = GameObject.Find("TutorialOverlay");
        }

        if (tutorialOverlay != null)
        {
            tutorialOverlay.SetActive(true);
            overlayOcultoPorTutorial = false;
        }
    }

    public void OcultarOverlay()
    {
        if (tutorialOverlay == null)
        {
            tutorialOverlay = GameObject.Find("TutorialOverlay");
        }

        if (tutorialOverlay != null)
        {
            tutorialOverlay.SetActive(false);
            overlayOcultoPorTutorial = true;
        }
    }

    // Evento llamado al cargar una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si el tutorial pidió ocultar el overlay, asegurar que el overlay de la nueva escena también quede oculto.
        if (overlayOcultoPorTutorial || HayBotonHabilitado())
        {
            // Rebuscar el objeto overlay en la nueva escena y ocultarlo si existe
            tutorialOverlay = GameObject.Find("TutorialOverlay");
            if (tutorialOverlay != null)
                tutorialOverlay.SetActive(false);
        }
    }

    // Método auxiliar para comprobar si la lista de botones está bien
    private bool BotonesValidos()
    {
        return botonesPaso != null && botonesPaso.Count > 0;
    }



        
    // Método auxiliar para comprobar si hay algún botón actualmente habilitado
    private bool HayBotonHabilitado()
    {
        if (botonesPaso == null)
            return false;

        foreach (var b in botonesPaso)
        {
            if (b != null && b.enabled)
                return true;
        }

        return false;
    }

}
