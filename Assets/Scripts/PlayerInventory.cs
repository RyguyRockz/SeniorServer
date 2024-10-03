using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Transform itemHoldPosition1; // The position for the first item in the stack (above player's head)
    public Transform itemHoldPosition2; // The position for the second item in the stack (higher up)
    public float dropOffset = 1f; // How far from the player the items are dropped

    private Stack<GameObject> inventoryStack = new Stack<GameObject>();
    private const int maxInventorySize = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryDropItem();
        }
    }

    private void TryPickupItem()
    {
        if (inventoryStack.Count >= maxInventorySize) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Pickup"))
            {
                PickupItem(hitCollider.gameObject);
                break;
            }
        }
    }

    private void TryDropItem()
    {
        if (inventoryStack.Count == 0) return;

        GameObject itemToDrop = inventoryStack.Pop();
        Vector3 dropPosition = transform.position + transform.forward * dropOffset;
        itemToDrop.transform.position = dropPosition;
        itemToDrop.SetActive(true);

        UpdateInventoryDisplay();
    }

    private void PickupItem(GameObject item)
    {
        inventoryStack.Push(item);
        item.SetActive(false); // Hide the item from the scene

        UpdateInventoryDisplay();
    }

    private void UpdateInventoryDisplay()
    {
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