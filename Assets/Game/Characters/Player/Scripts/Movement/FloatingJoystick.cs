using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class FloatingJoystick : MonoBehaviour
{
    [SerializeField] Vector2 joystickSize = new Vector2(300, 300);
    [SerializeField] Vector2 joystickSizeMargin = new Vector2(5, 5);
    [SerializeField] float knobSpeed = 3f;
    public float JoystickSize { get { return joystickSize.x; } }

    [SerializeField] RectTransform outerJoystick;
    [SerializeField] RectTransform knob;

    private void OnEnable()
    {
        TouchManager.touchStarted += ActivateJoystick;
        TouchManager.touchEnded += DeactivateJoystick;
    }

    private void OnDisable()
    {
        TouchManager.touchStarted -= ActivateJoystick;
        TouchManager.touchEnded -= DeactivateJoystick;
    }

    private void ActivateJoystick(Vector2 screenTouchPos)
    {
        outerJoystick.gameObject.SetActive(true);
        outerJoystick.sizeDelta = joystickSize;
        outerJoystick.anchoredPosition = ClampJoystickPos(screenTouchPos);
        knob.anchoredPosition = Vector2.zero;
    }
    
    private void MoveKnob(Vector2 screenTouchPos)
    {
        float maxJoystickMovement = joystickSize.x / 2;

        if (Vector2.Distance(screenTouchPos, outerJoystick.anchoredPosition) > maxJoystickMovement)
        {
            knob.anchoredPosition = (screenTouchPos - outerJoystick.anchoredPosition).normalized * maxJoystickMovement;
        }
        else
        {
            knob.anchoredPosition = screenTouchPos - outerJoystick.anchoredPosition;
        }
    }

    public void DeactivateJoystick()
    {
        knob.anchoredPosition = Vector2.zero;
        outerJoystick.gameObject.SetActive(false);
    }

    private Vector2 ClampJoystickPos(Vector2 screenTouchPos)
    {
        if (screenTouchPos.x < (joystickSize.x / 2 + joystickSizeMargin.x))
        {
            screenTouchPos.x = (joystickSize.x / 2 + joystickSizeMargin.x);
        }
        else if (screenTouchPos.x > (Screen.width - (joystickSize.x / 2 + joystickSizeMargin.x)))
        {
            screenTouchPos.x = Screen.width - (joystickSize.x / 2 + joystickSizeMargin.x);
        }

        if (screenTouchPos.y < (joystickSize.y / 2 + joystickSizeMargin.y))
        {
            screenTouchPos.y = (joystickSize.y / 2 + joystickSizeMargin.y);
        }
        else if (screenTouchPos.y > (Screen.height - (joystickSize.y / 2 + joystickSizeMargin.y)))
        {
            screenTouchPos.y = Screen.height - (joystickSize.y / 2 + joystickSizeMargin.y);
        }

        return screenTouchPos;
    }
    
}
