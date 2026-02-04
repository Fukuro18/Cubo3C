using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFace2x2 : MonoBehaviour
{
    int layerMask = 1 << 8;

    // Reference to User's custom rays
    private ReadCube2x2 readCube;

    // Drag handling
    private bool isDragging = false;
    private Vector3 mouseDownPos;
    private float dragThreshold = 5f; // Reduced for better sensitivity
    private GameObject currentPiece;

    // Dynamic pivot management
    private Dictionary<string, GameObject> facePivots = new Dictionary<string, GameObject>();
    
    // Mapping of face letters to their axis directions
    private Dictionary<string, Vector3> faceAxes = new Dictionary<string, Vector3>();

    void Start()
    {
        Debug.Log("[2x2 Cube] SelectFace2x2 iniciado y habilitado.");
        
        readCube = GetComponent<ReadCube2x2>();
        if (readCube == null)
        {
             readCube = FindObjectOfType<ReadCube2x2>();
        }
        
        if (readCube == null)
        {
            Debug.LogError("[2x2 Cube] CRÍTICO: No se encontró el script 'ReadCube2x2'. Se necesitan sus rayos para detectar las caras.");
        }
        else
        {
            Debug.Log("[2x2 Cube] 'ReadCube2x2' encontrado exitosamente. Se usarán sus transformaciones como guía, pero se crearán PIVOTES FÍSICOS CENTRADOS.");
            CreatePhysicsPivots();
        }
    }



    void CreatePhysicsPivots()
    {
        // Users rays are used for SELECTION (readCube), but for ROTATION we need pivots at (0,0,0)
        string[] faceNames = { "U", "D", "L", "R", "F", "B" };
        
        foreach (string face in faceNames)
        {
            GameObject pivot = new GameObject(face + "_Pivot_Physics");
            pivot.transform.parent = transform;
            pivot.transform.localPosition = Vector3.zero; // CENTERED at (0,0,0)
            pivot.layer = 0;
            
            PivotRotation2x2 pivotRotation = pivot.AddComponent<PivotRotation2x2>();
            
            facePivots[face] = pivot;
        }

        // Define standard axes for these faces (World Space assumption)
        faceAxes["U"] = Vector3.up;
        faceAxes["D"] = Vector3.down;
        faceAxes["L"] = Vector3.left;
        faceAxes["R"] = Vector3.right;
        faceAxes["F"] = Vector3.forward;
        faceAxes["B"] = Vector3.back;
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
            // Debug: Temporarily disabled UI check to rule out false positives
            /*
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("[2x2 Cube] El click fue bloqueado por un elemento de UI.");
                return;
            }
            */

            mouseDownPos = Input.mousePosition;

            RaycastHit hit;
            
            // Ensure camera exists
            if (MainCamera == null) 
            {
                 Debug.LogError("[2x2 Cube] CRÍTICO: MainCamera es NULL. Etiqueta tu cámara principal como 'MainCamera'.");
                 return;
            }

            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            
            // VISUAL DEBUG: Draw the ray in the scene view for 2 seconds (RED COLOR)
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 2.0f);
            
            // Check ANY layer, infinite distance
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.collider.gameObject;
                string hitName = hitObject.name;
                int hitLayer = hitObject.layer;
                
                Debug.Log($"[2x2 Cube] ¡RAYCAST EXITOSO! Objeto: '{hitName}' | Layer: {hitLayer} | Distancia: {hit.distance}");

                // Check naming convention
                // First, check if the hit object itself is the piece
                GameObject finalPiece = hitObject;
                bool foundPiece = IsValidPieceName(finalPiece.name);

                // If not, try to find a parent that is a valid piece
                if (!foundPiece)
                {
                    Transform parentPoints = hitObject.transform.parent;
                    while (parentPoints != null)
                    {
                        if (IsValidPieceName(parentPoints.name))
                        {
                            finalPiece = parentPoints.gameObject;
                            foundPiece = true;
                            break;
                        }
                        parentPoints = parentPoints.parent;
                    }
                }
                
                // Warn about layer 8
                if ((1 << hitLayer & layerMask) == 0)
                {
                     Debug.LogWarning($"[2x2 Cube] IMPORTANTE: El objeto '{hitName}' NO está en la layer del script (Layer 8). El script espera Layer 8. Asegúrate de cambiar la Layer del objeto.");
                }

                if (foundPiece)
                {
                    currentPiece = finalPiece;
                    
                    // Determine color: Try by name first (e.g. "Front" -> Green)
                    string faceColor = GetColorFromObjectName(hitObject.name);
                    
                    // 2. PREFERRED: Use User's Read Rays from ReadCube2x2
                    if (readCube != null)
                    {
                         string userRayColor = GetFaceFromUserRays(hit.normal);
                         // Use user ray result if valid
                         if (userRayColor != "Unknown")
                         {
                             // If we already had a color from name, log if they differ (debug info)
                             if (faceColor != "Unknown" && faceColor != userRayColor)
                             {
                                 Debug.LogWarning($"[2x2 Cube] Conflicto: Nombre='{faceColor}' vs Rayos='{userRayColor}'. Usando Rayos.");
                             }
                             faceColor = userRayColor;
                             Debug.Log($"[2x2 Cube] Cara detectada por RAYOS DEL USUARIO: {faceColor}");
                         }
                    }
                    
                    // 3. Fallback to calculated normal
                    if (faceColor == "Unknown")
                    {
                        faceColor = GetTouchedFaceColor(currentPiece, hit.normal);
                    }
                    
                    Debug.Log($"[2x2 Cube] RESULTADO FINAL -> Pieza: {currentPiece.name} | Cara: {faceColor} (Objeto tocado: {hitObject.name})");
                }
                else
                {
                    currentPiece = null;
                    Debug.LogWarning($"[2x2 Cube] El objeto '{hitName}' (ni sus padres) es una pieza válida. Asegúrate de que las piezas tengan nombres de 3 letras (e.g. 'FUR') y colliders.");
                }
            }
            else
            {
                // If we reach here, the raycast truly missed everything in the scene
                currentPiece = null;
                Debug.Log("[2x2 Cube] El Raycast salio de la camara pero NO golpeó ningun collider en toda la escena.");
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

    bool IsValidPieceName(string name)
    {
        return name.Length == 3 &&
               (name.Contains("F") || name.Contains("B")) &&
               (name.Contains("U") || name.Contains("D")) &&
               (name.Contains("L") || name.Contains("R"));
    }

    /// <summary>
    /// Uses the transforms from ReadCube2x2 (tUp, tDown, etc.) to determine which face aligns with the hit normal.
    /// Mapped according to CubeMap2x2 logic:
    /// F -> Orange
    /// B -> Red
    /// U -> Yellow
    /// D -> White
    /// L -> Green
    /// R -> Blue
    /// </summary>
    string GetFaceFromUserRays(Vector3 hitNormal)
    {
        if (readCube == null) return "Unknown";

        // Map transforms to colors/names based on CubeMap2x2.cs
        var faceRays = new Dictionary<string, Transform>
        {
            { "Yellow", readCube.tUp },     // U -> Yellow
            { "White", readCube.tDown },    // D -> White
            { "Green", readCube.tLeft },    // L -> Green
            { "Blue", readCube.tRight },    // R -> Blue
            { "Orange", readCube.tFront },  // F -> Orange
            { "Red", readCube.tBack }       // B -> Red
        };

        string bestMatch = "Unknown";
        float maxDot = -2f; 

        foreach (var kvp in faceRays)
        {
            Transform t = kvp.Value;
            if (t == null) continue;
            
            float dot = Vector3.Dot(hitNormal, t.forward);
            
            if (dot > maxDot)
            {
                maxDot = dot;
                bestMatch = kvp.Key;
            }
        }
        
        if (maxDot > 0.5f) 
        {
            return bestMatch;
        }

        return "Unknown";
    }

    // ... CreateFacePivots removed replaced by InitializePivotsFromReadCube ...



    string GetColorFromObjectName(string name)
    {
        // Mapping based on CubeMap2x2.cs
        name = name.ToUpper();
        if (name.StartsWith("F") || name.Contains("FRONT")) return "Orange";
        if (name.StartsWith("B") || name.Contains("BACK")) return "Red";
        if (name.StartsWith("U") || name.Contains("UP")) return "Yellow";
        if (name.StartsWith("D") || name.Contains("DOWN")) return "White";
        if (name.StartsWith("L") || name.Contains("LEFT")) return "Green";
        if (name.StartsWith("R") || name.Contains("RIGHT")) return "Blue";
        return "Unknown";
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

        // Determine rotation axis and direction based on drag
        Vector3 rotationAxis;
        float dragSign = 1f;

        if (Mathf.Abs(dotUp) > Mathf.Abs(dotRight))
        {
            // Vertical drag (Up/Down) -> rotate around horizontal axis (Right)
            rotationAxis = pieceRight;
            dragSign = Mathf.Sign(dotUp);
        }
        else
        {
            // Horizontal drag (Left/Right) -> rotate around vertical axis (Up)
            rotationAxis = pieceUp;
            dragSign = Mathf.Sign(dotRight);
        }

        Debug.Log($"[2x2 Cube] Dirección de arrastre: dotUp={dotUp:F2}, dotRight={dotRight:F2} -> Sign: {dragSign}");
        Debug.Log($"[2x2 Cube] Eje de rotación (World): {rotationAxis}");

        // Find which of the 3 faces has an axis aligned with the rotation axis
        string faceToRotate = null;
        float maxDot = 0.5f; // Threshold for alignment

        foreach (string face in pieceFaces)
        {
            // IMPORTANT: faceAxes are LOCAL to the cube. Transform to WORLD to compare with rotationAxis (which is derived from piece transform in world)
            Vector3 worldFaceAxis = transform.TransformDirection(faceAxes[face]);
            
            float dot = Mathf.Abs(Vector3.Dot(worldFaceAxis, rotationAxis));
            
            Debug.Log($"[2x2 Cube] Cara {face}: eje WORLD {worldFaceAxis}, dot con rotationAxis = {dot:F2}");
            
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

        // Calculate Angle with Sign Correction
        // We compare the World Rotation Axis (from Piece) with the World Face Axis.
        // If they point in opposite directions, we flip the rotation sign.
        Vector3 targetWorldAxis = transform.TransformDirection(faceAxes[faceToRotate]);
        float alignmentSign = Mathf.Sign(Vector3.Dot(targetWorldAxis, rotationAxis));
        
        float finalAngle = 90f * dragSign * alignmentSign;

        // Rotate
        pivotRotation.Rotate(facePieces, faceAxes[faceToRotate], finalAngle);
    }

    List<GameObject> GetPiecesForFace(string faceLetter)
    {
        List<GameObject> pieces = new List<GameObject>();
        float tolerance = 0.1f; // Tolerance for floating point checks

        foreach (Transform child in transform)
        {
            // Skip non-pieces (pivots, or things without colliders/meshes that might be children)
            // We assume pieces are the ones that were originally named with 3 letters or have the piece tag/layer
            if (child.name.Contains("Pivot")) continue;
            if (child.name.Length != 3) continue; // STRICTER CHECK: Only standard pieces (FUR, FDL, etc.)
            
            // Optional: stricter check if needed, e.g. checking for specific component
            // if (child.GetComponent<Collider>() == null) continue;

            Vector3 localPos = child.localPosition;
            bool matches = false;

            switch (faceLetter.ToUpper())
            {
                case "R": matches = localPos.x > tolerance; break;
                case "L": matches = localPos.x < -tolerance; break;
                case "U": matches = localPos.y > tolerance; break;
                case "D": matches = localPos.y < -tolerance; break;
                case "F": matches = localPos.z > tolerance; break;
                case "B": matches = localPos.z < -tolerance; break;
            }

            if (matches)
            {
                pieces.Add(child.gameObject);
            }
        }
        
        return pieces;
    }

    /// <summary>
    /// Rotates a specific face by name. Used for programmatic rotations (e.g., shuffle).
    /// </summary>
    /// <param name="faceName">Face to rotate: U, D, L, R, F, or B</param>
    public void RotateFaceByName(string faceName, float angle = 90f)
    {
        faceName = faceName.ToUpper();
        
        if (!facePivots.ContainsKey(faceName))
        {
            Debug.LogError($"[2x2 Cube] Cara inválida: {faceName}");
            return;
        }

        // Check if any pivot is currently rotating
        PivotRotation2x2[] pivots = FindObjectsOfType<PivotRotation2x2>();
        foreach (var p in pivots)
        {
            if (p.IsRotating())
            {
                Debug.LogWarning("[2x2 Cube] No se puede rotar - otra cara está rotando");
                return;
            }
        }

        // Get all pieces for this face
        List<GameObject> facePieces = GetPiecesForFace(faceName);
        
        if (facePieces.Count != 4)
        {
            Debug.LogError($"[2x2 Cube] La cara {faceName} no tiene 4 piezas (tiene {facePieces.Count})");
            return;
        }

        // Get the pivot
        GameObject pivot = facePivots[faceName];
        PivotRotation2x2 pivotRotation = pivot.GetComponent<PivotRotation2x2>();

        if (pivotRotation == null)
        {
            Debug.LogError($"[2x2 Cube] El pivote {faceName} no tiene PivotRotation2x2");
            return;
        }

        Debug.Log($"[2x2 Cube] Rotando cara {faceName} programáticamente por {angle} grados");

        // Parent pieces to pivot
        foreach (GameObject p in facePieces)
        {
            p.transform.SetParent(pivot.transform, true);
        }

        // Rotate
        pivotRotation.Rotate(facePieces, faceAxes[faceName], angle);
    }

    /// <summary>
    /// Checks if any face is currently rotating
    /// </summary>
    public bool IsAnyFaceRotating()
    {
        PivotRotation2x2[] pivots = FindObjectsOfType<PivotRotation2x2>();
        foreach (var p in pivots)
        {
            if (p.IsRotating())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines the color of the touched face based on local normal and piece name.
    /// </summary>
    string GetTouchedFaceColor(GameObject piece, Vector3 worldNormal)
    {
        Vector3 localNormal = piece.transform.InverseTransformDirection(worldNormal);

        // Find dominant axis
        float x = localNormal.x;
        float y = localNormal.y;
        float z = localNormal.z;

        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        float absZ = Mathf.Abs(z);

        char faceChar = ' ';
        string color = "Unknown";

        if (absX > absY && absX > absZ)
        {
            if (x > 0) { faceChar = 'R'; color = "Blue"; }
            else { faceChar = 'L'; color = "Green"; }
        }
        else if (absY > absX && absY > absZ)
        {
            if (y > 0) { faceChar = 'U'; color = "Yellow"; }
            else { faceChar = 'D'; color = "White"; }
        }
        else
        {
            if (z > 0) { faceChar = 'F'; color = "Orange"; }
            else { faceChar = 'B'; color = "Red"; }
        }

        // Check if this face is valid for this piece (has a sticker)
        // Pieces are named like "FUR", "BDL", etc.
        if (piece.name.ToUpper().Contains(faceChar.ToString()))
        {
            return color;
        }
        else
        {
            return "Internal/Black";
        }
    }
}