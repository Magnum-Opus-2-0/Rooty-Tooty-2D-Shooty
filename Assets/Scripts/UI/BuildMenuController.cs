using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuController : MonoBehaviour
{
    public Vector3 activePos; //The position to be set once menu is active
    public Vector3 inactivePos; //the position to be set once menu is inactive
    public float timeStep;
    private Vector3 currPos; // current position
    private Vector3 endPos; //Position to travel to
    private bool isActive; //Controls where to set endPos
    private bool isDone;
    public float actionTime; //Time to complete travel
    
    void Start(){
        isActive = false;
        setIsDone(true);
    }
    
    ///<summary>
    /// Calls the subrputine necessary for the menu to toggle
    ///</summary> 
    public void ToggleMenu(){
        Debug.Log("Called toggleMenu");
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
}
