using UnityEngine;
using UnityEngine.UI;

public class PhysicalSliderToUISlider : MonoBehaviour
{
    [Header("Physical handle (lo que se mueve con la mano)")]
    [SerializeField] private Transform handle; // tu Button (handle)

    [Header("Rango del movimiento en X (local)")]
    [SerializeField] private float minLocalX;
    [SerializeField] private float maxLocalX;

    [Header("UI Slider destino")]
    [SerializeField] private Slider targetSlider;

    [Header("Opcional")]
    [SerializeField] private bool invert = false;
    [SerializeField] private bool useSetValueWithoutNotify = false;

    void Update()
    {
        if (handle == null || targetSlider == null) return;

        float x = handle.localPosition.x;

        float t = Mathf.InverseLerp(minLocalX, maxLocalX, x);
        if (invert) t = 1f - t;

        if (useSetValueWithoutNotify)
            targetSlider.SetValueWithoutNotify(t);
        else
            targetSlider.value = t; // <- Esto dispara OnValueChanged y tu SettingsController aplica el volumen
    }
}
