using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class FloatingJoystick : MonoBehaviour
{
    #region StaticInstanceGetter

    private static FloatingJoystick _instance;

    public static FloatingJoystick Instance
    {
        get { return _instance; }
    }

    #endregion

    [SerializeField] Vector2 joystickSize = new(300, 300);
    [SerializeField] Vector2 joystickSizeMargin = new(5, 5);
    [SerializeField] float knobSpeed = 3f;
    private float maxJoystickMovement;

    public float JoystickSize => joystickSize.x;

    [SerializeField] RectTransform outerJoystick;

    [SerializeField] RectTransform knob;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        outerJoystick.sizeDelta = joystickSize;
        maxJoystickMovement = joystickSize.x / 2;
    }

    private void OnEnable()
    {
        TouchManager.onTouchStarted += ActivateJoystick;
        TouchManager.onTouchEnded += DeactivateJoystick;
        TouchManager.onTouchPerformed += MoveKnob;
    }

    private void OnDisable()
    {
        TouchManager.onTouchStarted -= ActivateJoystick;
        TouchManager.onTouchEnded -= DeactivateJoystick;
        TouchManager.onTouchPerformed -= MoveKnob;
    }

    private void ActivateJoystick(Vector2 screenTouchPos)
    {
        outerJoystick.gameObject.SetActive(true);
        outerJoystick.anchoredPosition = ClampJoystickPos(screenTouchPos);
        knob.anchoredPosition = Vector2.zero;
    }

    private void MoveKnob(Vector2 screenTouchPos)
    {
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