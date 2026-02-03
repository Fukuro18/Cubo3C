using UnityEngine;

/// <summary>
/// Script de debug para diagnosticar problemas con el sistema de interacción de manos
/// Agrégalo al mismo GameObject que tiene HandInteractionManager
/// </summary>
public class HandInteractionDebug : MonoBehaviour
{
    [Header("Referencias")]
    public HandTracking handTracking;
    public HandInteractionManager interactionManager;

    [Header("Debug Settings")]
    public bool showDebugLogs = true;
    public bool showGizmos = true;
    public float gizmoSize = 0.1f;

    void Start()
    {
        if (handTracking == null)
            handTracking = FindObjectOfType<HandTracking>();
        
        if (interactionManager == null)
            interactionManager = GetComponent<HandInteractionManager>();

        if (showDebugLogs)
        {
            Debug.Log("=== HAND INTERACTION DEBUG STARTED ===");
            Debug.Log($"HandTracking found: {handTracking != null}");
            Debug.Log($"InteractionManager found: {interactionManager != null}");
        }
    }

    void Update()
    {
        if (!showDebugLogs) return;

        // Check 1: ¿Hay datos de UDP?
        if (handTracking != null && handTracking.udpReceive != null)
        {
            bool hasData = !string.IsNullOrEmpty(handTracking.udpReceive.data);
            if (!hasData)
            {
                Debug.LogWarning("⚠️ NO HAY DATOS DE UDP - ¿Está corriendo el script de Python?");
                return;
            }
        }

        // Check 2: ¿Se detecta la mano?
        if (handTracking != null)
        {
            bool handDetected = handTracking.IsHandDetected();
            if (!handDetected)
            {
                Debug.LogWarning("⚠️ NO SE DETECTA MANO - Verifica MediaPipe");
                return;
            }
        }

        // Check 3: Mostrar estado de gestos cada 2 segundos
        if (Time.frameCount % 120 == 0 && interactionManager != null)
        {
            Debug.Log($"✋ ESTADO: Pinch={interactionManager.isPinching} | PalmOpen={interactionManager.isPalmOpen}");
            
            if (handTracking != null)
            {
                float pinchDist = handTracking.GetPinchDistance();
                Debug.Log($"🤏 Distancia Pinch: {pinchDist:F3} (threshold: {interactionManager.pinchThreshold})");
            }
        }

        // Check 4: Mostrar posición del puntero
        if (Time.frameCount % 60 == 0 && interactionManager != null)
        {
            Vector3 pointerPos = interactionManager.GetPointerPosition();
            Debug.Log($"👆 Posición del puntero: {pointerPos}");
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || handTracking == null) return;

        // Dibujar puntos clave de la mano
        if (handTracking.indexTip != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(handTracking.indexTip.position, gizmoSize);
        }

        if (handTracking.thumbTip != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(handTracking.thumbTip.position, gizmoSize);
        }

        if (handTracking.palmCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(handTracking.palmCenter.position, gizmoSize * 1.5f);
        }

        // Dibujar línea entre índice y pulgar (pinch)
        if (handTracking.indexTip != null && handTracking.thumbTip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(handTracking.indexTip.position, handTracking.thumbTip.position);
        }
    }
}
