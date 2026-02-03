using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UISliderRotateCube : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cubeRoot;
    public Slider slider;
    public float rotationMultiplier = 360f;

    private float lastValue;

    void Start()
    {
       lastValue = slider.value;
       slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        float delta = value - lastValue;
        lastValue = value;

        cubeRoot.Rotate(Vector3.up, delta * rotationMultiplier, Space.World);
    }
}

