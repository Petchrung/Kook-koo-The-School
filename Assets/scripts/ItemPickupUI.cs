
using UnityEngine;
using TMPro;
using System.Collections;

public class ItemPickupUI : MonoBehaviour
{
    public static ItemPickupUI Instance;

    public TMP_Text messageText;

    private Coroutine hideCoroutine;

    void Awake()
    {
        Instance = this;
        messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideMessage());
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2f);

        messageText.gameObject.SetActive(false);
    }
}
