using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFace2x2 : MonoBehaviour
{
    int layerMask = 1 << 8;

    // Drag handling
    private bool isDragging = false;
    private Vector3 mouseDownPos;
    private float dragThreshold = 10f;
    private GameObject currentPiece;

    // Dynamic pivot management
    private Dictionary<string, GameObject> facePivots = new Dictionary<string, GameObject>();
    
    // Mapping of face letters to their axis directions
    private Dictionary<string, Vector3> faceAxes = new Dictionary<string, Vector3>();

    void Start()
    {
        CreateFacePivots();
    }

    void CreateFacePivots()
    {
        string[] faceNames = { "U", "D", "L", "R", "F", "B" };
        Vector3[] facePositions = {
            new Vector3(0, 1, 0),   // Up
            new Vector3(0, -1, 0),  // Down
            new Vector3(-1, 0, 0),  // Left
            new Vector3(1, 0, 0),   // Right
            new Vector3(0, 0, 1),   // Front
            new Vector3(0, 0, -1)   // Back
        };

        for (int i = 0; i < faceNames.Length; i++)
        {
            GameObject pivot = new GameObject(faceNames[i] + "_Pivot");
            pivot.transform.parent = transform;
            pivot.transform.localPosition = facePositions[i];
            pivot.layer = 0; // Default layer
            
            PivotRotation2x2 pivotRotation = pivot.AddComponent<PivotRotation2x2>();
            
            facePivots[faceNames[i]] = pivot;
            faceAxes[faceNames[i]] = facePositions[i].normalized;
            
            Debug.Log($"[2x2 Cube] Pivote creado: {faceNames[i]} en {facePositions[i]}");
        }
    }

    private Camera _mainCamera;
    Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    _mainCamera = FindObjectOfType<Camera>();
                }
            }
            return _mainCamera;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            mouseDownPos = Input.mousePosition;

            RaycastHit hit;
            if (MainCamera == null) return;

            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                string hitName = hitObject.name;
                
                // Only accept valid 2x2 cube piece names
                bool isValidPiece = hitName.Length == 3 &&
                                   (hitName.Contains("F") || hitName.Contains("B")) &&
                                   (hitName.Contains("U") || hitName.Contains("D")) &&
                                   (hitName.Contains("L") || hitName.Contains("R"));
                
                if (isValidPiece)
                {
                    currentPiece = hitObject;
                    Debug.Log($"[2x2 Cube] Pieza detectada: {currentPiece.name}");
                }
                else
                {
                    currentPiece = null;
                }
            }
            else
            {
                currentPiece = null;
            }
        }
        else if (Input.GetMouseButton(0) && !isDragging && currentPiece != null)
        {
            Vector3 mouseCurrentPos = Input.mousePosition;
            if (Vector3.Distance(mouseDownPos, mouseCurrentPos) > dragThreshold)
            {
                isDragging = true;

                // Check if any pivot is rotating
                PivotRotation2x2[] pivots = FindObjectsOfType<PivotRotation2x2>();
                foreach (var p in pivots)
                {
                    if (p.IsRotating())
                    {
                        isDragging = false;
                        Debug.Log("[2x2 Cube] Rotación cancelada - otra cara está rotando");
                        return;
                    }
                }

                RotateFace(currentPiece, mouseCurrentPos - mouseDownPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentPiece = null;
        }
    }

    void RotateFace(GameObject piece, Vector3 dragVector)
    {
        string pieceName = piece.name.ToUpper();
        
        // Extract the 3 faces this piece belongs to
        List<string> pieceFaces = new List<string>();
        foreach (char c in pieceName)
        {
            string face = c.ToString();
            if (facePivots.ContainsKey(face))
            {
                pieceFaces.Add(face);
            }
        }

        if (pieceFaces.Count != 3)
        {
            Debug.LogError($"[2x2 Cube] La pieza {pieceName} no tiene exactamente 3 caras");
            return;
        }

        Debug.Log($"[2x2 Cube] Pieza {pieceName} pertenece a caras: {string.Join(", ", pieceFaces)}");

        // Calculate drag direction in world space relative to the piece
        if (MainCamera == null) return;

        Vector3 pieceUp = piece.transform.up;
        Vector3 pieceRight = piece.transform.right;

        Vector3 screenUp = MainCamera.WorldToScreenPoint(piece.transform.position + pieceUp) - 
                          MainCamera.WorldToScreenPoint(piece.transform.position);
        Vector3 screenRight = MainCamera.WorldToScreenPoint(piece.transform.position + pieceRight) - 
                             MainCamera.WorldToScreenPoint(piece.transform.position);

        Vector2 dragDir = new Vector2(dragVector.x, dragVector.y).normalized;
        Vector2 screenUpDir = new Vector2(screenUp.x, screenUp.y).normalized;
        Vector2 screenRightDir = new Vector2(screenRight.x, screenRight.y).normalized;

        float dotUp = Vector2.Dot(dragDir, screenUpDir);
        float dotRight = Vector2.Dot(dragDir, screenRightDir);

        // Determine rotation axis based on drag direction
        Vector3 rotationAxis;
        if (Mathf.Abs(dotUp) > Mathf.Abs(dotRight))
        {
            // Vertical drag - rotate around horizontal axis
            rotationAxis = pieceRight;
        }
        else
        {
            // Horizontal drag - rotate around vertical axis
            rotationAxis = pieceUp;
        }

        Debug.Log($"[2x2 Cube] Dirección de arrastre: dotUp={dotUp:F2}, dotRight={dotRight:F2}");
        Debug.Log($"[2x2 Cube] Eje de rotación: {rotationAxis}");

        // Find which of the 3 faces has an axis aligned with the rotation axis
        string faceToRotate = null;
        float maxDot = 0.5f; // Threshold for alignment

        foreach (string face in pieceFaces)
        {
            Vector3 faceAxis = faceAxes[face];
            float dot = Mathf.Abs(Vector3.Dot(faceAxis, rotationAxis));
            
            Debug.Log($"[2x2 Cube] Cara {face}: eje {faceAxis}, dot con rotationAxis = {dot:F2}");
            
            if (dot > maxDot)
            {
                maxDot = dot;
                faceToRotate = face;
            }
        }

        if (faceToRotate == null)
        {
            Debug.LogWarning("[2x2 Cube] No se pudo determinar qué cara rotar");
            return;
        }

        Debug.Log($"[2x2 Cube] Cara seleccionada para rotar: {faceToRotate}");

        // Get all pieces for this face
        List<GameObject> facePieces = GetPiecesForFace(faceToRotate);
        
        if (facePieces.Count != 4)
        {
            Debug.LogError($"[2x2 Cube] La cara {faceToRotate} no tiene 4 piezas (tiene {facePieces.Count})");
            return;
        }

        // Get the pivot
        GameObject pivot = facePivots[faceToRotate];
        PivotRotation2x2 pivotRotation = pivot.GetComponent<PivotRotation2x2>();

        if (pivotRotation == null)
        {
            Debug.LogError($"[2x2 Cube] El pivote {faceToRotate} no tiene PivotRotation2x2");
            return;
        }

        Debug.Log($"[2x2 Cube] Rotando cara {faceToRotate} con piezas: {string.Join(", ", facePieces.ConvertAll(p => p.name))}");

        // Parent pieces to pivot
        foreach (GameObject p in facePieces)
        {
            p.transform.SetParent(pivot.transform, true);
        }

        // Rotate
        pivotRotation.Rotate(facePieces);
    }

    List<GameObject> GetPiecesForFace(string faceLetter)
    {
        List<GameObject> pieces = new List<GameObject>();
        
        foreach (Transform child in transform)
        {
            // Skip non-pieces
            if (child.name.Length != 3) continue;
            if (child.name.Contains("Pivot")) continue;
            
            // Check if piece name contains this face letter
            if (child.name.ToUpper().Contains(faceLetter.ToUpper()))
            {
                pieces.Add(child.gameObject);
            }
        }
        
        return pieces;
    }
}
