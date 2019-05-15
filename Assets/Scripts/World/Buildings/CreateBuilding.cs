﻿using System.Collections;
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
    private Collider col;
    public int fluffCost;
    public int plasticCost;

    public bool IsValid{
        get
        {
            return isValid;
        }

        set
        {
            isValid = value;
        }
    }

    void Start(){
        isValid = true;
        hasEnough = false;    
    }
    // Update is called once per frame
    void Update()
    {
        if(!isValid && col == null){
            isValid = true;
        }
        hasEnough = checkResources();
        for (int i = 0; i < parts.Length; i++){
            Color temp;
//            Debug.Log(isValid);
            if(isValid && hasEnough){
                temp = new Color(165.0f, 165.0f, 165.0f, .10f); 
                parts[i].GetComponent<Renderer>().materials[0].color = temp;
            }else
            {
                temp = new Color(255.0f, 0.0f, 0.0f, .10f); 
                parts[i].GetComponent<Renderer>().materials[0].color = temp;
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
        col = other;
        //Debug.Log("Called onStay");
                
    }    

    public void OnTriggerExit(Collider other){
       
        isValid = true;
                
       // Debug.Log("Called onExit");
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
