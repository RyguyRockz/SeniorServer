using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract class for all interactable objects
public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public abstract void Interact(); // Force derived classes to define interaction behavior
}
