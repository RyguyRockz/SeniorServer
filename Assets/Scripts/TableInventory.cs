using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInventory : MonoBehaviour
{
    public Transform dropPoint; // Point where items will snap
    private GameObject currentItem; // The item currently on the table
    public Sink sink; // Reference to the Sink script

    public bool HasItem => currentItem != null;

    public void PlaceItem(GameObject item)
    {
        if (HasItem) return; // Do nothing if there's already an item

        currentItem = item;
        item.transform.position = dropPoint.position; // Snap to drop point
        item.transform.rotation = dropPoint.rotation;
        item.transform.SetParent(transform); // Make it a child of the table
        item.SetActive(true); // Make sure it's visible

        // Check if the placed item is named "Plate"
        if (sink != null && item.name == "Plate")
        {
            // If it's the sink, destroy the plate after a delay
            sink.StartCoroutine(sink.DestroyPlateAfterDelay(item, 1f));
        }
    }

    public GameObject PickUpItem()
    {
        if (!HasItem) return null;

        GameObject itemToReturn = currentItem;
        currentItem = null;
        itemToReturn.transform.SetParent(null); // Detach from table
        return itemToReturn;
    }
}
