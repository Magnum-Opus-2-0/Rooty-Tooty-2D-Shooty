using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{

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
    /// The axis this tank will use to aim left and right. Defaults to player
    /// one.
    /// </summary>
    public string xAim = "x-aim-1";
    /// <summary>
    /// The axis this tank will use to aim up and down. Defaults to player one.
    /// </summary>
    public string zAim = "z-aim-1";

    //public float speed = 10;

    private Vector3 input;
    private float inputSpeed;
    private float inputAngle;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        input = new Vector3();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateInput();
    }

    private void FixedUpdate()
    {
        MoveTank();
    }

    /// <summary>
    /// Updates the <see cref="input"/> vector.
    /// </summary>
    private void UpdateInput()
    {
        input.Set(Input.GetAxis("Drive-x"), 0, -1 * Input.GetAxis("Drive-z"));
        inputSpeed = Mathf.Max(input.x, input.z);
        inputAngle = Vector3.Angle(Vector3.forward, input);
    }    

    private void MoveTank()
    {
        rb.AddForce(input, ForceMode.Impulse);
    }
}
