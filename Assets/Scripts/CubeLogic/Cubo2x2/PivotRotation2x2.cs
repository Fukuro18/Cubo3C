using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation2x2 : MonoBehaviour
{
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private bool autoRotating = false;
    private float speed = 300f;
    private Quaternion targetQuaternion;
    private Transform cubeTransform;

    void Start()
    {
        cubeTransform = transform.parent;
    }

    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        localForward = Vector3.zero - transform.localPosition;
        autoRotating = true;
        targetQuaternion = Quaternion.Euler(transform.localEulerAngles + localForward * 90f);
        Debug.Log("[PivotRotation2x2] Iniciando rotacion automatica de " + side.Count + " piezas");
    }

    void Update()
    {
        if (autoRotating)
        {
            AutoRotate();
        }
    }

    private void AutoRotate()
    {
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        if (Quaternion.Angle(transform.localRotation, targetQuaternion) < 1f)
        {
            transform.localRotation = targetQuaternion;
            Vector3 euler = transform.localEulerAngles;
            euler.x = Mathf.Round(euler.x / 90f) * 90f;
            euler.y = Mathf.Round(euler.y / 90f) * 90f;
            euler.z = Mathf.Round(euler.z / 90f) * 90f;
            transform.localEulerAngles = euler;
            UnParentAll();
            transform.localRotation = Quaternion.identity;
            autoRotating = false;
            Debug.Log("[PivotRotation2x2] Rotacion completada");
        }
    }

    private void UnParentAll()
    {
        if (activeSide == null || cubeTransform == null) return;
        foreach (GameObject piece in activeSide)
        {
            if (piece != null)
            {
                piece.transform.SetParent(cubeTransform, true);
            }
        }
        Debug.Log("[PivotRotation2x2] " + activeSide.Count + " piezas devueltas al cubo principal");
    }

    public bool IsRotating()
    {
        return autoRotating;
    }
}
