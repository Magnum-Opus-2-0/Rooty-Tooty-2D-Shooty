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
    private bool isValid;

    void Start(){
        isValid = true;
      
    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < parts.Length; i++){
            Color temp;
            
            if(isValid){
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
        if(isValid){
            if(isP1Obj){
                temp = Instantiate(p1_prefab, this.transform.position, Quaternion.identity);
            }
            else{
                temp = Instantiate(p2_prefab, this.transform.position, Quaternion.identity);
            }   
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


}
