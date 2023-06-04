using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchBoard : MonoBehaviour
{
    public Camera mainCamera;


    void Update()
    {
        // Rotate the UI to face the camera
        var rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);
        
        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane+20));

    }
}