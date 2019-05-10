using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildBuilding(){

        GameObject temp;
        temp = Instantiate(prefab, this.transform.position, Quaternion.identity);
    }
}
