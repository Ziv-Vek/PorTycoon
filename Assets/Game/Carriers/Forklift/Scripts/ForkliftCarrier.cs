using UnityEngine;

public class ForkliftCarrier : Carrier
{
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
        GameObject newPlace = Instantiate(CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).gameObject, CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).position + new Vector3(0,5,0), CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).rotation);
        newPlace.transform.parent = CargoPlacesHolder.transform;

        newPlace.name = "CargoPlace (" + (CargoPlacesHolder.childCount - 1) + ")";
        newPlace.transform.localScale = new Vector3(1, 1, 1f);
        ArrayPlaces[ArrayPlaces.Length - 1] = newPlace.transform;
        try { Destroy(newPlace.transform.GetChild(0).gameObject); }
        catch { }
        boxesPlaces = ArrayPlaces;
        maxBoxesCapacity++;
        AddBox();
        //gameObject.GetComponent<ForkliftMover>().SetCarryingTask();
    }
}
