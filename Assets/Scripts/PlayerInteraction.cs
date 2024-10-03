using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public LayerMask pickupLayer; // Layer for items to pick up

    // Inventory system
    public Transform itemHoldPosition1;
    public Transform itemHoldPosition2;
    public float dropOffset = 1f;
    private Stack<GameObject> inventoryStack = new Stack<GameObject>();
    private const int maxInventorySize = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Check for 'E' key press
        {
            TryInteractOrPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Drop item with 'Q'
        {
            TryDropItem();
        }
    }

    private void TryInteractOrPickup()
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        // Check if the ray hits anything in range
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            // Check if it's a pickup item
            if (hitInfo.collider.CompareTag("Pickup") && inventoryStack.Count < maxInventorySize)
            {
                PickupItem(hitInfo.collider.gameObject);
                return;
            }

            // If not a pickup, check for general interactable objects
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(); // Interact with non-pickup objects
            }
        }
    }

    private void TryDropItem()
    {
        if (inventoryStack.Count == 0) return;

        GameObject itemToDrop = inventoryStack.Pop();
        Vector3 dropPosition = transform.position + transform.forward * dropOffset;
        itemToDrop.transform.position = dropPosition;
        itemToDrop.SetActive(true); // Re-enable the dropped item

        UpdateInventoryDisplay();
    }

    private void PickupItem(GameObject item)
    {
        inventoryStack.Push(item);
        item.SetActive(false); // Hide the item while in the player's inventory
        UpdateInventoryDisplay();
    }

    private void UpdateInventoryDisplay()
    {
        // Update positions for the items in the player's inventory (above their head)
        if (inventoryStack.Count > 0)
        {
            var item1 = inventoryStack.Peek();
            item1.transform.position = itemHoldPosition1.position;
            item1.SetActive(true);
        }
        if (inventoryStack.Count > 1)
        {
            var item2 = inventoryStack.ToArray()[1];
            item2.transform.position = itemHoldPosition2.position;
            item2.SetActive(true);
        }
    }
}
