using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

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
    public bool prefabController; //Used to set which player a created building belongs to. True = p1, false = p2
    public int iconTracker;
    public Image[] buildIcons;
    public GameObject[] buildGhosts; //A collection of hologram prefabs to instatiate from
    public GameObject buildHolo; //Used to assign the current hologram
    public GameObject turret; // used to get the direction the turret is facing.
    public TankController tankController; //Used to pass reference to appropriate TankController.cs to CreateBuilding.cs and to get the player's resource count
    public float buildDistance = 2.65f; // Used to set the distance of the hologram away from the turret

    private static bool findBoardControllerOnce = true;
    private static BoardController boardController;

    [SerializeField]
    private Text build_name;
    [SerializeField]
    private Text plastic_cost;
    [SerializeField]
    private Text fluff_cost;
    [SerializeField]
    private Image tool_tip;
    [SerializeField]
    private Text fluff_count;
    [SerializeField]
    private Text plastic_count;
    
    void Start(){
        isActive = false;
        setCanUse(false);
        setIsDone(true);
        SetBuildHolo();

        if (findBoardControllerOnce)
        {

            boardController = FindBoardController();
            findBoardControllerOnce = false;
        }

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

        plastic_count.text = tankController.Plastic.ToString();
        fluff_count.text = tankController.Fluff.ToString();
        DisplayBuildHolo();
        
    }
    
    ///<summary>
    /// Calls the subroutine necessary for the menu to toggle
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

    ///<summary>
    ///Takes in a value and adjusts what building the player is currently selecting.
    ///</summary>
    public void ModifyIconTracker(int val){
        iconTracker += val;
        if(iconTracker < 0){
            iconTracker = 3;
        }
        if(iconTracker > 3){
            iconTracker = 0;
        }

        SetBuildHolo();
    }

    /// <summary>
    /// changes out the build holgram for the purposes of displayBuildHolo and PlaceBuilding functions.
    /// </summary>
    void SetBuildHolo(){
        CreateBuilding cb;
        if(buildHolo != null){
            Destroy(buildHolo);
        }
        buildHolo = Instantiate(buildGhosts[iconTracker],  turret.transform.position + turret.transform.forward* 2.0f, Quaternion.identity);
        cb = buildHolo.gameObject.GetComponent<CreateBuilding>();
        cb.isP1Obj = prefabController;
        cb.tc = tankController;
        UpdateToolTip();
    }

    /// <summary>
    /// Adjusts the position of the buildHolo object and activates and deactivates object based on 
    /// whether or not the build menu is active.
    /// </summary>
    void DisplayBuildHolo(){

        if(getCanUse()){
            buildHolo.gameObject.SetActive(true);
            tool_tip.gameObject.SetActive(true);
        }
        else{
            buildHolo.gameObject.SetActive(false);
            tool_tip.gameObject.SetActive(false);
        }

        buildHolo.transform.position = turret.transform.position + turret.transform.forward*buildDistance - (turret.transform.up * turret.transform.position.y); 
        buildHolo.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Calls the BuildBuilding function on the createBuilding script of the buildHolo object. This function
    /// is called from the TankController script.
    /// </summary>
    public void PlaceBuilding(){
        CreateBuilding cb = buildHolo.gameObject.GetComponent<CreateBuilding>();
        cb.BuildBuilding();
        boardController.bakeNavMesh();
    }

    ///<summary>
    /// Updates the tool tip to display the correct amount of information on screen.
    ///</summary>
    void UpdateToolTip(){
        CreateBuilding cb = buildHolo.gameObject.GetComponent<CreateBuilding>();
        switch(iconTracker){
            case 0:
                build_name.text = "Army Men Bucket";
                break;
            case 1:
                build_name.text = "Teddy Bear Chest";
                break;
            case 2:
                build_name.text = "Guard Turret";
                break;
            case 3:
                build_name.text = "Repair Station";
                break;
            default:
                break;
            
        }

        plastic_cost.text = cb.plasticCost.ToString() + " Plastic" ;
        fluff_cost.text = cb.fluffCost.ToString() + " Fluff";

    }

    private static BoardController FindBoardController()
    {
        BoardController bc = GameObject.FindWithTag("Board").GetComponent<BoardController>();
        Assert.IsTrue(bc != null, "Board controller not found");

        return bc;
    }

}
