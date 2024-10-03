using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : InteractableObject
{
    public GameObject terminalCanvas; 

    public override void Interact()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.SetActive(true); // Show the terminal canvas
        }
    }
}
