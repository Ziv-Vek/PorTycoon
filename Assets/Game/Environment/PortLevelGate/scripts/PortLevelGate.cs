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
            GameManager.Instance.SetCurrentLevel(transform.parent.GetComponent<PortLoader>().PortLevel);
            GetComponent<BoxCollider>().enabled = false;
            UserDataManager.Instance.SaveUserData();
        }
    }
}
