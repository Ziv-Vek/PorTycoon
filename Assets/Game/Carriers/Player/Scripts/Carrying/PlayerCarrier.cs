using System;
using UnityEngine;
using System.Linq;
using CandyCoded;
using CandyCoded.HapticFeedback;

public class PlayerCarrier : Carrier, IBoxOpener
{
    public ScratchBoard scratchBoard; // Drag the Canvas GameObject with the script attached here in the inspector

    private PlayerMover playerMover;

    [SerializeField] AudioClip ReceiveBoxSound;
    [SerializeField] AudioClip GivingBoxSound;

    public override void Awake()
    {
        playerMover = GetComponent<PlayerMover>();
        boxes = new PortBox[maxBoxesCapacity];
    }

    public bool OpenBox(PortBox box)
    {
        if (scratchBoard.gameObject.activeSelf) return false;

        scratchBoard.Open(box);
        return true;
    }
    
    public void addBoxPlace()
    {
        Transform[] ArrayPlaces = new Transform[boxesPlaces.Length + 1];
        for (int i = 0; i < boxesPlaces.Length; i++)
        {
            ArrayPlaces[i] = boxesPlaces[i];
        }
        Vector3 place;
        place = transform.GetChild(transform.childCount - 1).transform.position;
        place = new Vector3(place.x, place.y + 5, place.z);
        GameObject newPlace = Instantiate(gameObject.transform.GetChild(1).gameObject, place, Quaternion.Euler(0, gameObject.transform.rotation.y, 0));
        newPlace.transform.parent = gameObject.transform;

        newPlace.name = "CargoPlace (" + (transform.childCount - 2) + ")";
        newPlace.transform.localScale = new Vector3(1, 1, 1f);
        ArrayPlaces[ArrayPlaces.Length - 1] = newPlace.transform;
        try { Destroy(newPlace.transform.GetChild(0).gameObject); }
        catch { }
        boxesPlaces = ArrayPlaces;
        maxBoxesCapacity++;
        AddBox();
    }
    
    public override PortBox GiveBox()
    {
        int index = Array.FindLastIndex(boxes, box => box != null);
        PortBox box = boxes[index];
        boxes[index] = null;
        VibrationManager.Instance.HeavyVibrate();
        if (boxes.Any()) playerMover.ToggleAnimatorHoldingBox(false);

        GetComponent<AudioSource>().clip = GivingBoxSound;
        GetComponent<AudioSource>().Play();

        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().SetBoxTable_Target();

        return box;
    }
    
    public override void ReceiveBox(PortBox box)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = box;
        box.transform.SetParent(boxesPlaces[index]);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = gameObject.transform.rotation;
        VibrationManager.Instance.MediumeVivrate();
        playerMover.ToggleAnimatorHoldingBox(true);
        GetComponent<AudioSource>().clip = ReceiveBoxSound;
        GetComponent<AudioSource>().Play();
        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().SetConveyor_Target();
    }

}