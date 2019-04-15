using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteerMode
{
    /// <summary>
    /// The Tank turns to point toward the angle determined by the left analog
    /// stick.
    /// </summary>
    Point,
    /// <summary>
    /// The Tank turns left or right depending on the input from the left analog
    /// stick.
    /// </summary>
    Turn
}
