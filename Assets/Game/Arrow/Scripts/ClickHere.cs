using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHere : MonoBehaviour
{
    public Transform Target;
    public GameObject NextClickHere;
    private void Start()
    {
        transform.position = Target.position;
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
    }
    private void OnMouseDown()
    {
        Destroy(gameObject);
        if(NextClickHere != null)
        {
            NextClickHere.SetActive(true);
        }
    }
}
