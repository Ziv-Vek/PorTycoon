using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Stats Config:
        [SerializeField] float maxMovementSpeed = 2.0f;
        [SerializeField] float turnSmoothTime = 0.1f;

        // Variables:
        float turnSmoothVelocity;
        private Vector3 playerVelocity;
        Vector2 startTouchPos = new Vector2();
        bool isMovementAllowed = true;

        // Cached ref:
        [SerializeField] FloatingJoystick joystick;
        [SerializeField] TimeScaleManager timeScaleManager;
        
        CharacterController controller;
        [SerializeField] Animator myAnimator;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            GetComponent<ActionsScheduler>().OnMovementToggle += ToggleMovement;
        }

        private void OnDisable()
        {
            GetComponent<ActionsScheduler>().OnMovementToggle -= ToggleMovement;
        }

        private void Start()
        {
            JoinToDieAction();
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

            timeScaleManager.UnfreezeTime(movementSpeed / maxMovementSpeed);

            Vector2 screenMovementVector = screenTouchPos - startTouchPos;
            Vector3 direction = new Vector3(screenMovementVector.x, 0f, screenMovementVector.y).normalized;
            controller.Move(direction * movementSpeed * Time.deltaTime);
            float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            UpdateAnimator(direction * movementSpeed);
        }

        public void CancelMovement()
        {
            UpdateAnimator(Vector3.zero);

            timeScaleManager.FreezeTime();
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

        void ToggleMovement(bool enableMovement)
        {
            isMovementAllowed = enableMovement;
        }

        public void JoinToDieAction()
        {
            GetComponent<DeathAction>().JoinToDieAction(this);
        }

        public void Die()
        {
            ToggleMovement(false);
        }
}
