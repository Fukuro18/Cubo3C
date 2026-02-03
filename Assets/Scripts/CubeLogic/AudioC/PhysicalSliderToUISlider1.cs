using UnityEngine;
using UnityEngine.UI;
using Leap.PhysicalHands;

public class PhysicalSliderToUISlider1 : MonoBehaviour
{
    public Slider uiSlider;

    private PhysicalHandsSlider physicalSlider;

    void Awake()
    {
        // Nos aseguramos de tomar el PhysicalHandsSlider del MISMO objeto
        physicalSlider = GetComponent<PhysicalHandsSlider>();
    }

    void Update()
    {
        if (!physicalSlider || !uiSlider) return;

        // ESTE valor refleja el movimiento del Cube
        uiSlider.value = physicalSlider.SliderValue;
    }
}
