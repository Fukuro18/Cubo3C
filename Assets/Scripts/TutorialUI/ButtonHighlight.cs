
using UnityEngine;

public class ButtonHighlight : MonoBehaviour
{
    public float speed = 3f;
    public float scaleAmount = 0.1f;

    Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = baseScale * scale;
    }

    void OnDisable()
    {
        transform.localScale = baseScale; // resetear
    }
}

