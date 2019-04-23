using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankController : MonoBehaviour
{

    private HealthBehavior hb;
    private TankStates state;
    public TankStates State
    {
        get { return state; }
    }
    private bool wasNotDead;

    #region MEMBERS_RESOURCES
    private int fluff;
    public int Fluff
    {
        get
        {
            return fluff;
        }

        set
        {
            fluff = value;
        }
    }

    private int plastic;
    public int Plastic
    {
        get
        {
            return plastic;
        }
        set
        {
            plastic = value;
        }
    }
    #endregion


    #region MEMBERS_MOVEMENT
    /// <summary>
    /// Determines the driving mode of the tank. Defaults to using the analog
    /// stick.
    /// </summary>
    public DriveMode driveMode = DriveMode.Stick;
    /// <summary>
    /// Determines which method the tank will be steered with. Defaults to
    /// <see cref="SteerMode.Point"/>.
    /// </summary>
    public SteerMode steerMode = SteerMode.Point;
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
    /// one and MacOS. This is only applicable if the DriveMode <see cref="DriveMode.Stick"/>.
    /// </summary>
    public string reverse = "reverse-1-mac";
    /// <summary>
    /// The trigger this tank will use to drive forward. Defaults to player one
    /// and MacOS. This is only applicable if the DriveMode is <see cref="DriveMode.Triggers"/>.
    /// </summary>
    public string forwardDrive = "forward-drive-1-mac";
    /// <summary>
    /// The trigger this tank will use to drive backward. Defaults to player one
    /// and MacOS. This is only applicable if the DriveMode is <see cref="DriveMode.Triggers"/>.
    /// </summary>
    public string backwardDrive = "backward-drive-1-mac";


    /// <summary>
    /// The turn speed of the tank in radians per second.
    /// </summary>
    public float turnSpeed = 3f;
    /// <summary>
    /// The maximum speed of the tank.
    /// </summary>
    public float maxSpeed = 4f;
    /// <summary>
    /// The acceleration of the tank. That is, how fast its speed will increase
    /// to <see cref="maxSpeed"/>.
    /// </summary>
    public float accelerate = 2.5f;


    /// <summary>
    /// The input from the left analog stick. Not normalized.
    /// </summary>
    private Vector3 stickInput;
    /// <summary>
    /// The input from the triggers. The range is [-1, 1], where [-1, 0) when
    /// the left trigger is pushed or (0, 1] when the right trigger is pushed.
    /// </summary>
    private float triggerInput;
    /// <summary>
    /// How much the left analog stick has been pushed.
    /// Equal to max(x-axis, z-axis).
    /// </summary>
    private float inputSpeed;
    /// <summary>
    /// The <see cref=" Rigidbody"/> attached to this Tank. 
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// Controls the direction of driving when driveMode is
    /// <see cref="DriveMode.Stick"/>.
    /// Also adjusts steering to make it feel more natural when steerMode is
    /// <see cref="SteerMode.Point"/>
    /// </summary>
    private int dir;
    /// <summary>
    /// The current speed of the Tank.
    /// </summary>
    private float curSpeed;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Determine which OS the game is being run on, and set controller
        // mappings accordingly.
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:

                forwardDrive = (tag == "Player_1" ? "forward-drive-1-mac" : "forward-drive-2-mac");
                backwardDrive = (tag == "Player_1" ? "backward-drive-1-mac" : "backward-drive-2-mac");
                reverse = (tag == "Player_1" ? "reverse-1-mac" : "reverse-2-mac");
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                forwardDrive = (tag == "Player_1" ? "forward-drive-1-win" : "forward-drive-2-win");
                backwardDrive = (tag == "Player_1" ? "backward-drive-1-win" : "backward-drive-2-win");
                reverse = (tag == "Player_1" ? "reverse-1-win" : "reverse-2-win");
                break;

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                break;
        }
        // Left analog stick is the same mapping on Mac OS and Windows
        xDrive = (tag == "Player_1" ? "x-drive-1" : "x-drive-2");
        zDrive = (tag == "Player_1" ? "z-drive-1" : "z-drive-2");

        // Define movement starting values
        stickInput = new Vector3();
        rb = transform.GetComponent<Rigidbody>();
        dir = 1;

        // Define resource starting values
        Fluff = 0;
        Plastic = 0;

        // Get the reference to the HealthBehavior script\
        hb = GetComponent<HealthBehavior>();
        state = TankStates.Alive;
        wasNotDead = true;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateState();

        UpdateMoveInput();

        if (IsDriving())
        {
            AccelerateTank();
        }
        else
        {
            AccelerateTank(-1);
        }

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
    /// Updates the different move input fields depending upon the Drive and
    /// Steer modes.
    /// </summary>
    private void UpdateMoveInput()
    {
        stickInput.Set(Input.GetAxis(xDrive), 0, Input.GetAxis(zDrive));
        inputSpeed = Mathf.Max(Mathf.Abs(stickInput.x), Mathf.Abs(stickInput.z));

        // This block determined the outcome of the dir variable depending on
        // which drive mode is being used.
        switch (driveMode) {
            case DriveMode.Stick:
                // Only allow the player to switch directions while they are not
                // driving
                if (!IsDriving())
                {
                    if (Input.GetButton(reverse))
                    {
                        dir = -1;
                    }
                    else
                    {
                        dir = 1;
                    }
                }
                break;

            case DriveMode.Triggers:
                // If both triggers are pressed, then forward movement takes priority
                #pragma warning disable RECS0018 // Suppressing floating point comparison warning because Unity promises to return exactly 0 if it receives no input.
                if (Input.GetAxis(forwardDrive) != 0)
                #pragma warning restore RECS0018
                {
                    triggerInput = Input.GetAxis(forwardDrive);
                    dir = 1;
                }
                #pragma warning disable RECS0018 // Suppressing floating point comparison warning because Unity promises to return exactly 0 if it receives no input.
                else if (Input.GetAxis(backwardDrive) != 0)
                #pragma warning restore RECS0018
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
    /// <para><see cref="DriveMode.Free"/></para>
    /// </summary>
    private void MoveTankFree()
    {
        rb.AddForce(stickInput, ForceMode.Impulse);
    }

    /// <summary>
    /// Drives the tank forward or backwards by applying an Impulse.
    /// </summary>
    /// <param name="inputMod">The input modifier of the Tank. That is, how much
    /// the player is pushing on the triggers or analog stick. Also determines
    /// whether the force is applied forward or backward.</param>
    public void DriveTank(float inputMod)
    {
        // Using this temporary variable to make modifications to speed easier
        // later. In case we want to add powerups that increase the speed by 10
        // temporarily or whatever.
        float speed = curSpeed;
        rb.AddForce(inputMod * speed * transform.forward, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Turns the tank to point in the direction given by the input vector
    /// relative to world space.
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
            case SteerMode.Point:
                PointTank();
                break;

            case SteerMode.Turn:
                TurnTank();
                break;
        }

        // Determine which mode the user is driving with then accelerate the tank.
        if (driveMode == DriveMode.Stick && steerMode == SteerMode.Point)
        {
            DriveTank(dir * inputSpeed);
        }
        else if (driveMode == DriveMode.Stick && steerMode == SteerMode.Turn)
        {
            DriveTank(stickInput.z);
        }
        else if (driveMode == DriveMode.Triggers)
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

    /// <summary>
    /// Accelerates the tank in the given direction.
    /// </summary>
    /// <param name="direction">Direction.</param>
    public void AccelerateTank(int direction=1)
    {
        curSpeed += direction * accelerate * Time.fixedDeltaTime;
        curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);
    }

    /// <summary>
    /// Determines whether the Tank is driving. That is, whether the controller
    /// is receiving input corresponding to the current <see cref="DriveMode"/>.
    /// </summary>
    /// <returns><c>true</c>, if the tank is driving, <c>false</c> otherwise.</returns>
    public bool IsDriving()
    {
        if (driveMode == DriveMode.Stick)
        {
            return inputSpeed > 0;
        }

        if (driveMode == DriveMode.Triggers)
        {
            #pragma warning disable RECS0018 // Suppressing floating point comparison warning because triggerInput is set to exactly 0 if we get no input.
            return triggerInput != 0;
            #pragma warning restore RECS0018
        }

        return false;
    }

    /// <summary>
    /// Updates the <see cref="state"/> Only modifies the state variable if the
    /// player has died or respawned since last Update.
    /// </summary>
    public void UpdateState()
    {
        if (wasNotDead != hb.isNotDead() && hb.isNotDead())
        {
            state = TankStates.Alive;
            Debug.Log("IT'S ALIVE");
        }
        else if (wasNotDead != hb.isNotDead() && !hb.isNotDead())
        {
            state = TankStates.Dead;
            Debug.Log("She's dead, Jim");
        }

        wasNotDead = hb.isNotDead();
    }
}
