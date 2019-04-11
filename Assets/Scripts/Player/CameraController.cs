using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    /// <summary>
    /// The Transform of the GameObject that the Camera will follow.
    /// </summary>
    public Transform target;
    /// <summary>
    /// The coefficient of smoothing. A higher value equates to a faster follow.
    /// </summary>
    public float smoothing = 5f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position + Vector3.up*14;
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // We want to interpolate between the camera's current position and
        // where the camera should be
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothing * Time.deltaTime);
    }
}
