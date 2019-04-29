﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSetup : MonoBehaviour
{
    
    //Objects needed
    public GameObject cam1;
    public GameObject cam2;
    public GameObject canvas1;
    public GameObject canvas2;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSplitScreen();
    }

    
    ///<summary>
    /// Instatiates the two cameras alongwith their screen space canvas.
    /// Also attaches necessary compoents for their scripts to function.
    ///</summary>    
    public void InitializeSplitScreen(){
        
        //Initialize objects
        GameObject p1 = GameObject.FindWithTag("Player1_obj");
        Debug.Log("Found p1. Transform: " + p1.transform);
        GameObject p2 = GameObject.FindWithTag("Player2_obj");
        Debug.Log("Found p2. Transform: " + p2.transform);

        cam1 = Instantiate(cam1, new Vector3(0,0,0), Quaternion.identity);
        cam2 = Instantiate(cam2, new Vector3(0,0,0), Quaternion.identity);
        Debug.Log("Instantiated cameras: " + cam1.transform);

        canvas1 = Instantiate(canvas1, new Vector3(0,0,0), Quaternion.identity);
        canvas2 = Instantiate(canvas2, new Vector3(0,0,0), Quaternion.identity);
        //Set camera to tank transforms
        CameraController cc1 = cam1.GetComponent<CameraController>();
        CameraController cc2 = cam2.GetComponent<CameraController>();
        cc1.SetTarget(p1.transform);
        cc2.SetTarget(p2.transform);

        //Set up canvas

        canvas1.GetComponent<Canvas>().worldCamera = cam1.GetComponent<Camera>();
        canvas2.GetComponent<Canvas>().worldCamera = cam2.GetComponent<Camera>();
        canvas1.GetComponent<Canvas>().planeDistance = 1;
        canvas2.GetComponent<Canvas>().planeDistance = 1;

        InitializeHealthBehavior(p1, canvas1);
        InitializeHealthBehavior(p2, canvas2);
    }

    void InitializeHealthBehavior(GameObject player, GameObject cav){
        HealthBehavior hb = player.GetComponent<HealthBehavior>();
        hb.health_icon = cav.transform.Find("Heart_back/Heart_health").gameObject.GetComponent<Image>();
        hb.respawn_background = cav.transform.Find("Respawn_back").gameObject.GetComponent<Image>();
        hb.health_val = cav.transform.Find("Heart_back/Health_Text").gameObject.GetComponent<Text>();
        hb.respawnText = cav.transform.Find("Respawn_back/Respawn_Text").gameObject.GetComponent<Text>();

        hb.health_icon.fillAmount = 1.0f;
        hb.health_val.text = HealthBehavior.MAX_HEALTH.ToString();
        hb.respawnText.gameObject.SetActive(false);
        hb.respawn_background.gameObject.SetActive(false);

    }
}
