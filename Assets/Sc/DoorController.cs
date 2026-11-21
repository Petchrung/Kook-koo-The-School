
using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Interaction")]
    public float interactDistance = 0.3f;
    public KeyCode interactKey = KeyCode.E;

    private Transform player;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerNearby = false;

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

        if (playerNearby && !isMoving &&
            Input.GetKeyDown(interactKey))
        {
            StartCoroutine(ToggleDoor());
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

        string message = isOpen
            ? "[ E ] กดเพื่อปิดประตู"
            : "[ E ] กดเพื่อเปิดประตู";

        GUIStyle style = new GUIStyle();

        style.fontSize = 22;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(
                Screen.width / 2 - 200,
                Screen.height - 130,
                400,
                50
            ),
            message,
            style
        );
    }
}