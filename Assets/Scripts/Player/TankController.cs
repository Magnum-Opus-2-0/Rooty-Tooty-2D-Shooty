﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{

    /// <summary>
    /// The axis this tank will use to drive left and right. Defaults to player
    /// one and MacOS.
    /// </summary>
    public string xDrive = "x-drive-1-mac";
    /// <summary>
    /// The axis this tank will use to drive up and down. Defaults to player
    /// one and MacOS.
    /// </summary>
    public string zDrive = "z-drive-1-mac";
    /// <summary>
    /// The axis this tank will use to aim left and right. Defaults to player
    /// one and MacOS.
    /// </summary>
    public string xAim = "x-aim-1-mac";
    /// <summary>
    /// The axis this tank will use to aim up and down. Defaults to player one
    /// and MacOS.
    /// </summary>
    public string zAim = "z-aim-1-mac";
    /// <summary>
    /// The button this tank will use to drive backwards. Defaults to player
    /// one and MacOS. 
    /// </summary>
    public string reverse = "reverse-1-mac";

    /// <summary>
    /// The turn speed of the tank in radians per second.
    /// </summary>
    public float turnSpeed = 5;

    /// <summary>
    /// The Transform of the Camera that follows this Tank.
    /// </summary>
    public Transform camSpace;


    private Vector3 input;
    private float inputSpeed;
    private Rigidbody rb;
    private int dir;


    // Start is called before the first frame update
    void Start()
    {
        input = new Vector3();
        rb = GetComponent<Rigidbody>();
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateInput();

        if (Input.GetButton(reverse))
        {
            dir = -1;
        } else
        {
            dir = 1;
        }

        //DEBUG make sure to take this out later
        if (Input.GetButtonUp("reset-pos"))
        {
            transform.position = new Vector3(0, .3f, 0);
        }
        

    }

    private void FixedUpdate()
    {
        MoveTankToInput();
    }

    /// <summary>
    /// Updates the <see cref="input"/> vector.
    /// </summary>
    private void UpdateInput()
    {
        input.Set(Input.GetAxis(xDrive), 0, Input.GetAxis(zDrive));
        inputSpeed = Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.z));
    }

    /// <summary>
    /// <para>Debug function.</para>
    /// Moves the tank by adding an impulse in whichever direction the left
    /// analog stick points.
    /// </summary>
    private void MoveTankFree()
    {
        rb.AddForce(input, ForceMode.Impulse);
    }

    /// <summary>
    /// Moves the tank by applying a force and turning to the direction the 
    /// analog stick points.
    /// The force is applied to the local space's forward or backward direction 
    /// depending on whether the reverse button is pressed.
    /// </summary>
    private void MoveTankToInput()
    {
        rb.AddForce(dir * inputSpeed * transform.forward, ForceMode.Impulse);
        TurnTowardInput();
    }

    /// <summary>
    /// Turns the tank in the direction given by the input vector relative to
    /// the camSpace.
    /// </summary>
    private void TurnTowardInput()
    {
        // We want to rotate the tank a little at a time until it points in the
        // direction we are pointing with the left analog stick
        // So this is going to interpolate between the world space forward 
        // vector and the input vector at a speed of turnSpeed
        transform.rotation = Quaternion.LookRotation(
            Vector3.RotateTowards(transform.forward, transform.forward + input.normalized, turnSpeed * Time.deltaTime, 0.0f)
        );
    }
}
