using System.Collections.Generic;
using UnityEngine;

public class FindShader : MonoBehaviour
{
    public Shader targetShader;

    void Start()
    {
        // Find objects with the specified shader
        List<GameObject> objectsWithShader = FindObjectsWithShader(targetShader);

        // Do something with the objects found
        foreach (GameObject obj in objectsWithShader)
        {
            Debug.Log("Object with shader found: " + obj.name);
        }
    }

    List<GameObject> FindObjectsWithShader(Shader shader)
    {
        // Find all objects with a renderer component
        Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>();

        // Filter objects that use the specified shader
        List<GameObject> objectsWithShader = new List<GameObject>();
        foreach (Renderer renderer in allRenderers)
        {
            if (renderer.material.shader == shader)
            {
                objectsWithShader.Add(renderer.gameObject);
                Debug.Log(renderer.gameObject.name);
            }
        }

        return objectsWithShader;
    }
}