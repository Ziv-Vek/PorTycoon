using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    // Stats Config:
    [SerializeField] public float maxMovementSpeed = 2.0f;
    [SerializeField] float turnSmoothTime = 0.1f;

    // Variables:
    float turnSmoothVelocity;
    Vector3 playerVelocity;
    Vector2 startTouchPos = new Vector2();
    bool isMovementAllowed = true;

    // Cached ref:
    [SerializeField] Animator myAnimator;
    FloatingJoystick joystick = null;
    CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        TouchManager.onTouchStarted += StartMove;
        TouchManager.onTouchEnded += CancelMovement;
        TouchManager.onTouchPerformed += Move;
    }

    private void OnDisable()
    {
        TouchManager.onTouchStarted -= StartMove;
        TouchManager.onTouchEnded -= CancelMovement;
        TouchManager.onTouchPerformed -= Move;
    }

    private void Start()
    {
        joystick = FloatingJoystick.Instance;
        if (!joystick) throw new Exception("FloatingJoystick is missing.");
        
        playerVelocity.y = 0f;
    }

    public void StartMove(Vector2 screenTouchPos)
    {
        startTouchPos = screenTouchPos;
    }

    public void Move(Vector2 screenTouchPos)
    {
        if (!isMovementAllowed) { return; }
        
        float movementSpeed = GetMovementSpeed(screenTouchPos);

        Vector2 screenMovementVector = screenTouchPos - startTouchPos;
        Vector3 direction = new Vector3(screenMovementVector.x, 0f, screenMovementVector.y).normalized;
        controller.Move(direction * (movementSpeed * Time.deltaTime));
        float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        UpdateAnimator(direction * movementSpeed);
    }

    public void CancelMovement()
    {
        UpdateAnimator(Vector3.zero);
    }

    private float GetMovementSpeed(Vector2 screenTouchPos)
    {
        float touchesDistance = Vector2.Distance(screenTouchPos, startTouchPos);
        float joystickSize = joystick.JoystickSize;

        if (touchesDistance > joystickSize / 2)
        {
            return maxMovementSpeed;
        }
        
        return ((touchesDistance / (joystickSize / 2)) * maxMovementSpeed);
    }

    void UpdateAnimator(Vector3 velocity)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        myAnimator.SetFloat("forwardSpeed", speed);
    }

    public void ToggleMovement(bool enableMovement)
    {
        isMovementAllowed = enableMovement;
    }
    
    public void HideJoystick()
    {
        joystick.gameObject.SetActive(false);
    }
    
    public void ShowJoystick()
    {
        joystick.gameObject.SetActive(true);
    }

    private void Update()
    {
        // Move player with wasd keys
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        controller.Move(direction * (maxMovementSpeed * Time.deltaTime));
        float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
        
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        UpdateAnimator(direction * maxMovementSpeed);
    }
}
