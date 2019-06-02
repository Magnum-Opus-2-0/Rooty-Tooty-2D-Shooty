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
        if (!healthBehave.isNotDead() && !died)
        {
            switch (this.gameObject.tag)
            {
                case "P1_Soldier":
                case "P2_Soldier":
                    DropResources(ResourceType.Plastic, 1);
                    break;
                case "P1_Teddy":
                case "P2_Teddy":
                    DropResources(ResourceType.Fluff, 1);
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
                    break;
            }

            resource.transform.parent = resourceFondler.transform;

            if (GetComponent<ExplodeBehavior>() != null)
                GetComponent<ExplodeBehavior>().AddPiece(resource);
            else
                Debug.LogError(this.name + " doesn't have an " +
                    "ExplodeBehavior script attachedbut should");
        }

        this.GetComponent<ExplodeBehavior>().Explode();


    }
}
