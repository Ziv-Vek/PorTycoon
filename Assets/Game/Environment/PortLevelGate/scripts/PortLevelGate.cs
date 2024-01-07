using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortLevelGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player entered");
            GameManager.Instance.level = transform.parent.GetComponent<PortLoader>().PortLevel;
            UserDataManager.Instance.SaveUserDataAsync();
            other.GetComponent<PlayerCarrier>().RemoveAllBoxes();
        }
    }
}
