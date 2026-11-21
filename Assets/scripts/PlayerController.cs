using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchSmooth = 8f;
    public LayerMask obstructionMask = ~0;
    [Header("Camera")]
    public Transform cameraHolder;
    public float standingCameraHeight = 1.65f;
    public float crouchingCameraHeight = 0.75f;
    private CharacterController controller;
    private Vector3 velocity;
    private float targetHeight;
    private float targetCameraHeight;
    private bool wantsToCrouch;
    public bool IsRunning { get; private set; }
    public bool IsCrouching { get; private set; }
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
        controller.center = Vector3.up * standingHeight / 2f;
        targetHeight = standingHeight;
        targetCameraHeight = standingCameraHeight;
    }
    void Update()
    {
        HandleCrouch();
        HandleMovement();
        UpdateCrouchHeight();
    }
    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction =
            transform.right * x +
            transform.forward * z;
        direction = Vector3.ClampMagnitude(direction, 1f);
        bool moving = direction.sqrMagnitude > 0.01f;
        IsRunning =
            Input.GetKey(KeyCode.LeftShift) &&
            moving &&
            !IsCrouching;
        float speed = walkSpeed;
        if (IsCrouching)
            speed = crouchSpeed;
        else if (IsRunning)
            speed = runSpeed;
        controller.Move(direction * speed * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        if (Input.GetKeyDown(KeyCode.Space) &&
            controller.isGrounded &&
            !IsCrouching)
        {
            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void HandleCrouch()
    {
        wantsToCrouch = Input.GetKey(KeyCode.LeftControl);
        if (wantsToCrouch)
        {
            IsCrouching = true;
        }
        else if (IsCrouching && CanStand())
        {
            IsCrouching = false;
        }
        targetHeight =
            IsCrouching ? crouchingHeight : standingHeight;
        targetCameraHeight =
            IsCrouching ? crouchingCameraHeight : standingCameraHeight;
    }
    bool CanStand()
    {
        float radius = controller.radius * 0.95f;
        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = transform.position +
            Vector3.up * (standingHeight - radius);
        return !Physics.CheckCapsule(
            bottom, top, radius,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );
    }
    void UpdateCrouchHeight()
    {
        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchSmooth * Time.deltaTime
        );
        controller.center =
            Vector3.up * controller.height / 2f;
        if (cameraHolder != null)
        {
            Vector3 pos = cameraHolder.localPosition;
            pos.y = Mathf.Lerp(
                pos.y,
                targetCameraHeight,
                crouchSmooth * Time.deltaTime
            );
            cameraHolder.localPosition = pos;
        }
    }
}
