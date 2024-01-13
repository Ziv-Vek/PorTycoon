using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera PointerCoinCamera;

    public void setPointerCoinCamera(bool b)
    {
        PointerCoinCamera.enabled = b;
        GetComponent<Camera>().enabled = !b;
    }
    
}
