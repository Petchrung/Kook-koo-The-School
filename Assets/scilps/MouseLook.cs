using UnityEngine;
public class MouseLook : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerBody;
    public float mouseSensitivity = 150f;
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;
    private float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            return;
        }
        float mouseX = Input.GetAxis("Mouse X")
            * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y")
            * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(
            xRotation,
            minLookAngle,
            maxLookAngle
        );
        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}