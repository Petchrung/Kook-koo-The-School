
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth = 3;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("GAME OVER");

        // หยุดระบบเดินและวิ่ง
        TestPlayerMovement movement =
            GetComponent<TestPlayerMovement>();

        if (movement != null)
            movement.enabled = false;

        // ปลดล็อกเมาส์
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.red;

        GUI.Label(
            new Rect(25, 25, 300, 50),
            "HP: " + currentHealth + " / " + maxHealth,
            style
        );

        if (isDead)
        {
            GUIStyle gameOverStyle = new GUIStyle();
            gameOverStyle.fontSize = 55;
            gameOverStyle.fontStyle = FontStyle.Bold;
            gameOverStyle.alignment = TextAnchor.MiddleCenter;
            gameOverStyle.normal.textColor = Color.red;

            GUI.Label(
                new Rect(
                    0,
                    Screen.height / 2 - 50,
                    Screen.width,
                    100
                ),
                "GAME OVER",
                gameOverStyle
            );
        }
    }
}