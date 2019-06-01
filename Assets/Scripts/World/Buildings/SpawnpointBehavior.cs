using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointBehavior : MonoBehaviour
{

    private Collider col;

    private bool open;
    public bool IsOpen
    {
        get
        {
            return open;
        }
    }

    void Start()
    {
        open = true;
    }

    void Update()
    {
        if (!open && (col == null || !col.gameObject.activeInHierarchy))
        {
            open = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        open = false;
        //Debug.Log("Enter spawnpoint: " + other.name);
    }

    private void OnTriggerStay(Collider other)
    {   
        open = false;
        //Debug.Log("Stay spawnpoint: " + other.name);
        col = other;
    }

    private void OnTriggerExit(Collider other)
    {
        open = true;
        //Debug.Log("Exit spawnpoint: " + other.name);
    }

    #region DEBUG_METHODS
    public IEnumerator PrintIsOpen()
    {
        while (true)
        {
            Debug.Log("Spawnpoint Is Open: " + open);
            yield return new WaitForSeconds(.1f);
        }
    }
    #endregion
}
