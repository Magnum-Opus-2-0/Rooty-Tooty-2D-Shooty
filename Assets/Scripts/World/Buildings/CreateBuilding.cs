using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject p1_prefab;
    public GameObject p2_prefab;


    public GameObject[] parts;
    public bool isP1Obj;
    public TankController tc;
    private bool isValid;
    private bool hasEnough;
    public int fluffCost;
    public int plasticCost;

    void Start(){
        isValid = true;
        hasEnough = false;    
    }
    // Update is called once per frame
    void Update()
    {
        hasEnough = checkResources();
        for (int i = 0; i < parts.Length; i++){
            Color temp;
            
            if(isValid && hasEnough){
                temp = new Color(185.0f, 185.0f, 185.0f, .40f); 
                parts[i].GetComponent<Renderer>().material.color = temp;
            }else
            {
                temp = new Color(255.0f, 0.0f, 0.0f, .40f); 
                parts[i].GetComponent<Renderer>().material.color = temp;
            }
        }
    }

    public void BuildBuilding(){

        GameObject temp;
        if(isValid && hasEnough){
            if(isP1Obj){
                temp = Instantiate(p1_prefab, this.transform.position, Quaternion.identity);
            }
            else{
                temp = Instantiate(p2_prefab, this.transform.position, Quaternion.identity);
            }
            tc.Plastic -= plasticCost;
            tc.Fluff -= fluffCost;
        }
    }

    void OnTriggerEnter(Collider other){
        isValid = false;
    }


    void OnTriggerStay(Collider other){
       
        isValid = false;
                
    }    

    void OnTriggerExit(Collider other){
       
        isValid = true;
                
        Debug.Log(other.tag);
    }

    bool checkResources()
    {
        bool result = false;
        if(tc.Fluff - fluffCost >= 0 && tc.Plastic - plasticCost >= 0)
        {
            result = true; 
        }
        return result;
    }
}
