using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInteraction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Check for 'E' key press
        {
            Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(); // Trigger interaction if the object implements IInteractable
                }
            }
        }
    }
}
