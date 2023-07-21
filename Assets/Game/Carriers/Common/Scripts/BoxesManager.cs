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

    //[SerializeField] private GameObject simpleBox;

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

    public GameObject GiveCargo()
    {
        return Instantiate(boxesPrefabs[0].box, Vector3.zero, Quaternion.identity);
    }

    public List<GameObject> GetBoxesByName(List<string> boxesNames)
    {
        if (boxesNames == null || boxesNames.Count == 0)
        {
            throw new Exception("Did not receive boxesNames");
        }
        
        List<GameObject> boxes = new List<GameObject>();

        for (int i = 0; i < boxesNames.Count; i++)
        {
            if (boxesNames[i] == boxesPrefabs[0].boxName)
            {
                Debug.LogWarning("boxName received is of default box name");
                boxes.Add(InstantiateBox(0));
                continue;
            }
            
            for (int j = 0; j < boxesPrefabs.Length; j++)
            {
                if (boxesPrefabs[j].boxName == boxesNames[i])
                {
                    boxes.Add(InstantiateBox(j));
                    break;
                }

                if (j == boxesPrefabs.Length - 1)
                {
                    Debug.LogWarning("No identical boxName found. Instantiating default box prefab");
                    boxes.Add(InstantiateBox(0));
                }
            }
        }

        return boxes;
    }

    public struct BoxesStruct
    {
        public List<GameObject> boxes;
        public List<string> boxesNames;
    }
    
    /*public BoxesStruct GetBoxesByQuantity(int numBoxesToInstantiate)
    {
        List<GameObject> boxes = new List<GameObject>();
        List<string> boxesNames = new List<string>();
        int randNum;

        for (int i = 0; i < numBoxesToInstantiate; i++)
        {
            randNum = Random.Range(1, boxesPrefabs.Length);
            boxes.Add(InstantiateBox(randNum));
            boxesNames.Add(boxesPrefabs[randNum].boxName);
        }

        BoxesStruct newBoxesData = new BoxesStruct();
        newBoxesData.boxes = boxes;
        newBoxesData.boxesNames = boxesNames;
        
        return newBoxesData;
    }*/
    
    /*public GameObject[] GetBoxesByQuantity(int numBoxesToInstantiate, out List<string> boxesNames)
    {
        GameObject[] boxes = new GameObject[numBoxesToInstantiate];
        List<string> names = new List<string>();
        int randNum;

        for (int i = 0; i < numBoxesToInstantiate; i++)
        {
            randNum = Random.Range(1, boxesPrefabs.Length);
            boxes.Add(InstantiateBox(randNum));
            names.Add(boxesPrefabs[randNum].boxName);
        }

        boxesNames = names;

        return boxes;
    }*/
    
    public GameObject[] GetBoxesByQuantity(int numBoxesToInstantiate)
    {
        GameObject[] boxes = new GameObject[numBoxesToInstantiate];
        int randNum;

        for (int i = 0; i < numBoxesToInstantiate; i++)
        {
            randNum = Random.Range(1, boxesPrefabs.Length);
            boxes[i] = InstantiateBox(randNum);
        }

        return boxes;
    }
    
    public void ReceiveCargo(GameObject cargo)
    {
        throw new System.NotImplementedException();
    }

    public bool CanReceiverAcceptCargo { get; }

    private GameObject InstantiateBox(int targetBoxName)
    {
        GameObject newBox = Instantiate(boxesPrefabs[targetBoxName].box, Vector3.zero, Quaternion.identity);
        return newBox;
    }
}

[System.Serializable]
class BoxPrefab
{
    public string boxName;
    public GameObject box;
}

