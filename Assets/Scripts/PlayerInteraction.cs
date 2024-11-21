using UnityEngine;
using System.Collections;
using System.Linq;
public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;

    public GameObject[] currentItems = new GameObject[2]; // Array to hold up to 2 items
    public Transform tray; // Tray GameObject
    public Transform itemSlot1; // Slot 1 on the tray
    public Transform itemSlot2; // Slot 2 on the tray

    public GameObject interactableIndicatorPrefab; // Prefab to show above interactable object
    private GameObject activeIndicator; // To store the current active indicator
    private Transform lastInteractable; // To track the last interactable object we looked at
    public LayerMask interactableLayers;

    private bool isCleaning = false; // Flag to track if the player is cleaning
    private float cleanTimer = 0f; // Timer to track how long the E key is held down
    public float cleanDuration = 2f; // Time required to clean the spill

    private Coroutine cleaningCoroutine; // To keep track of the cleaning coroutine

    private void Update()
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        // Raycast only hits objects on the specified layers
        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, interactableLayers))
        {
            // Check if the object hit by the ray is interactable (like a Pickup or Spill)
            if (hitInfo.collider.CompareTag("Pickup") || hitInfo.collider.CompareTag("Spill") || hitInfo.collider.GetComponent<IInteractable>() != null)
            {
                ShowIndicator(hitInfo.collider.transform); // Show indicator above the interactable object
            }
            else
            {
                HideIndicator(); // Hide the indicator if no valid object is detected
            }

            // Check if we are looking at a spill to start cleaning
            if (hitInfo.collider.CompareTag("Spill"))
            {
                HandleCleaning(hitInfo.collider.gameObject);
            }
            else
            {
                ResetCleaning(); // Reset cleaning timer if no spill is in view
            }

            // Handle normal interactions (picking up items, interacting with IInteractable objects)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hitInfo.collider.CompareTag("Pickup"))
                {
                    PickUpItem(hitInfo.collider.gameObject);
                }
                else if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            HideIndicator(); // Hide the indicator when not looking at anything
            ResetCleaning(); // Reset cleaning if no interactable object is in view
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    private void HandleCleaning(GameObject spill)
    {
        // If the player holds down the E key, start the cleaning process
        if (Input.GetKey(KeyCode.E))
        {
            cleanTimer += Time.deltaTime; // Increase the timer while holding E

            if (cleanTimer >= cleanDuration && !isCleaning)
            {
                isCleaning = true;
                CleanSpill(spill); // Clean the spill
            }
            else if (cleanTimer < cleanDuration && cleaningCoroutine == null)
            {
                cleaningCoroutine = StartCoroutine(AnimateWaterScaling(spill.transform.GetChild(0))); // Animate the water scaling
            }
        }
        else
        {
            ResetCleaning(); // Reset the timer if E is not held
        }
    }

    private void ResetCleaning()
    {
        cleanTimer = 0f;
        isCleaning = false;
        if (cleaningCoroutine != null)
        {
            StopCoroutine(cleaningCoroutine);
            cleaningCoroutine = null;
        }
    }

    private void CleanSpill(GameObject spill)
    {
        SpillManager.Instance.OnSpillCleaned(spill); // Notify the manager
        Destroy(spill);
        Debug.Log("Spill cleaned!");
        ResetCleaning();
    }

    private IEnumerator AnimateWaterScaling(Transform waterObject)
    {
        Vector3 initialScale = waterObject.localScale;
        Vector3 targetScale = Vector3.zero;
        float timeElapsed = 0f;

        // Scale down the water object over 2 seconds
        while (timeElapsed < 2f)
        {
            waterObject.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / 2f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        waterObject.localScale = targetScale; // Ensure it reaches zero scale
        cleaningCoroutine = null; // Reset the coroutine reference
    }

    private void ShowIndicator(Transform interactable)
    {
        // Check if we're looking at the same object as before
        if (lastInteractable == interactable)
        {
            return; // Already showing indicator for this object
        }

        // If we were showing an indicator for a different object, hide it first
        HideIndicator();

        // Instantiate or move the indicator above the new interactable object
        activeIndicator = Instantiate(interactableIndicatorPrefab, interactable.position + Vector3.up * 1.5f, Quaternion.identity);
        activeIndicator.transform.SetParent(interactable); // Attach it to the object
        lastInteractable = interactable;
    }

    private void HideIndicator()
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator); // Destroy the current indicator
            activeIndicator = null;
        }

        lastInteractable = null; // Reset the last interactable object
    }

    private void PickUpItem(GameObject item)
    {
        TableInventory tableInventory = item.GetComponentInParent<TableInventory>();
        if (tableInventory != null && tableInventory.HasItem)
        {
            Debug.Log("Picking Up Item off Table");
            // Pick up the item from the table
            item = tableInventory.PickUpItem();
        }

        // Disable colliders for the item and its children
        DisableColliders(item);

        // Find an empty slot (itemSlot1 or itemSlot2) and place the item there
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] == null)
            {
                currentItems[i] = item;
                item.SetActive(true);  // Reactivate the item when it's picked up

                // Position the item in its respective slot
                Transform slot = (i == 0) ? itemSlot1 : itemSlot2;
                item.transform.position = slot.position;
                item.transform.SetParent(slot);  // Parent the item to the slot for easy positioning

                // Scale the item down when picked up
                Vector3 currentScale = item.transform.localScale;
                item.transform.localScale = currentScale * 0.8f;  // Scale down by 0.8

                return;
            }
        }
        Debug.Log("Inventory full!");
    }

    private void DropItem()
    {
        // Loop through the player's inventory from the last item to the first
        for (int i = currentItems.Length - 1; i >= 0; i--)
        {
            if (currentItems[i] != null)
            {
                // Cast a ray to check if the player is looking at a table with a TableInventory
                Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, interactableLayers))
                {
                    if (hitInfo.collider.TryGetComponent(out TableInventory tableInventory))
                    {
                        if (!tableInventory.HasItem) // Place item if table is empty
                        {
                            // Place the item in inventory
                            tableInventory.PlaceItem(currentItems[i]);

                            // Scale the item back by dividing by .8
                            currentItems[i].transform.localScale /= .8f;

                            // Re-enable colliders for the item and its children
                            EnableColliders(currentItems[i]);

                            // Remove the item from the player's inventory and reset its parent
                            currentItems[i].transform.SetParent(null);  // Remove the item from the slot
                            currentItems[i] = null; // Remove from the player's inventory
                            return;
                        }
                        else
                        {
                            Debug.Log("Table already has an item!");
                        }
                    }
                    else
                    {
                        Debug.Log("You can only place items on a table!");
                    }
                }
                else
                {
                    Debug.Log("No table detected. Drop action canceled.");
                }

                return;
            }
        }
    }


    private void PlaceOnTray(GameObject item, Transform slot)
    {
        item.SetActive(true);
        item.transform.SetParent(slot);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    private void ClearSlot(Transform slot)
    {
        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisableColliders(GameObject item)
    {
        // Disable the collider on the item itself (if it's a BoxCollider or CapsuleCollider)
        BoxCollider itemBoxCollider = item.GetComponent<BoxCollider>();
        if (itemBoxCollider != null)
        {
            itemBoxCollider.enabled = false;
        }

        CapsuleCollider itemCapsuleCollider = item.GetComponent<CapsuleCollider>();
        if (itemCapsuleCollider != null)
        {
            itemCapsuleCollider.enabled = false;
        }

        // Recursively disable colliders on all child objects (if any)
        foreach (Transform child in item.transform)
        {
            BoxCollider childBoxCollider = child.GetComponent<BoxCollider>();
            if (childBoxCollider != null)
            {
                childBoxCollider.enabled = false;
            }

            CapsuleCollider childCapsuleCollider = child.GetComponent<CapsuleCollider>();
            if (childCapsuleCollider != null)
            {
                childCapsuleCollider.enabled = false;
            }
        }
    }

    private void EnableColliders(GameObject item)
    {
        // Re-enable the collider on the item itself (if it's a BoxCollider or CapsuleCollider)
        BoxCollider itemBoxCollider = item.GetComponent<BoxCollider>();
        if (itemBoxCollider != null)
        {
            itemBoxCollider.enabled = true;
        }

        CapsuleCollider itemCapsuleCollider = item.GetComponent<CapsuleCollider>();
        if (itemCapsuleCollider != null)
        {
            itemCapsuleCollider.enabled = true;
        }

        // Recursively re-enable colliders on all child objects (if any)
        foreach (Transform child in item.transform)
        {
            BoxCollider childBoxCollider = child.GetComponent<BoxCollider>();
            if (childBoxCollider != null)
            {
                childBoxCollider.enabled = true;
            }

            CapsuleCollider childCapsuleCollider = child.GetComponent<CapsuleCollider>();
            if (childCapsuleCollider != null)
            {
                childCapsuleCollider.enabled = true;
            }
        }
    }
}
