using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Create a list of subscribers for mouse input
    public static List<Action<Vector2, Vector3, Vector2>> mousePosActions = new();
    // Create a list of subscribers for movement input
    public static List<Action<Vector2>> moveInputActions = new();
    // Create a list of subscribers for jump input
    public static List<Action<float>> jumpInputActions = new();
    float jumpHeldTime;
    void Update()
    {
        // Get screen position of mouse
        Vector2 mouseScreenPos = Input.mousePosition;

        // Convert screen position to world position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        // Get mouse axis input
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        foreach (var action in mousePosActions)
        {
            // Run subscribed method
            action(mouseScreenPos, mouseWorldPos, mouseDelta);
        }
        // Get moveAxis input
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        foreach(var action in moveInputActions)
        {
            // Run subscribed method
            action(moveInput);
        }

        //On jump button
        if(Input.GetButton("Jump"))
        {
            //Increase jump held time
            jumpHeldTime += Time.deltaTime;
        }
        else
        {
            //if no jump button input then set held time to zero
            jumpHeldTime = 0f;
        }
        foreach (var action in jumpInputActions)
        {
            //run subscribed method
            action(jumpHeldTime);
        }
    }

    // Subscribe method for mouseInput
    public static void Subscribe_MouseInput(Action<Vector2, Vector3, Vector2> action)
    {
        // Add action to list
        mousePosActions.Add(action);
    }

    // Subscribe method for moveInput
    public static void Subscribe_MoveInput(Action<Vector2> action)
    {
        // Add action to list
        moveInputActions.Add(action);
    }
    
    //Subscribe method for jumpInput
    public static void Subscribe_JumpInput(Action<float> action)
    {
        //Add action to list
        jumpInputActions.Add(action);
    }
}
