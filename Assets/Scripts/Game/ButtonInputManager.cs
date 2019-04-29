using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInputManager : MonoBehaviour
{

    public string submitButton;
    public StandaloneInputModule sim;

    // Start is called before the first frame update
    void Start()
    {
        // Determine which OS the game is being run on, and set controller
        // mappings accordingly.
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                submitButton = "submit-mac";
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                submitButton = "submit-win";
                break;

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                break;
        }

        sim.submitButton = submitButton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
