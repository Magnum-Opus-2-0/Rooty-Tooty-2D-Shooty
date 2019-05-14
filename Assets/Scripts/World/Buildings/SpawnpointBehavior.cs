using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointBehavior : MonoBehaviour
{
    private bool open;
    public bool IsOpen
    {
        get
        {
            return open;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        open = false;
    }

    private void OnTriggerStay(Collider other)
    {   
        open = false;
    }

    private void OnTriggerExit(Collider other)
    {
        open = true;
    }
}
