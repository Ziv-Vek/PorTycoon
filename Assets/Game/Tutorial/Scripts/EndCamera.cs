using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCamera : MonoBehaviour
{
    public void EndAnimation()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        FindAnyObjectByType<TutorialM>().DestroyItSelf();
    }
}
