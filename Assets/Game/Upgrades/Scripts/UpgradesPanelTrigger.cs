using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesPanelTrigger : MonoBehaviour
{
    [SerializeField] GameObject UpgradesPanel;
    private void OnTriggerEnter(Collider other)
    {
        UpgradesPanel.SetActive(true);
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try { playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>(); } catch { }  
        playerMover.HideJoystick();
        playerMover.ToggleMovement(false);
    }
}