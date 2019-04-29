using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistController : MonoBehaviour
{

    public float renderDistance = 50f;
    public float startWidth = 0.075f;
    public float endWidth = 0.0f;
    public string aimToggle = "aim-toggle-1-mac";

    private LineRenderer lr;

    private RaycastHit aimHit;

    // Start is called before the first frame update
    void Start()
    {
        // Determine which OS the game is being run on, and set controller
        // mappings accordingly.
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                aimToggle = (tag == "Player_1" ? "aim-toggle-1-mac" : "aim-toggle-2-mac");
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                aimToggle = (tag == "Player_1" ? "aim-toggle-1-win" : "aim-toggle-2-win");
                break;

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                break;
        }

        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        if (lr.enabled)
        {
            UpdateLineRenderer();
        }
    }

    /// <summary>
    /// Updates the aim assist toggle input. Every time the button is pushed,
    /// the <see cref="aimToggle"/> is enabled or disabled depending on its
    /// previous state.
    /// </summary>
    private void UpdateInput()
    {
        if (Input.GetButtonDown(aimToggle))
        {
            lr.enabled = !lr.enabled;
        }
    }

    /// <summary>
    /// Updates the positions of beginning and end points of the <see cref="LineRenderer"/>.
    /// </summary>
    private void UpdateLineRenderer()
    {
        lr.SetPosition(0, transform.localPosition);
        if (CastAimRay())
        {
            lr.SetPosition(1, Vector3.forward * aimHit.distance);
            lr.endWidth = Mathf.Lerp(startWidth, endWidth, (aimHit.distance / renderDistance));
        }
        else
        {
            lr.SetPosition(1, Vector3.forward * renderDistance);
            lr.endWidth = endWidth;
        }
    }

    private bool CastAimRay()
    {
        Physics.Raycast(transform.position, transform.forward, out aimHit, renderDistance);
        return aimHit.transform != null;
    }

}
