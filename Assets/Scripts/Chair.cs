using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool IsOccupied = false;
    public Transform GuestFacingPoint; // Add this public field to assign the direction the guest should face when seated
}