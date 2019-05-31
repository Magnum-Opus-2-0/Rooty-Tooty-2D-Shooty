using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteResources : MonoBehaviour
{
    private TankController tc;

    // Start is called before the first frame update
    void Start()
    {
        tc = GetComponent<TankController>();
        tc.Plastic = tc.Fluff = int.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            tc.Plastic = tc.Fluff = int.MaxValue;
        }
    }
}
