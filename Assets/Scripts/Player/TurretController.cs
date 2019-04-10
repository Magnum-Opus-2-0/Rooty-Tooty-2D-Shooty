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

    // Start is called before the first frame update
    void Start()
    {
        input = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        TurnTurret();
    }

    private void UpdateInput()
    {
        input.Set(Input.GetAxis(xAim), 0f, Input.GetAxis(zAim));
    }

    /// <summary>
    /// Turns the tank in the direction given by the input vector relative to
    /// the camSpace.
    /// </summary>
    private void TurnTurret()
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
