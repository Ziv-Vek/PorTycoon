using System.Collections.Generic;
using UnityEngine;

public class BoxesManager : MonoBehaviour
{
    public static BoxesManager Instance;
    
    public List<Box> boxes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        //boxes = GameManager.boxes;
    }
    
    public List<BoxSO> boxesData = new List<BoxSO>();
    
    
    
}