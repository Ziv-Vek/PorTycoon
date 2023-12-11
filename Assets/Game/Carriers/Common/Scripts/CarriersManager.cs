using UnityEngine;

public class CarriersManager : MonoBehaviour, ISaveable
{
    private static CarriersManager _instance;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }


    public object CaptureState()
    {
        return null;
    }

    public void RestoreState(object state)
    {
        /*carriers = (Dictionary<string, CarrierData>)state;
        Debug.Log("restoring carrierManager" + state);*/
    }
}