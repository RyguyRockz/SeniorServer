using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public Transform itemHolder; // Parent object to hold items above player
    public GameObject[] currentItems = new GameObject[2]; // Array to hold up to 2 items

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Check for 'E' key press
        {
            Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
            {
                // Check if it's a Pickup item
                if (hitInfo.collider.CompareTag("Pickup"))
                {
                    PickUpItem(hitInfo.collider.gameObject);
                }
                // Check if it's an interactable terminal
                else if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(); // Trigger interaction if the object implements IInteractable
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Check for 'Q' key press
        {
            DropItem();
        }
    }

    private void PickUpItem(GameObject item)
    {
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] == null) // Find an empty slot
            {
                currentItems[i] = item; // Store the item
                item.SetActive(false); // Optionally deactivate the item in the world

                // Set the item's position above the player
                item.transform.position = itemHolder.position + new Vector3(0, -i * 0.5f, 0); // Adjust position
                item.transform.SetParent(itemHolder); // Parent it to the holder
                return;
            }
        }

        Debug.Log("Inventory full!"); // Handle full inventory
    }

    private void DropItem()
    {
        for (int i = currentItems.Length - 1; i >= 0; i--) // Start from the top item
        {
            if (currentItems[i] != null) // Check if there's an item
            {
                currentItems[i].SetActive(true); // Reactivate the item
                currentItems[i].transform.SetParent(null); // Remove parenting
                currentItems[i].transform.position = InteractorSource.position + InteractorSource.forward; // Drop the item

                currentItems[i] = null; // Clear the inventory slot
                return; // Drop only the top item
            }
        }
    }
}
