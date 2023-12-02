using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateItemScreen : MonoBehaviour
{
    public GameObject item;
    Vector2 PressMouse;
    Vector2 LastPos;
    public GameObject RotateOnY;
    private void OnEnable()
    {
        item = transform.parent.Find("ItemPlace").gameObject;
        LastPos = item.transform.position;
    }
    private Vector2 GetPointerPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                 //Vector2 DragPosition = GetPointerPosition();
                 //item.transform.Rotate(CalculateDistance(PressMouse.y, DragPosition.y), -CalculateDistance(PressMouse.x, DragPosition.x), 0);
            }
        }
    }

    private void OnMouseDown()
    {
        PressMouse = GetPointerPosition();
    }
    private void OnMouseDrag()
    {
        Vector2 DragPosition = GetPointerPosition();
        item.transform.SetParent(RotateOnY.transform);
        if (DragPosition != LastPos)
        {
            item.transform.Rotate(0, CalculateDistance(PressMouse.x, DragPosition.x), 0);
            RotateOnY.transform.Rotate(-CalculateDistance(PressMouse.y, DragPosition.y), 0, 0);
        }
        LastPos = DragPosition;
    }
    private void OnMouseUp()
    {
        item.transform.SetParent(transform.parent);
    }
    static float CalculateDistance(float x1, float x2)
    {
        // Absolute value is used to ensure the distance is positive
        return (x1 - x2);
    }
}
