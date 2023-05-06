using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
  private PlayerInput playerInput;
  private InputAction touchPositionAction;
  private InputAction touchPressAction;
  private Camera mainCam;

  #region EVENTS
  public static event Action<Vector2> touchStarted;
  public static event Action<Vector2> touchEnded;
  #endregion

  private void Awake() {
    playerInput = GetComponent<PlayerInput>();
    touchPositionAction = playerInput.actions["TouchPosition"];
    touchPressAction = playerInput.actions.FindAction("TouchPress");
  }

  private void Start()
  {
    mainCam = Utils.Instance.MainCamera;
  }

  private void OnEnable() {
    touchPressAction.performed += TouchPressed;
  }

  private void OnDisable()
  {
    touchPressAction.performed -= TouchPressed;
  }

  private void Update()
  {
    if (touchPressAction.WasPerformedThisFrame())
    {
      Debug.Log(touchPositionAction.ReadValue<Vector2>());
    }

    Debug.Log(touchPositionAction.ReadValue<Vector2>());
  }

  void TouchPressed(InputAction.CallbackContext context)
  {
    Vector2 screenTouchPos = touchPositionAction.ReadValue<Vector2>();
    Vector3 worldTouchPos = mainCam.ScreenToWorldPoint(screenTouchPos);
    worldTouchPos.z = mainCam.nearClipPlane;
  }
}
