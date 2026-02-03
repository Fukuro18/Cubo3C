using UnityEngine;
using System.Collections;



public class RotateBigCubeLeap : MonoBehaviour
/*{
    [Header("Referencias")]
    public Transform cube;

    [Header("Sensibilidad Leap")]
    public float sensitivity = 0.02f;     // Movimiento mínimo para detectar swipe
    public float rotationSpeed = 250f;

    [Header("Opciones")]
    public bool allowHand = false;         // Permitir palma además del dedo

    private bool isRotating = false;
    private Vector3 startPos;
    private Vector3 rotationAxis;
    private float rotationAngle;

    private Rigidbody rb;

    void Start()
    {
        rb = cube.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Finger") && !(allowHand && other.CompareTag("Hand")))
            return;

        startPos = other.transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        // 🔒 BLOCK IF INTERACTING WITH A SPECIFIC FACE
        if (SelectFaceLeap.IsActive) return;

        // 🔒 BLOQUEOS IMPORTANTES
        if (CubeState.autoRotating) return;
        if (isRotating) return;
        if (!other.CompareTag("Finger") && !(allowHand && other.CompareTag("Hand")))
            return;

        Vector3 delta = other.transform.position - startPos;

        if (delta.magnitude < sensitivity)
            return;

        // Detectar eje dominante del movimiento
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            rotationAxis = Vector3.up;
            rotationAngle = delta.x > 0 ? -90f : 90f;
        }
        else if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) > Mathf.Abs(delta.z))
        {
            rotationAxis = Vector3.right;
            rotationAngle = delta.y > 0 ? 90f : -90f;
        }
        else
        {
            rotationAxis = Vector3.forward;
            rotationAngle = delta.z > 0 ? 90f : -90f;
        }

        CubeState.autoRotating = true;
        StartCoroutine(RotateCube(rotationAxis, rotationAngle));
    }

    IEnumerator RotateCube(Vector3 axis, float angle)
    {
        isRotating = true;

        if (rb != null)
            rb.isKinematic = true;

        Quaternion startRot = cube.rotation;
        Quaternion endRot = Quaternion.AngleAxis(angle, axis) * startRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed / 90f;
            cube.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // 🔧 Alineación exacta a múltiplos de 90°
        Vector3 euler = cube.eulerAngles;
        cube.rotation = Quaternion.Euler(
            Mathf.Round(euler.x / 90f) * 90f,
            Mathf.Round(euler.y / 90f) * 90f,
            Mathf.Round(euler.z / 90f) * 90f
        );

        if (rb != null)
            rb.isKinematic = false;

        isRotating = false;
        CubeState.autoRotating = false;
    }
}*/

{
    public Transform cube;
    public float rotationSpeed = 300f;
    public float sensitivity = 0.015f;

    private Vector3 lastHandPos;
    private bool rotating = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hand")) return;
        lastHandPos = other.transform.position;
        rotating = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
            rotating = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!rotating || !other.CompareTag("Hand")) return;

        Vector3 delta = other.transform.position - lastHandPos;

        if (delta.magnitude < sensitivity) return;

        float rotX = -delta.y * rotationSpeed;
        float rotY = delta.x * rotationSpeed;

        cube.Rotate(Vector3.up, rotY, Space.World);
        cube.Rotate(Vector3.right, rotX, Space.World);

        lastHandPos = other.transform.position;
    }
}

/*
{
    [Header("Configuración")]
    public Transform cube;
    public float sensitivity = 0.02f;     // Cuánto debe moverse la mano para iniciar rotación
    public float rotationSpeed = 250f;

    [Header("Opciones de detección")]
    public bool allowHand = false;        // Si true, puede usar la palma además del dedo

    private bool isRotating = false;
    private Vector3 startPos;
    private Vector3 rotationAxis;
    private float rotationAngle;

    private Rigidbody rb;

    void Start()
    {
        rb = cube.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Finger") && !(allowHand && other.CompareTag("Hand"))) return;

        startPos = other.transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        if (isRotating) return;
        if (!other.CompareTag("Finger") && !(allowHand && other.CompareTag("Hand"))) return;

        Vector3 delta = other.transform.position - startPos;

        if (delta.magnitude < sensitivity) return;

        // Determinar el eje principal de movimiento
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
        {
            rotationAxis = Vector3.up;
            rotationAngle = delta.x > 0 ? -90f : 90f;
        }
        else if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) > Mathf.Abs(delta.z))
        {
            rotationAxis = Vector3.right;
            rotationAngle = delta.y > 0 ? 90f : -90f;
        }
        else
        {
            rotationAxis = Vector3.forward;
            rotationAngle = delta.z > 0 ? 90f : -90f;
        }

        StartCoroutine(RotateCube(rotationAxis, rotationAngle));
    }

    IEnumerator RotateCube(Vector3 axis, float angle)
    {
        isRotating = true;

        if (rb != null) rb.isKinematic = true;

        Quaternion startRot = cube.rotation;
        Quaternion endRot = Quaternion.AngleAxis(angle, axis) * startRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed / 90f;
            cube.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // Redondeo exacto a 90° para que quede alineado
        Vector3 euler = cube.eulerAngles;
        cube.rotation = Quaternion.Euler(
            Mathf.Round(euler.x / 90) * 90,
            Mathf.Round(euler.y / 90) * 90,
            Mathf.Round(euler.z / 90) * 90
        );

        if (rb != null) rb.isKinematic = false;
        isRotating = false;
    }
}



public class RotateBigCubeLeap : MonoBehaviour
{
    public Transform cube;
    public float pushThreshold = 0.015f;
    public float rotationSpeed = 250f;

    private Vector3 lastFingerPos;
    private bool fingerInside = false;
    private bool isRotating = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Finger")) return;

        lastFingerPos = other.transform.position;
        fingerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Finger")) return;

        fingerInside = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!fingerInside || isRotating) return;
        if (!other.CompareTag("Finger")) return;

        Vector3 delta = other.transform.position - lastFingerPos;

        // empuje horizontal
        if (Mathf.Abs(delta.x) > pushThreshold)
        {
            StartCoroutine(Rotate(Vector3.up, delta.x > 0 ? -90 : 90));
            fingerInside = false;
        }
        // empuje vertical
        else if (Mathf.Abs(delta.y) > pushThreshold)
        {
            StartCoroutine(Rotate(Vector3.right, delta.y > 0 ? 90 : -90));
            fingerInside = false;
        }

        lastFingerPos = other.transform.position;
    }

    IEnumerator Rotate(Vector3 axis, float angle)
    {
        isRotating = true;

        Quaternion start = cube.rotation;
        Quaternion end = Quaternion.AngleAxis(angle, axis) * start;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed / 90f;
            cube.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        cube.rotation = end;
        isRotating = false;
    }
}*/


/*public class RotateBigCubeLeap : MonoBehaviour
{
    public Transform cube;
    public float swipeThreshold = 0.05f;
    public float rotationSpeed = 240f;

    private Vector3 lastPos;
    private bool tracking = false;
    private bool isRotating = false;

    void Update()
    {
        if (isRotating) return;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finger"))
        {
            lastPos = other.transform.position;
            tracking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Finger"))
        {
            tracking = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!tracking || isRotating) return;
        if (!other.CompareTag("Finger")) return;

        Vector3 delta = other.transform.position - lastPos;

        // Ignorar micro-movimientos
        if (delta.magnitude < swipeThreshold)
            return;

        // Horizontal
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
                StartCoroutine(RotateCube(Vector3.up, -90f));
            else
                StartCoroutine(RotateCube(Vector3.up, 90f));
        }
        // Vertical
        else
        {
            if (delta.y > 0)
                StartCoroutine(RotateCube(Vector3.right, 90f));
            else
                StartCoroutine(RotateCube(Vector3.right, -90f));
        }

        tracking = false; // 🔒 IMPORTANTE: evita repetición
    }

    IEnumerator RotateCube(Vector3 axis, float angle)
    {
        isRotating = true;

        Quaternion startRot = cube.rotation;
        Quaternion endRot = Quaternion.AngleAxis(angle, axis) * startRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed / 90f;
            cube.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        cube.rotation = endRot;
        isRotating = false;
    }
}*/


