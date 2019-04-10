using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    /// <summary>
    /// Determines the driving mode of the tank.
    /// </summary>
    public DriveMode driveMode = DriveMode.POINT_STICK;
    /// <summary>
    /// The axis this tank will use to drive left and right. Defaults to player
    /// one.
    /// </summary>
    public string xDrive = "x-drive-1";
    /// <summary>
    /// The axis this tank will use to drive up and down. Defaults to player
    /// one.
    /// </summary>
    public string zDrive = "z-drive-1";
    /// <summary>
    /// The button this tank will use to drive backwards. Defaults to player
    /// one and MacOS. This is only applicable if the DriveMode <see cref="DriveMode.POINT_STICK"/>.
    /// </summary>
    public string reverse = "reverse-1-mac";
    /// <summary>
    /// The trigger this tank will use to drive forward. Defaults to player one
    /// and MacOS. This is only applicable if the DriveMode is POINT_TRIGGERS.
    /// </summary>
    public string forwardDrive = "forward-drive-1-mac";
    /// <summary>
    /// The trigger this tank will use to drive backward. Defaults to player one
    /// and MacOS. This is only applicable if the DriveMode is TRIGGERS.
    /// </summary>
    public string backwardDrive = "backward-drive-1-mac";


    /// <summary>
    /// The turn speed of the tank in radians per second.
    /// </summary>
    public float turnSpeed = 5f;
    /// <summary>
    /// The drive speed of the tank. This is only applicable if the DriveMode
    /// is TURN, POINT_TRIGGERS or TURN_TRIGGERS.
    /// </summary>
    public float driveSpeed = 50f;

    /// <summary>
    /// The Transform of the Camera that follows this Tank.
    /// </summary>
    public Transform camSpace;


    private Vector3 stickInput;
    private float triggerInput;
    private float inputSpeed;
    private Rigidbody rb;
    private int dir;


    // Start is called before the first frame update
    void Start()
    {
        stickInput = new Vector3();
        rb = GetComponent<Rigidbody>();
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateInput();

        if (driveMode == DriveMode.POINT_STICK && Input.GetButton(reverse))
        {
            dir = -1;
        } 
        else if (driveMode == DriveMode.POINT_STICK)
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
        switch (driveMode)
        {
            case DriveMode.FREE:
                MoveTankFree();
                break;

            case DriveMode.POINT_STICK:
                MoveTankWithPoint();
                break;

            case DriveMode.TURN_STICK:
                MoveTankWithTurn();
                break;

            case DriveMode.POINT_TRIGGERS:
                MoveTankWithTriggers();
                break;

            case DriveMode.TURN_TRIGGERS:
                MoveTankWithTurnTriggers();
                break;
        }
    }

    /// <summary>
    /// Updates the <see cref="stickInput"/> vector.
    /// </summary>
    private void UpdateInput()
    {
        stickInput.Set(Input.GetAxis(xDrive), 0, Input.GetAxis(zDrive));
        inputSpeed = Mathf.Max(Mathf.Abs(stickInput.x), Mathf.Abs(stickInput.z));

        // If both triggers are pressed, then forward movement takes priority
        if (Input.GetAxis(forwardDrive) != 0)
        {
            triggerInput = Input.GetAxis(forwardDrive);
            dir = 1;
        }
        else if (Input.GetAxis(backwardDrive) != 0) 
        {
            triggerInput = -1 * Input.GetAxis(backwardDrive);
            dir = -1;
        }
        else
        {
            triggerInput = 0;
            dir = 1;
        }
    }

    /// <summary>
    /// <para>Debug function.</para>
    /// Moves the tank by adding an impulse in whichever direction the left
    /// analog stick points.
    /// <para><see cref="DriveMode.FREE"/></para>
    /// </summary>
    private void MoveTankFree()
    {
        rb.AddForce(stickInput, ForceMode.Impulse);
    }

    /// <summary>
    /// Moves the tank by applying a force and turning to the direction the 
    /// left analog stick points.
    /// The force is applied to the local space's forward or backward direction 
    /// depending on whether the reverse button is pressed.
    /// <para>Corresponds to: <see cref="DriveMode.POINT_STICK"/></para>
    /// </summary>
    private void MoveTankWithPoint()
    {
        rb.AddForce(dir * inputSpeed * transform.forward, ForceMode.Impulse);
        TurnTowardInput();
    }

    /// <summary>
    /// Moves the tank by applying a force and turning left or right according
    /// to left analog stick input.
    /// The force is applied in the forward direction if the player pushes up
    /// on the left analog and vice versa.
    /// /// <para>Corresponds to: <see cref="DriveMode.TURN_STICK"/></para>
    /// </summary>
    private void MoveTankWithTurn()
    {
        rb.AddForce(stickInput.z * driveSpeed * transform.forward);
        transform.Rotate(0f, stickInput.x * turnSpeed, 0f);
    }

    /// <summary>
    /// Moves the tank by applying a force and turning to the direction the
    /// left analog stick points.
    /// The force is applied forward if the right trigger is pulled and backward
    /// if the left trigger is pulled.
    /// <para>Corresponds to: <see cref="DriveMode.POINT_TRIGGERS"/></para>
    /// </summary>
    private void MoveTankWithTriggers()
    {
        rb.AddForce(triggerInput * driveSpeed * transform.forward);
        TurnTowardInput();
    }

    /// <summary>
    /// Moves the tank by applying a force and turning left or right according
    /// to the left analog stick input.
    /// The force is applied forward if the right trigger is pulled and backward
    /// if the left trigger is pulled.
    /// <para>Corresponds to: <see cref="DriveMode.TURN_TRIGGERS"/></para>
    /// </summary>
    private void MoveTankWithTurnTriggers()
    {
        rb.AddForce(triggerInput * driveSpeed * transform.forward);
        transform.Rotate(0f, stickInput.x * turnSpeed, 0f);
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
            Vector3.RotateTowards(transform.forward, dir * stickInput.normalized, turnSpeed * Time.deltaTime, 0.0f)
        );
    }
}
