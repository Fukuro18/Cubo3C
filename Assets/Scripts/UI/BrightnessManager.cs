using UnityEngine;

public static class BrightnessManager
{
    /// <summary>
    /// Aplica el brillo globalmente:
    /// 1. Modifica RenderSettings.ambientLight para objetos 3D.
    /// 2. Gestiona un Canvas Overlay para oscurecer la UI y el resto de la pantalla.
    /// </summary>
    /// <param name="brightness">Valor entre 0.0 (Oscuro) y 1.0 (Brillante)</param>
    public static void SetBrightness(float brightness)
    {
        // 1. RenderSettings para objetos 3D
        RenderSettings.ambientLight = Color.white * brightness;

        // 2. Overlay para UI (Simular brillo reduciendo opacidad de un panel negro)
        UpdateOverlay(brightness);
    }

    private static void UpdateOverlay(float brightness)
    {
        // Buscamos o creamos el canvas de overlay
        GameObject overlayObj = GameObject.Find("BrightnessOverlayCanvas");
        UnityEngine.UI.Image overlayImage = null;

        if (overlayObj == null)
        {
            overlayObj = new GameObject("BrightnessOverlayCanvas");
            Canvas canvas = overlayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767; // Encima de todo
            Object.DontDestroyOnLoad(overlayObj);

            overlayObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            // Importante: No bloquear clicks
            overlayObj.AddComponent<UnityEngine.UI.GraphicRaycaster>().blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None; 

            GameObject panelObj = new GameObject("DarknessPanel");
            panelObj.transform.SetParent(overlayObj.transform, false);
            overlayImage = panelObj.AddComponent<UnityEngine.UI.Image>();
            overlayImage.color = Color.black;
            overlayImage.raycastTarget = false; // No bloquear clicks

            RectTransform rect = overlayImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        else
        {
            Transform panel = overlayObj.transform.Find("DarknessPanel");
            if (panel != null) overlayImage = panel.GetComponent<UnityEngine.UI.Image>();
        }

        // Ajustar opacidad: Brillo 1.0 -> Alpha 0.0 (Invisible). Brillo 0.0 -> Alpha 0.9 (Oscuro)
        if (overlayImage != null)
        {
            float alpha = 1.0f - brightness; 
            // Limitamos para que no sea negro total si bajan todo el brillo (max alpha 0.9)
            alpha = Mathf.Clamp(alpha, 0f, 0.9f);
            overlayImage.color = new Color(0, 0, 0, alpha);
        }
    }
}
