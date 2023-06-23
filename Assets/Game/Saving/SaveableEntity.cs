using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string uniqueID = "";

    public string GetUID()
    {
        return uniqueID;
    }
    
    #if UNITY_EDITOR

    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;
        
        
    }

    #endif

}
