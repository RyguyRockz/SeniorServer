using UnityEngine;
using System.Collections;
using System.Linq;
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private bool isDropping = false; // Prevent overlapping drops

    public Transform InteractorSource;
    public float InteractRange;

    public GameObject[] currentItems = new GameObject[2]; // Array to hold up to 2 items
    public Transform tray; // Tray GameObject
    public Transform itemSlot1; // Slot 1 on the tray
    public Transform itemSlot2; // Slot 2 on the tray

    public GameObject interactableIndicatorPrefab; // Prefab to show above interactable object
    public GameObject tableIndicatorPrefab;
    private GameObject activeIndicator; // To store the current active indicator
    private Transform lastInteractable; // To track the last interactable object we looked at
    public LayerMask interactableLayers;

    private bool isCleaning = false; // Flag to track if the player is cleaning
    private float cleanTimer = 0f; // Timer to track how long the E key is held down
    public float cleanDuration = 2f; // Time required to clean the spill

    private Coroutine cleaningCoroutine; // To keep track of the cleaning coroutine

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal") + Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(moveInput) > 0.1f;
        playerAnimator.SetBool("IsWalking", isMoving);

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        // NEW: Check if player has items to drop
        bool hasItemToDrop = currentItems[0] != null || currentItems[1] != null;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, interactableLayers))
        {
            // NEW: Priority check for tables when holding items
            TableInventory table = hitInfo.collider.GetComponent<TableInventory>();
            if (table != null && hasItemToDrop)
            {
                if (!table.HasItem)
                {
                    // Show indicator at the table's DROP POINT (not the table's root)
                    Transform indicatorSpot = table.tableIndicatorSpot != null
                    ? table.tableIndicatorSpot
                    : table.dropPoint;

                    ShowIndicator(indicatorSpot, isTable: true);
                }
                else
                {
                    HideIndicator(); // Table is full
                }
            }
            else // Else check for other interactables
            {
                // Original logic for pickups/spills
                if (hitInfo.collider.CompareTag("Pickup") || hitInfo.collider.CompareTag("Spill") || hitInfo.collider.GetComponent<IInteractable>() != null)
                {
                    ShowIndicator(hitInfo.collider.transform, isTable: false);
                }
                else
                {
                    HideIndicator();
                }
            }

            // Existing guest interaction
            if (hitInfo.collider.CompareTag("Guest"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    playerAnimator.SetTrigger("TakeOrder");
                }
            }

            // Existing spill cleaning logic
            if (hitInfo.collider.CompareTag("Spill"))
            {
                HandleCleaning(hitInfo.collider.gameObject);
            }
            else
            {
                ResetCleaning();
            }

            // Existing E-key interactions
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hitInfo.collider.CompareTag("Pickup"))
                {
                    playerAnimator.SetTrigger("Grab");
                    audioManager.PlaySFX(audioManager.PickUpFoodSFX);
                    PickUpItem(hitInfo.collider.gameObject);
                }
                else if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }
        else // Raycast didn't hit anything
        {
            HideIndicator();
            ResetCleaning();
        }

        // Existing Q-key drop logic
        if (Input.GetKeyDown(KeyCode.Q) && !isDropping)
        {
            playerAnimator.SetTrigger("Drop");
            isDropping = true;
        }
    }

    private void HandleCleaning(GameObject spill)
    {
        // If the player holds down the E key, start the cleaning process
        if (Input.GetKey(KeyCode.E))
        {
            cleanTimer += Time.deltaTime; // Increase the timer while holding E
            playerAnimator.SetBool("IsCleaning", true); // Start cleaning loop

            if (cleanTimer >= cleanDuration && !isCleaning)
            {
                isCleaning = true;
                CleanSpill(spill); // Clean the spill
            }
            else if (cleanTimer < cleanDuration && cleaningCoroutine == null)
            {
                cleaningCoroutine = StartCoroutine(AnimateWaterScaling(spill.transform.GetChild(0))); // Animate the water scaling
                audioManager.PlaySFX(audioManager.CleanSpillSFX);
            }
        }
        else
        {
            playerAnimator.SetBool("IsCleaning", false); // Stop cleaning
            ResetCleaning(); // Reset the timer if E is not held
        }
    }

    private void ResetCleaning()
    {
        playerAnimator.SetBool("IsCleaning", false); // Explicitly stop the animation
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
        Destroy(spill); // Destroy after marking it as cleaned
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

    private void ShowIndicator(Transform interactable, bool isTable = false)
    {
        if (lastInteractable == interactable)
            return;

        HideIndicator();

        // Choose the prefab: use tableIndicatorPrefab for tables, else default  
        GameObject prefabToUse = isTable ? tableIndicatorPrefab : interactableIndicatorPrefab;
        if (prefabToUse == null)
        {
            Debug.LogError("Indicator prefab not assigned!");
            return;
        }

        // Position the indicator  
        Vector3 indicatorPos = interactable.position + Vector3.up * 1.5f;
        activeIndicator = Instantiate(prefabToUse, indicatorPos, Quaternion.identity);
        activeIndicator.transform.SetParent(interactable);
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

                //Scale the item down when picked up
                Vector3 currentScale = item.transform.localScale;
                item.transform.localScale = currentScale * 0.8f;  // Scale down by 0.8

                return;
            }
        }
        Debug.Log("Inventory full!");
    }

    private void DropItem()
    {
        // Loop through the player's inventory
        for (int i = currentItems.Length - 1; i >= 0; i--)
        {
            if (currentItems[i] != null)
            {
                Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange, interactableLayers))
                {
                    if (hitInfo.collider.TryGetComponent(out TableInventory tableInventory))
                    {
                        if (!tableInventory.HasItem)
                        {
                            tableInventory.PlaceItem(currentItems[i]);
                            EnableColliders(currentItems[i]);
                            currentItems[i].transform.SetParent(null);
                            currentItems[i] = null;

                            // FIX: Reset here after successful drop
                            isDropping = false; // <-- ADD THIS LINE
                            return;
                        }
                        else
                        {
                            Debug.Log("Table full!");
                        }
                    }
                    else
                    {
                        Debug.Log("Not a table!");
                    }
                }
                else
                {
                    Debug.Log("No table detected.");
                }

                // Reset if drop failed (e.g., no table)
                isDropping = false;
                return;
            }
        }

        // Reset if no item was found
        isDropping = false;
    }

    public void PlayDropSound()
    {
        audioManager.PlaySFX(audioManager.DropFoodSFX);
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