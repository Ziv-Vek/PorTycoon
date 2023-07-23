using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxTable : MonoBehaviour
{
    // Boxes on the table queue
    //TODO: when box implemented change to Box
    private Queue<GameObject> Boxes { get; set; }

    // num of assistants
    public int numOfAssistants = 0;

    //max boxes on the table
    public int maxBoxesOnTable = 3;

    public BoxTable(Queue<GameObject> boxes)
    {
        Boxes = boxes;
    }
    // 

    // get next box
    public GameObject GetNextBox()
    {
        return Boxes.Peek();
    }

    //remove box from queue
    public void RemoveBox()
    {
        Boxes.Dequeue();
    }

    // add box to queue
    public void AddBox(GameObject box)
    {
        Boxes.Enqueue(box);
    }

    // check if there is a box on the table
    public bool HasBox()
    {
        return Boxes.Count > 0;
    }

    // has assistant
    public bool HasAssistant()
    {
        return numOfAssistants > 0;
    }
    
}