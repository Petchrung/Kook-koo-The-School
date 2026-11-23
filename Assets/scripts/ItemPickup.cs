
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
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
}
