
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float holdDuration = 1.5f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Progress Bar")]
    public float barWidth = 250f;
    public float barHeight = 20f;

    private bool playerNearby = false;
    private float holdTimer = 0f;

    void Update()
    {
        if (!playerNearby)
        {
            holdTimer = 0f;
            return;
        }

        if (Input.GetKey(pickupKey))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                PickUpItem();
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            holdTimer = 0f;
        }
    }

    void PickUpItem()
    {
        if (ItemPickupUI.Instance != null)
        {
            ItemPickupUI.Instance.ShowMessage("ได้รับไอเทมแล้ว!");
        }

        Destroy(gameObject);
    }

    void OnGUI()
    {
        if (!playerNearby)
            return;

        GUIStyle textStyle = new GUIStyle();

        textStyle.fontSize = 22;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(
                Screen.width / 2 - 200,
                Screen.height - 180,
                400,
                40
            ),
            "กด E ค้าง เพื่อเก็บไอเทม",
            textStyle
        );

        float progress = Mathf.Clamp01(
            holdTimer / holdDuration
        );

        float x = Screen.width / 2 - barWidth / 2;
        float y = Screen.height - 130;

        GUI.color = Color.black;

        GUI.DrawTexture(
            new Rect(x, y, barWidth, barHeight),
            Texture2D.whiteTexture
        );

        GUI.color = Color.green;

        GUI.DrawTexture(
            new Rect(
                x + 2,
                y + 2,
                (barWidth - 4) * progress,
                barHeight - 4
            ),
            Texture2D.whiteTexture
        );

        GUI.color = Color.white;
    }
}