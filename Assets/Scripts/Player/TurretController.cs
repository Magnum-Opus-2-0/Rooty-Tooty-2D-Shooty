using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

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
    /// The turn speed of the turret in radians per second.
    /// </summary>
    public float turnSpeed = 3f;

    private Vector3 input;
    private Vector3 lastRot;

    // Start is called before the first frame update
    void Start()
    {
        // Determine which OS the game is being run on, and set controller
        // mappings accordingly.
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                xAim = (tag == "Player_1" ? "x-aim-1-mac" : "x-aim-2-mac");
                zAim = (tag == "Player_1" ? "z-aim-1-mac" : "z-aim-2-mac");
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                xAim = (tag == "Player_1" ? "x-aim-1-win" : "x-aim-2-win");
                zAim = (tag == "Player_1" ? "z-aim-1-win" : "z-aim-2-win");
                break;

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                break;
        }

        input = new Vector3();
        lastRot = transform.parent.forward;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        if (input.magnitude > 0)
        {
            TurnTurret();
        } 
        else
        {
            HoldTurretRotation();
        }
    }

    private void UpdateInput()
    {
        input.Set(Input.GetAxis(xAim), 0f, Input.GetAxis(zAim));
    }

    /// <summary>
    /// Turns the turret in the direction given by the input vector relative to
    /// the camSpace.
    /// </summary>
    private void TurnTurret()
    {
        // Save the last rotation we set it to so we can hold the turret there
        // even if the tank turns.
        lastRot = Vector3.RotateTowards(transform.forward, input.normalized, turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(lastRot);
    }

    /// <summary>
    /// Holds the turret at the last rotation inputted.
    /// </summary>
    private void HoldTurretRotation()
    {
        transform.rotation = Quaternion.LookRotation(
            Vector3.RotateTowards(transform.forward, lastRot, turnSpeed * Time.deltaTime, 0.0f)
        );
    }
}
