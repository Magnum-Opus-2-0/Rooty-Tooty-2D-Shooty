using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField]
    private int fluffCost = 0;
    public int FluffCost
    {
        get
        {
            return FluffCost;
        }
    }

    [SerializeField]
    private int plasticCost = 0;
    public int PlasticCost
    {
        get
        {
            return plasticCost;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
