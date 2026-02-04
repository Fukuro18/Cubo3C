using UnityEngine;

/// <summary>
/// Script de prueba para el cubo 2x2. Adjunta este script al GameObject del cubo
/// para probar la funcionalidad de mezcla desde el Inspector o mediante teclas.
/// </summary>
public class TestCube2x2 : MonoBehaviour
{
    private CubeState2x2 cubeState;

    [Header("Shuffle Settings")]
    [Tooltip("Numero de movimientos aleatorios para mezclar")]
    public int shuffleMoves = 20;

    [Header("Keyboard Controls")]
    [Tooltip("Presiona esta tecla para mezclar el cubo")]
    public KeyCode shuffleKey = KeyCode.Space;

    void Start()
    {
        cubeState = GetComponent<CubeState2x2>();
        
        if (cubeState == null)
        {
            Debug.LogError("[TestCube2x2] No se encontro CubeState2x2 en este GameObject");
        }
        else
        {
            Debug.Log("[TestCube2x2] Presiona ESPACIO para mezclar el cubo");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(shuffleKey))
        {
            ShuffleCube();
        }
    }

    /// <summary>
    /// Mezcla el cubo. Puedes llamar este metodo desde un boton UI o desde codigo.
    /// </summary>
    public void ShuffleCube()
    {
        if (cubeState == null)
        {
            Debug.LogError("[TestCube2x2] CubeState2x2 no esta asignado");
            return;
        }

        if (cubeState.IsShuffling())
        {
            Debug.LogWarning("[TestCube2x2] El cubo ya se esta mezclando");
            return;
        }

        Debug.Log($"[TestCube2x2] Mezclando cubo con {shuffleMoves} movimientos");
        cubeState.Shuffle(shuffleMoves);
    }

    /// <summary>
    /// Detiene la mezcla actual
    /// </summary>
    public void StopShuffle()
    {
        if (cubeState != null)
        {
            cubeState.StopShuffle();
        }
    }
}
