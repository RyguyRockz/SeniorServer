using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool IsOccupied { get; set; } // Indicates if the chair is occupied by a guest
    private GameObject currentPlate; // The plate currently on this chair

    public Transform GuestFacingPoint; // Add this line to reference where the guest should face when seated

    // Check if there is a plate on the chair
    public bool HasPlate()
    {
        return currentPlate != null; // Return true if currentPlate is set
    }

    // Call this method when a plate is added to the chair
    public void SetPlate(GameObject plate)
    {
        currentPlate = plate;
        IsOccupied = true; // Mark the chair as occupied
    }

    // Call this method when a plate is removed from the chair
    public void RemovePlate()
    {
        currentPlate = null;
        IsOccupied = false; // Mark the chair as unoccupied
    }
}
