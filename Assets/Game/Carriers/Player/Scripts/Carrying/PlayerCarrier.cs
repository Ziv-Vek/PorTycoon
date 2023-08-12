using UnityEngine;
public class PlayerCarrier : Carrier, IBoxOpener
{
    public ScratchBoard scratchBoard; // Drag the Canvas GameObject with the script attached here in the inspector

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
        place = new Vector3(place.x, place.y + 3, place.z);
        GameObject newPlace = Instantiate(gameObject.transform.GetChild(1).gameObject, place, gameObject.transform.GetChild(1).rotation);
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
}