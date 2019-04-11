using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    /// <summary>
    /// Determines the driving mode of the tank. Defaults to using the analog
    /// stick.
    /// </summary>
    public DriveMode driveMode = DriveMode.STICK;
    /// <summary>
    /// Determines which method the tank will be steered with. Defaults to
    /// <see cref="SteerMode.POINT"/>.
    /// </summary>
    public SteerMode steerMode = SteerMode.POINT;
    /// <summary>
    /// The axis this tank will use to drive left and right. Defaults to player
    /// one.
    /// </summary>
    public string xDrive1 = "x-drive-1";
    /// <summary>
    /// The axis this tank will use to drive up and down. Defaults to player
    /// one.
    /// </summary>
    public string zDrive1 = "z-drive-1";
    /// <summary>
    /// The button this tank will use to drive backwards. Defaults to player
    /// one and MacOS. This is only applicable if the DriveMode <see cref="DriveMode.STICK"/>.
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
    private Rigidbody p1_rb;
    private Rigidbody p2_rb;
    private int dir;


    // Start is called before the first frame update
    void Start()
    {
        stickInput = new Vector3();
        p1_rb = GameObject.FindGameObjectWithTag("Player_1").GetComponent<Rigidbody>();
        p2_rb = GameObject.FindGameObjectWithTag("Player_2").GetComponent<Rigidbody>();
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateInput();

        //DEBUG make sure to take this out later
        if (Input.GetButtonUp("reset-pos"))
        {
            transform.position = new Vector3(0, .3f, 0);
        }
        

    }

    private void FixedUpdate()
    {
        MoveTank();
    }

    /// <summary>
    /// Updates the <see cref="stickInput"/> vector.
    /// </summary>
    private void UpdateInput()
    {
        stickInput.Set(Input.GetAxis(xDrive1), 0, Input.GetAxis(zDrive1));
        inputSpeed = Mathf.Max(Mathf.Abs(stickInput.x), Mathf.Abs(stickInput.z));

        // This block determined the outcome of the dir variable depending on
        // which drive mode is being used.
        switch (driveMode) {
            case DriveMode.STICK:
                if (Input.GetButton(reverse))
                {
                    dir = -1;
                }
                else
                {
                    dir = 1;
                }
                break;

            case DriveMode.TRIGGERS:
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
                break;
        }
    }

    /// <summary>
    /// <para>WARNING: This debug function does very silly things.</para>
    /// Moves the tank by adding an impulse in whichever direction the left
    /// analog stick points.
    /// <para><see cref="DriveMode.FREE"/></para>
    /// </summary>
    private void MoveTankFree()
    {
        p1_rb.AddForce(stickInput, ForceMode.Impulse);
    }

    /// <summary>
    /// Drives the tank forward or backwards by applying an Implulse.
    /// </summary>
    /// <param name="speed">The speed of the Tank. Also determines whether the
    /// force is applied forward or backward.</param>
    public void DriveTank(float speed)
    {
        p1_rb.AddForce(speed * transform.forward, ForceMode.Impulse);
    }

    /// <summary>
    /// Turns the tank to point in the direction given by the input vector 
    /// relative to the camSpace.
    /// </summary>
    private void PointTank()
    {
        // We want to rotate the tank a little at a time until it points in the
        // direction we are pointing with the left analog stick
        // So this is going to interpolate between the world space forward 
        // vector and the input vector at a speed of turnSpeed
        transform.rotation = Quaternion.LookRotation(
            Vector3.RotateTowards(transform.forward, dir * stickInput.normalized, turnSpeed * Time.deltaTime, 0.0f)
        );
    }

    /// <summary>
    /// Turns the tank left or right depending on the input from the left analog
    /// stick.
    /// </summary>
    private void TurnTank()
    {
        transform.Rotate(0f, stickInput.x * turnSpeed, 0f);
    }

    /// <summary>
    /// Moves the tank according to the drive and steer modes specified.
    /// </summary>
    private void MoveTank()
    {
        // Determine which mode the user is steering with then angle the tank.
        switch (steerMode)
        {
            case SteerMode.POINT:
                PointTank();
                break;

            case SteerMode.TURN:
                TurnTank();
                break;
        }

        // Determine which mode the user is driving with then accelerate the tank.
        if (driveMode == DriveMode.STICK && steerMode == SteerMode.POINT)
        {
            DriveTank(dir * inputSpeed);
        }
        else if (driveMode == DriveMode.STICK && steerMode == SteerMode.TURN)
        {
            DriveTank(stickInput.z);
        }
        else if (driveMode == DriveMode.TRIGGERS)
        {
            DriveTank(triggerInput);
        }
        else
        {
            // Unless the driveMode was specifically set to FREE, this indicates
            // some kind of error.
            MoveTankFree();
        }
    }
}
