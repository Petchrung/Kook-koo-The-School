
using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Interaction")]
    public float interactDistance = 2f;
    public KeyCode interactKey = KeyCode.E;
    public float holdDuration = 1.5f;

    [Header("Progress Bar")]
    public float barWidth = 250f;
    public float barHeight = 20f;

    private Transform player;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerNearby = false;

    private float holdTimer = 0f;

    void Start()
    {
        closedRotation = transform.localRotation;

        openRotation = closedRotation *
            Quaternion.Euler(0, openAngle, 0);

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        playerNearby = distance <= interactDistance;

        if (!playerNearby || isMoving)
        {
            holdTimer = 0f;
            return;
        }

        if (Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                holdTimer = 0f;
                StartCoroutine(ToggleDoor());
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    IEnumerator ToggleDoor()
    {
        isMoving = true;

        Quaternion target = isOpen
            ? closedRotation
            : openRotation;

        while (Quaternion.Angle(
            transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                target,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        transform.localRotation = target;

        isOpen = !isOpen;
        isMoving = false;
    }

    void OnGUI()
    {
        if (!playerNearby || isMoving)
            return;

        GUIStyle textStyle = new GUIStyle();

        textStyle.fontSize = 22;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;

        string message = isOpen
            ? "กด E ค้าง เพื่อปิดประตู"
            : "กด E ค้าง เพื่อเปิดประตู";

        GUI.Label(
            new Rect(
                Screen.width / 2 - 200,
                Screen.height - 180,
                400,
                40
            ),
            message,
            textStyle
        );

        // Progress Bar
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