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
    Vector2 startTouchPos;
    bool isMovementAllowed = true;

    // Cached ref:
    [SerializeField] Animator myAnimator;
    public FloatingJoystick joystick = null;
    CharacterController controller;
    [SerializeField] ParticleSystem DirtPartical;
    private static readonly int IsHoldingBox = Animator.StringToHash("isHoldingBox");
    private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

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
    private void Update()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0.0f; 
        transform.position = currentPosition;
    }
    public void StartMove(Vector2 screenTouchPos)
    {
        startTouchPos = screenTouchPos;
    }

    public void Move(Vector2 screenTouchPos)
    {
        if (!isMovementAllowed)
        {
            return;
        }

        float movementSpeed = GetMovementSpeed(screenTouchPos);

        Vector2 screenMovementVector = screenTouchPos - startTouchPos;
        Vector3 direction = new Vector3(screenMovementVector.x, 0, screenMovementVector.y).normalized;
        controller.Move(direction * (movementSpeed * Time.deltaTime));
        float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle =
            Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        UpdateAnimator(direction * movementSpeed);
    }

    public void CancelMovement()
    {
        UpdateAnimator(Vector3.zero);
        DirtPartical.Stop();
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
        myAnimator.SetFloat(ForwardSpeed, speed);
        if (speed > 0 && !DirtPartical.isPlaying)
            DirtPartical.Play();
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

    public void ToggleAnimatorHoldingBox(bool isHoldingBox)
    {
        if (myAnimator.GetBool(IsHoldingBox) != isHoldingBox)
            myAnimator.SetBool(IsHoldingBox, isHoldingBox);
    }
    
    public void SpawnPlayer(int level)
    {
        Debug.Log("level: " + level + "Port");
        transform.SetPositionAndRotation(GameObject.Find(level + "Port").GetComponent<PortLoader>().PlayerSpawnPoint.position, Quaternion.identity);
        //transform.position = GameObject.Find(level + "Port").GetComponent<PortLoader>().PlayerSpawnPoint.position;
    }
}