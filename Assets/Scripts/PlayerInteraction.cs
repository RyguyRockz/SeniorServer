using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public Transform itemHolder; // Parent object to hold items above player
    public GameObject[] currentItems = new GameObject[2]; // Array to hold up to 2 items

    public GameObject interactableIndicatorPrefab; // Prefab to show above interactable object
    private GameObject activeIndicator; // To store the current active indicator
    private Transform lastInteractable; // To track the last interactable object we looked at
    public LayerMask interactableLayers;

    private bool isCleaning = false; // Flag to track if the player is cleaning
    private float cleanTimer = 0f; // Timer to track how long the E key is held down
    public float cleanDuration = 2f; // Time required to clean the spill

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
    }

    private void CleanSpill(GameObject spill)
    {
        Destroy(spill);
        SpillManager.Instance.OnSpillCleaned(spill); // Notify the manager
        Debug.Log("Spill cleaned!");
        ResetCleaning();
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
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] == null)
            {
                currentItems[i] = item;
                item.SetActive(false);
                item.transform.position = itemHolder.position + new Vector3(0, -i * 0.5f, 0);
                item.transform.SetParent(itemHolder);
                return;
            }
        }
        Debug.Log("Inventory full!");
    }

    private void DropItem()
    {
        for (int i = currentItems.Length - 1; i >= 0; i--)
        {
            if (currentItems[i] != null)
            {
                currentItems[i].SetActive(true);
                currentItems[i].transform.SetParent(null);
                currentItems[i].transform.position = InteractorSource.position + InteractorSource.forward;
                currentItems[i] = null;
                return;
            }
        }
    }
}
