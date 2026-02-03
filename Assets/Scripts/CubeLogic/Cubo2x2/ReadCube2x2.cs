using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCube2x2 : MonoBehaviour

{
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    private List<GameObject> frontRays = new();
    private List<GameObject> backRays = new();
    private List<GameObject> upRays = new();
    private List<GameObject> downRays = new();
    private List<GameObject> leftRays = new();
    private List<GameObject> rightRays = new();

    private int layerMask = 1 << 8;

    CubeState2x2 cubeState;
    CubeMap2x2 cubeMap;

    public GameObject emptyGO;

    void Start()
    {
        cubeState = FindObjectOfType<CubeState2x2>();
        cubeMap = FindObjectOfType<CubeMap2x2>();

        SetRayTransforms();

        ReadState();

        CubeState2x2.started = true;
    }

    // =============================
    // LECTURA DEL ESTADO
    // =============================
    public void ReadState()
    {
        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);

        cubeMap.Set();
    }

    // =============================
    // CREAR RAYOS 2x2
    // =============================
    void SetRayTransforms()
    {
        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
        backRays = BuildRays(tBack, new Vector3(0, 270, 0));
    }

    List<GameObject> BuildRays(Transform rayParent, Vector3 direction)
    {
        List<GameObject> rays = new();

        float spacing = 0.6f;

        for (int y = 1; y >= 0; y--)
        {
            for (int x = 0; x < 2; x++)
            {
                GameObject rayStart = Instantiate(emptyGO, rayParent);

                rayStart.transform.localPosition =
                    new Vector3((x - 0.5f) * spacing, (y - 0.5f) * spacing, 0);

                rayStart.name = rays.Count.ToString();

                rays.Add(rayStart);
            }
        }

        rayParent.localRotation = Quaternion.Euler(direction);

        return rays;
    }

    // =============================
    // RAYCAST SEGURO
    // =============================
    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new();

        foreach (GameObject rayStart in rayStarts)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayStart.transform.position, rayTransform.forward, out hit, 10f, layerMask))
            {
                Debug.DrawRay(rayStart.transform.position, rayTransform.forward * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
            }
            else
            {
                Debug.DrawRay(rayStart.transform.position, rayTransform.forward * 10f, Color.green);
                facesHit.Add(null);
            }
        }

        return facesHit;
    }
}
