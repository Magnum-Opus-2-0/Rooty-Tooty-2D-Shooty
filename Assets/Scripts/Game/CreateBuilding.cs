using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject p1_prefab;
    public GameObject p2_prefab;

    public bool isP1Obj;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildBuilding(){

        GameObject temp;
        if(isP1Obj){
            temp = Instantiate(p1_prefab, this.transform.position, Quaternion.identity);
        }
        else{
            temp = Instantiate(p2_prefab, this.transform.position, Quaternion.identity);
        }   
    }
}
