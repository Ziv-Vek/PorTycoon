using UnityEngine;

public interface IGiveCargo
{
    public GameObject GiveCargo();
    public int CurrentNumOfCargoHolding { get; }

}

