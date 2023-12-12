using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortLevelGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        GameManager.Instance.currentLevel = transform.parent.GetComponent<PortLoader>().PortLevel;
    }
}
