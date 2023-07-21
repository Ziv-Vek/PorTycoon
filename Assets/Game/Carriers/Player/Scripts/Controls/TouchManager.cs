using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class TouchManager : MonoBehaviour
{
  // stats config:
  [SerializeField] private float touchDragDeadZone = 5f;
  
  // variables:
  private bool isTouching = false;
  bool isFirstTouch = true;
  bool isMoving = false;
  
  // cached ref:
  private Coroutine inputDragCoroutine;
  private Camera mainCam;
  
  private PlayerInput playerInput;
  private InputAction touchPositionAction;
  private InputAction touchPressAction;
  
  #region EVENTS
  public static event Action<Vector2> onTouchStarted;
  public static event Action<Vector2> onTouchPerformed; 
  public static event Action onTouchEnded;
  #endregion

  private void Awake() 
  {
    playerInput = GetComponent<PlayerInput>();
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
    Vector2 startTouchPos = touchPositionAction.ReadValue<Vector2>();
      
    isTouching = true;
    
    onTouchStarted?.Invoke(startTouchPos);
    
    StartCoroutine(TouchDrag(startTouchPos));
  }

  void TouchCanceled(InputAction.CallbackContext context)
  {
    isTouching = false;
    onTouchEnded?.Invoke();
  }

  IEnumerator TouchDrag(Vector2 startTouchPos)
  {
    var currentTouchPos = new Vector2();
    
    while (isTouching)
    {
      currentTouchPos = touchPositionAction.ReadValue<Vector2>();
      if (Vector2.Distance(startTouchPos, currentTouchPos) > touchDragDeadZone)
        onTouchPerformed?.Invoke(currentTouchPos);
      yield return null;
    }
  }
}
