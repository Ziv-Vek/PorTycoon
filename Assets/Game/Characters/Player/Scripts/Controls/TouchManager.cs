using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class TouchManager : MonoBehaviour
{
  // stats config:
  [SerializeField] private float touchDragDeadZone = 5f;
  
  // variables:
  Vector2 startTouchPos = new Vector2();
  private bool isTouching = false;
  bool isFirstTouch = true;
  bool isMoving = false;
  
  // cached ref:
  [SerializeField] private FloatingJoystick floatingJoystick;
  //private PlayerMover playerMover;
  //private PlayerInputActions playerInputActions;
  private Coroutine inputDragCoroutine;
  private Camera mainCam;
  
  
  private PlayerInput playerInput;
  private InputAction touchPositionAction;
  private InputAction touchPressAction;
  

  #region EVENTS
  public static event Action<Vector2> touchStarted;
  public static event Action<Vector2> touchPerformed; 
  public static event Action touchEnded;
  #endregion

  private void Awake() 
  {
    playerInput = GetComponent<PlayerInput>();
    //controller = GetComponent<CharacterController>();
    touchPressAction = playerInput.actions.FindAction("TouchPress");
    touchPositionAction = playerInput.actions["TouchPosition"];
  }

  private void Start()
  {
    if (!Utils.Instance)
    {
      this.AddComponent<Utils>();
    }
    
    mainCam = Utils.Instance.MainCamera;
  }

  private void OnEnable() {
    touchPressAction.performed += TouchPressed;
    touchPressAction.canceled += TouchCanceled;
  }

  private void OnDisable()
  {
    touchPressAction.performed += TouchPressed;
    touchPressAction.canceled -= TouchCanceled;
  }

  void TouchPressed(InputAction.CallbackContext context)
  {
    isTouching = true;
    
    touchStarted?.Invoke(touchPositionAction.ReadValue<Vector2>());
    
    /*Vector2 screenTouchPos = touchPositionAction.ReadValue<Vector2>();
    Vector3 worldTouchPos = mainCam.ScreenToWorldPoint(screenTouchPos);*/
    //worldTouchPos.z = mainCam.nearClipPlane;

    StartCoroutine(TouchDrag());
  }

  void TouchCanceled(InputAction.CallbackContext context)
  {
    isTouching = false;
    
    touchEnded?.Invoke();
  }

  IEnumerator TouchDrag()
  {
    while (isTouching)
    {
      touchPerformed?.Invoke(touchPositionAction.ReadValue<Vector2>());
      yield return null;
    }
  }
}
