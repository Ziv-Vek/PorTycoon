using System;
using UnityEngine;

[ExecuteAlways]
public partial class Utils: MonoBehaviour
{
    #region StaticInstanceGetter
    private static Utils _instance;
    public static Utils Instance
    {
        get { return _instance; }
    }
    #endregion
    
    private Camera mainCamera;
    public Camera MainCamera
    {
        get { return mainCamera; }
    }

    private void Awake()
    {
        _instance = this;
        
        try
        {
            mainCamera = Camera.main;
            if (mainCamera == null) throw new Exception("No main camera tag was found in the scene.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string GetUniqueID()
    {
        return Guid.NewGuid().ToString();
    }
}
