﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuController : MonoBehaviour
{
    public Vector3 activePos; //The position to be set once menu is active
    public Vector3 inactivePos; //the position to be set once menu is inactive
    public float timeStep;
    private Vector3 currPos; // current position
    private Vector3 endPos; //Position to travel to
    private bool isActive; //Controls where to set endPos
    private bool isDone;
    private bool canUse; //Make sure you can't build anything until animation completes
    public float actionTime; //Time to complete travel
    
    public float iconTracker;
    public Image[] buildIcons;

    void Start(){
        isActive = false;
        setCanUse(false);
        setIsDone(true);
    }

    void Update(){

        for(int i = 0; i < buildIcons.Length; i++){
            Color temp = buildIcons[i].color;
//            Debug.Log(temp);
            if(i == iconTracker){
                
                temp = new Color(255.0f, 255.0f, 255.0f, 1.0f); 
                buildIcons[i].color = temp;
            }
            else{
                temp = new Color(150.0f, 150.0f, 150.0f, 0.5f);
                buildIcons[i].color = temp;
            }
        }
    }
    
    ///<summary>
    /// Calls the subrputine necessary for the menu to toggle
    ///</summary> 
    public void ToggleMenu(){
        setIsDone(false);
        StartCoroutine("SwapBuildMode");
    }

    ///<summary>
    /// Smoothly moves build menu from the active to the inactive state (and vice-versa) over
    /// a period set by actionTime. Can adjust the speed by altering actionTime and timeStep
    /// through the Unity inspector.
    ///</summary>
    IEnumerator SwapBuildMode(){
        
        RectTransform rt = this.GetComponent<RectTransform>();
        float fracDone = 1.0f;
        currPos = rt.anchoredPosition;
        if(!isActive){
            endPos = activePos;
            isActive = true;
            }
         else{
            endPos = inactivePos;
            isActive = false;
        }

        setCanUse(!canUse);
        for(float t = 0.0f; t < actionTime; t += timeStep){           
            fracDone = (actionTime - t)/actionTime;
            rt.anchoredPosition = Vector3.Lerp(endPos, currPos, fracDone);
           
            yield return new WaitForSeconds(timeStep);
        }
        setIsDone(true);
    }
    
    ///<summary>
    /// Getter and setter for isDone boolean
    ///</summary>
    void setIsDone(bool val){
        isDone = val;
    }

    public bool getIsDone(){
        return isDone;
    }

    ///<summary>
    /// Getter and setter for canUse boolean
    ///</summary>
    void setCanUse(bool val){
        canUse = val;
    }

    public bool getCanUse(){
        return canUse;
    }

    public void ModifyIconTracker(int val){
        iconTracker += val;
        if(iconTracker < 0){
            iconTracker = 3;
        }
        Debug.Log(iconTracker);
        if(iconTracker > 3){
            iconTracker = 0;
        }
    }
}
