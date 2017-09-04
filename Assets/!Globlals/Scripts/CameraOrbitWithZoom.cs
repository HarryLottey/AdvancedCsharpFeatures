﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbitWithZoom : MonoBehaviour
{

    public Transform target; // The target point to rotate around
    public float distance = 0.5f; // The distance between the camera's point and the target
    public float sensitivity = 1f; // How sensitive the input is for rotating


    public int distanceMin;
    public int distanceMax;

    // Stored X and Y rotation in eulerAngles
    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        // Get current axis of rotation on X and Y
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        // IF right mouse button is pressed
        if (Input.GetMouseButtonDown(1) == true)
        {
            // Hide the cursor
            HideCursor(true);
            // GetInput()
            GetInput();
        }
        // ELSE
        else
        {
            // Unhide cursor
            HideCursor(false);
            // Movement()
            Movement();
        }


    }

    void HideCursor(bool isHiding)
    {
        // Is the cursor supposed to be hiding?
        if (isHiding)
        {
            // Hide the cursor
            Cursor.visible = false;
        }
        else
        {
            // Unhide the cursor
            Cursor.visible = true;
        }
    }

    void GetInput()
    {
        // Gather X and Y mouse offset input to rotate camera (by sensitivity)
        x += Input.GetAxis("Mouse X") * sensitivity;
        // Opposite direction for Y because it is inverted
        y -= Input.GetAxis("Mouse Y") * sensitivity;

        // Get mouse ScrollWheelinput offset for changing distance
        float inputScroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - inputScroll, distanceMin, distanceMax);
    }

    void Movement()
    {
        // Check if a target has been set
        if (target)
        {
            // Convert x and y rotations to quarternion using euler
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // Calculate new position offset using rotation
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            // Apply rotation and position to transform
            transform.rotation = rotation;
            transform.position = position;
        }
    }
}