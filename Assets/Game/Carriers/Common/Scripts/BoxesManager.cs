using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class BoxesManager : MonoBehaviour
{
    public static BoxesManager Instance;
    [SerializeField] private BoxPrefab[] boxesPrefabs = null;
    private int shipSize = 4;


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
    }


    public PortBox[] GetBoxesByQuantity(int numBoxesToInstantiate)
    {
        PortBox[] boxes = new PortBox[numBoxesToInstantiate];
        int randNum;

        for (int i = 0; i < numBoxesToInstantiate; i++)
        {
            randNum = Random.Range(1, boxesPrefabs.Length);
            boxes[i] = InstantiateBox(randNum);
        }

        return boxes;
    }


    private PortBox InstantiateBox(int targetBoxName)
    {
        PortBox newBox = Instantiate(boxesPrefabs[targetBoxName].box, Vector3.zero, Quaternion.identity)
            .GetComponent<PortBox>();
        return newBox;
    }
}

[System.Serializable]
class BoxPrefab
{
    public string boxName;
    public GameObject box;
}