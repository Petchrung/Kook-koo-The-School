
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float stamina = 5f;
    public float staminaDrain = 1f;
    public float staminaRegen = 0.7f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        stamina = maxStamina;
    }

    void Update()
    {
        // รับค่าปุ่มเดิน
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x +
                       transform.forward * z;

        // ป้องกันการเดินเร็วเกินไปในแนวทแยง
        move = Vector3.ClampMagnitude(move, 1f);

        bool isMoving = move.magnitude > 0.1f;

        // ตรวจสอบการวิ่ง
        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) &&
            isMoving &&
            stamina > 0f;

        // เลือกความเร็ว
        float currentSpeed = isRunning
            ? runSpeed
            : walkSpeed;

        // จัดการ Stamina
        if (isRunning)
        {
            stamina -= staminaDrain * Time.deltaTime;
        }
        else
        {
            stamina += staminaRegen * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0f, maxStamina);

        // เคลื่อนที่และหยุดทันทีเมื่อปล่อยปุ่ม
        controller.SimpleMove(move * currentSpeed);
    }

    void OnGUI()
    {
        float barWidth = 220f;
        float barHeight = 16f;

        float x = 25f;
        float y = Screen.height - 55f;

        float fill = maxStamina > 0f
            ? stamina / maxStamina
            : 0f;

        GUI.color = Color.white;
        GUI.Label(
            new Rect(x, y - 22f, 200f, 20f),
            "STAMINA"
        );

        GUI.color = Color.gray;
        GUI.Box(
            new Rect(x, y, barWidth, barHeight),
            ""
        );

        GUI.color = Color.Lerp(
            Color.red,
            Color.green,
            fill
        );

        GUI.Box(
            new Rect(x, y, barWidth * fill, barHeight),
            ""
        );

        GUI.color = Color.white;
    }
}