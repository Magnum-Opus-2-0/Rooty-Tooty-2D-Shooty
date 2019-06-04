using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public HealthBehavior healthBehave;

    public GameObject[] fluffs = new GameObject[2];
    public GameObject[] plastics = new GameObject[2];
    private GameObject resourceFondler;

    private int randomNum;
    private bool died;

    // Start is called before the first frame update
    void Start()
    {
        died = false;
        healthBehave = GetComponent<HealthBehavior>();
        resourceFondler = GameObject.Find("Resource Fondler");
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBehave == null)
            Debug.LogError("healthBehave is null, object with ResourceDrop is " + this.gameObject.name);

        if (!healthBehave.isNotDead() && !died)
        {
            switch (this.gameObject.tag)
            {
                case "P1_Soldier":
                case "P2_Soldier":
                    DropResources(ResourceType.Plastic, 1);
                    break;
                case "P1_Spawner":
                case "P2_Spawner":
                    DropResources(ResourceType.Plastic, 2);
                    break;
                case "P1_Teddy":
                case "P2_Teddy":
                    DropResources(ResourceType.Fluff, 1);
                    break;
                case "P1_Turret":
                case "P2_Turret":
                    DropResources(ResourceType.Plastic, 2);
                    break;
                case "P1_Healer":
                case "P2_Healer":
                    DropResources(ResourceType.Plastic, 2);
                    break;
                default:
                    Debug.LogError("Object " + this.name +
                        " is not supposed to drop resources");
                    break;
            }

            died = true;
        }
    }

    private void OnDisable()
    {

    }

    private void DropResources(ResourceType resourceType, int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            randomNum = Random.Range(0, 1);
            GameObject resource = null;

            switch (resourceType)
            {
                case ResourceType.Fluff:
                    resource = Instantiate(fluffs[randomNum],
                        this.transform.position,
                        Quaternion.identity);
                    break;
                case ResourceType.Plastic:
                    resource = Instantiate(plastics[randomNum],
                        this.transform.position,
                        Quaternion.identity);
                    Debug.Log("Instantiated " + resource.name);
                    break;
            }

            if (resourceFondler != null)
            {
                //Debug.Log("putting" + resource.name + " into " + resourceFondler.name);
                resource.transform.parent = resourceFondler.transform;
            }
            else
                Debug.LogError("Resource Fondler not found in hierarchy");

            if (GetComponent<ExplodeBehavior>() != null && resource != null)
            {
                GetComponent<ExplodeBehavior>().AddPiece(resource);
                //Debug.Log("adding" + resource.name + " to explosion set");
            }
            else
                Debug.LogError(this.name + " doesn't have an " + 
                    "ExplodeBehavior script attached but should");
        }

        this.GetComponent<ExplodeBehavior>().Explode();


    }
}
