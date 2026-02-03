using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    Vector2 firstPressPosition;
    Vector2 secondPressPosition;
    Vector2 currentSwipe;
    Vector3 previousMousePosition;
    Vector3 mouseDelta;

    public GameObject target;
    float speed = 180f; // grados por segundo para rotación suave
    Quaternion targetRotation;

    void Start()
    {
        targetRotation = target.transform.rotation;
        previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        Swipe();
        Drag();
    }

    // --------------------------------------------------------------
    // ROTACIÓN POR MOUSE (TU CÓDIGO ORIGINAL)
    // --------------------------------------------------------------
    void Drag()
    {
        if (Input.GetMouseButton(1))
        {
            mouseDelta = Input.mousePosition - previousMousePosition;
            mouseDelta *= 0.1f;

            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
        }
        else
        {
            if (transform.rotation != targetRotation)
            {
                float step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            }
        }

        previousMousePosition = Input.mousePosition;
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(1))
        {
            firstPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(1))
        {
            secondPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector2(
                secondPressPosition.x - firstPressPosition.x,
                secondPressPosition.y - firstPressPosition.y
            );

            currentSwipe.Normalize();

            if (leftSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(90f, Vector3.up) * targetRotation;
            }
            else if (rightSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(-90f, Vector3.up) * targetRotation;
            }
            else if (upRightSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(90f, Vector3.right) * targetRotation;
            }
            else if (upLeftSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(90f, Vector3.forward) * targetRotation;
            }
            else if (downRightSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * targetRotation;
            }
            else if (downLeftSwipe(currentSwipe))
            {
                targetRotation = Quaternion.AngleAxis(-90f, Vector3.right) * targetRotation;
            }
        }
    }

    bool leftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool rightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool upRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > 0;
    }

    bool upLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x < 0;
    }

    bool downRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > 0;
    }

    bool downLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x < 0;
    }
}
