using System.Collections.Generic;
using UnityEngine;

public class StepTutorialManager : MonoBehaviour
{
    [Header("Grupos por paso (paneles o botones)")]
    public List<GameObject> pasos;

    int pasoActual = 0;

    void Start()
    {
        ActivarPaso(0);
    }

    void ActivarPaso(int index)
    {
        for (int i = 0; i < pasos.Count; i++)
        {
            bool activo = (i == index);

            // Activa o desactiva TODO el grupo
            SetGrupoInteractivo(pasos[i], activo);
        }
    }

    void SetGrupoInteractivo(GameObject grupo, bool activo)
    {
        var botones = grupo.GetComponentsInChildren<Leap.PhysicalHands.PhysicalHandsButton>(true);

        foreach (var b in botones)
            b.enabled = activo;

        // highlight opcional
        var highlights = grupo.GetComponentsInChildren<ButtonHighlight>(true);

        foreach (var h in highlights)
            h.enabled = activo;
    }

    public void SiguientePaso()
    {
        pasoActual++;

        if (pasoActual >= pasos.Count)
            return;

        ActivarPaso(pasoActual);
    }
}


/*using Leap.PhysicalHands;

public class StepTutorialManager : MonoBehaviour
{
    [Header("Grupos por paso (paneles o botones)")]
    public List<GameObject> pasos;

    int pasoActual = 0;

    bool tutorialHecho;

    void Start()
    {
        // ?? clave DIFERENTE al menú
        tutorialHecho = PlayerPrefs.GetInt("TutorialCuboHecho", 0) == 1;

        if (tutorialHecho)
        {
            DesbloquearTodo();
            return;
        }

        ActivarPaso(0);
    }

    // =============================
    void ActivarPaso(int index)
    {
        for (int i = 0; i < pasos.Count; i++)
        {
            bool activo = (i == index);
            SetGrupoInteractivo(pasos[i], activo);
        }
    }

    // =============================
    void SetGrupoInteractivo(GameObject grupo, bool activo)
    {
        var botones = grupo.GetComponentsInChildren<PhysicalHandsButton>(true);

        foreach (var b in botones)
            b.enabled = activo;

        var highlights = grupo.GetComponentsInChildren<ButtonHighlight>(true);

        foreach (var h in highlights)
            h.enabled = activo;
    }

    // =============================
    void DesbloquearTodo()
    {
        foreach (var g in pasos)
        {
            var botones = g.GetComponentsInChildren<PhysicalHandsButton>(true);
            foreach (var b in botones)
                b.enabled = true;

            var highlights = g.GetComponentsInChildren<ButtonHighlight>(true);
            foreach (var h in highlights)
                h.enabled = false; // ?? apagar animación
        }
    }

    // =============================
    public void SiguientePaso()
    {
        pasoActual++;

        // ?? terminó el tutorial del cubo
        if (pasoActual >= pasos.Count)
        {
            PlayerPrefs.SetInt("TutorialCuboHecho", 1);
            PlayerPrefs.Save();

            DesbloquearTodo();
            return;
        }

        ActivarPaso(pasoActual);
    }
}*/
