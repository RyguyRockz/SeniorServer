using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    private CharacterController controller;

    void Start()
    {

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Capture input from the player
        float moveHorizontal = Input.GetAxisRaw("Horizontal"); // A/D 
        float moveVertical = Input.GetAxisRaw("Vertical");     // W/S 

        //Movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        //Move the player
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
}

