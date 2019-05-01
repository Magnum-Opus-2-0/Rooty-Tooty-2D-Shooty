using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightRingController : MonoBehaviour
{
    /// <summary>
    /// The length of time a flash should last. Default value is 1.0f so
    /// that <see cref="flashSpeed"/> can scale.
    /// </summary>
    public const float FLASH_LEN = 1.0f;

    /// <summary>
    /// Determines how quickly the highlight flashes.
    /// </summary>
    public float flashSpeed = 1f;

    /// <summary>
    /// The <see cref="Material"/> of the highlight when it is completely on.
    /// </summary>
    public Material highOn;
    /// <summary>
    /// The <see cref="Material"/> of the highlight when it is completely off.
    /// </summary>
    public Material highOff;

    /// <summary>
    /// The renderer of this highlight object. It is left ambiguous so that we
    /// can use a <see cref="MeshRenderer"/>, <see cref="LineRenderer"/>, etc.
    /// </summary>
    private Renderer highlight;

    /// <summary>
    /// The on flag. If the value is <c>true</c> the highlight interpolating
    /// from off to on and vice versa.
    /// </summary>
    private bool on;
    private float curTime;

    private bool once;

    void Awake()
    {
        highlight = GetComponent<Renderer>();

        on = false;
        curTime = 0.0f;

        once = true;
    }

    // Update is called once per frame
    void Update()
    {
        FlashHighlight();
    }

    void FlashHighlight()
    {
        if (on)
        {
            highlight.material.Lerp(highOff, highOn, curTime * flashSpeed);
            ResetCurTime();

            if (once)
            {
                once = false;
            }
        }
        else
        {
            highlight.material.Lerp(highOn, highOff, curTime * flashSpeed);
            ResetCurTime();
        }
        curTime += Time.deltaTime;
    }

    /// <summary>
    /// Checks to see if the current time has reached the <see cref="FLASH_LEN"/>.
    /// If so, reset the current time and flip the <see cref="on"/> flag.
    /// </summary>
    void ResetCurTime()
    {
        if (curTime * flashSpeed >= FLASH_LEN)
        {
            curTime = 0.0f;
            on = !on;
        }
    }
}
