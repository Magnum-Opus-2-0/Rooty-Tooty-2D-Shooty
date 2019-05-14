using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        TankController tc;
        tc = other.GetComponent<TankController>();
        tc.bmc.buildHolo.GetComponent<CreateBuilding>().IsValid = true;
        
        Destroy(this.gameObject);
    }
}
