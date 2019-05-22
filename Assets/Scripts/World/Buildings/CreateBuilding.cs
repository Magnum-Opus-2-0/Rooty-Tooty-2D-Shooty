using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CreateBuilding : MonoBehaviour
{

    public const string PREFIX_P1 = "P1_";
    public const string PREFIX_P2 = "P2_";
    public const string TAG_SPAWNER = "Spawner";


    // Start is called before the first frame update
    public GameObject p1_prefab;
    public GameObject p2_prefab;

    /// <summary>
    /// A prefab for the minion fondler that will be instantiated for every minion
    /// spawner built.
    /// </summary>
    public GameObject minion_fondler_prefab;
    public static Transform master_minion_fondler;

    private static bool findFondlerOnce = true;



    public GameObject[] parts;
    public bool isP1Obj;
    public TankController tc;
    private bool isValid;
    private bool hasEnough;
    private Collider col;
    public int fluffCost;
    public int plasticCost;
    public Color validColor = new Color(2.0f, 2.0f, 2.0f, .10f);
    public Color invalidColor = new Color(255.0f, 0.0f, 0.0f, .10f);

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

        // This might be bad practice, but it saves us passing the fondler down
        // all the way through the instantiations of objects. Starting with the
        // SceneManager and going down to every instantiation of spawners.
        if (findFondlerOnce)
        {
            master_minion_fondler = FindMasterMinionFondler();
            findFondlerOnce = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!isValid && col == null){
            isValid = true;
        }
        hasEnough = checkResources();
        for (int i = 0; i < parts.Length; i++){
            //            Debug.Log(isValid);
            MeshRenderer temp = parts[i].GetComponent<MeshRenderer>();
            if (isValid && hasEnough){
                temp.materials[0].SetColor("_Color", validColor);
            }
            else
            {
                temp.materials[0].SetColor("_Color", invalidColor);
            }
        }
    }

    public void BuildBuilding(){

        GameObject temp;
        if(isValid && hasEnough){
            if(isP1Obj){
                temp = Instantiate(p1_prefab, this.transform.position, Quaternion.identity);
                if (temp.tag ==  PREFIX_P1 + TAG_SPAWNER)
                {
                    InstantiateMinionFondler(temp, "P1");
                }
            }
            else{
                temp = Instantiate(p2_prefab, this.transform.position, Quaternion.identity);
                if (temp.tag == PREFIX_P2 + TAG_SPAWNER)
                {
                    InstantiateMinionFondler(temp, "P2");
                }
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

    private void InstantiateMinionFondler(GameObject spawner, string player)
    {
        GameObject temp = Instantiate(minion_fondler_prefab, master_minion_fondler) as GameObject;
        spawner.GetComponent<MinionBuildingController>().MinionFondler = temp.transform;
    }

    /// <summary>
    /// Finds the master minion fondler GameObject. This only needs to be called
    /// one time to set the <see cref=""/> field so we have something
    /// to spawn minions into.
    /// </summary>
    /// <returns>The master minion fondler.</returns>
    private static Transform FindMasterMinionFondler()
    {
        GameObject mmf = GameObject.FindWithTag("Master Minion Fondler");
        Assert.IsTrue(mmf != null, 
            "Could not find Minion Fondler. Was it added to the Hierarchy?\n" +
            "If it was and you are still seeing this error,\n" +
            "make sure it is tagged with the \"Master Minion Fondler\"");

        Transform ret = mmf.transform;
        Assert.IsTrue(ret != null, "ok somehow, Master Minion Fondler doesn't have a Transform");
        return ret;
    }
}
