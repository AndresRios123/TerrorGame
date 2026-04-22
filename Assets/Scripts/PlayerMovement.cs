using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;

    [Header("Mouse")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;

    [Header("Gravedad")]
    public float gravity = -9.81f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float cameraStandingY = 0.8f;
    public float cameraCrouchY = 0.4f;

    [Header("Sonidos")]
    public AudioSource footstepAudio;
    public float walkStepDelay = 0.5f;
    public float runStepDelay = 0.3f;
    public float crouchStepDelay = 0.8f;

    [HideInInspector] public bool animacionActiva = false;

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isCrouching = false;
    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!animacionActiva)
            Look();
        Move();
        HandleCrouch();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed;

        if (isCrouching)
            currentSpeed = crouchSpeed;
        else
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleFootsteps(move, currentSpeed);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;

            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = cameraCrouchY;
            cameraTransform.localPosition = camPos;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            controller.height = standingHeight;

            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = cameraStandingY;
            cameraTransform.localPosition = camPos;
        }
    }

    void HandleFootsteps(Vector3 move, float speed)
    {
        if (!controller.isGrounded) return;

        if (move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            float delay;

            if (isCrouching)
                delay = crouchStepDelay;
            else if (speed == runSpeed)
                delay = runStepDelay;
            else
                delay = walkStepDelay;

            if (stepTimer <= 0f)
            {
                footstepAudio.Play();
                stepTimer = delay;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            if (hit.moveDirection.y < -0.3f)
                return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            float pushForce = 3f;
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
    }
}