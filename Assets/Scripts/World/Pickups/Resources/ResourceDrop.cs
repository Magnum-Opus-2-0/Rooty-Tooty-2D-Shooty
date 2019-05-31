using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public HealthBehavior hb;

    public GameObject[] fluffs = new GameObject[2];
    public GameObject[] plastics = new GameObject[2];
    private GameObject resourceFondler;

    private int randomNum;

    // Start is called before the first frame update
    void Start()
    {
        hb = GetComponent<HealthBehavior>();
        resourceFondler = GameObject.Find("Resource Fondler");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        if (!hb.isNotDead())
        {
            randomNum = Random.Range(0, 2);

            switch (this.gameObject.tag)
            {
                case "P1_Soldier":
                case "P2_Soldier":
                    dropResources(ResourceType.Plastic, 1, randomNum);
                    break;
                case "P1_Teddy":
                case "P2_Teddy":
                    dropResources(ResourceType.Fluff, 1, randomNum);
                    break;
                default:
                    Debug.LogError("Object " + this.name + 
                        " is not supposed to drop resources");
                    break;
            }
        }
    }

    private void dropResources(ResourceType resourceType, int howMany, int randomNum)
    {
        for (int i = 0; i < howMany; i++)
        {
            GameObject resource;

            switch (resourceType)
            {
                case ResourceType.Fluff:
                    resource = Instantiate(fluffs[randomNum], 
                        new Vector3(this.transform.position.x, 0, this.transform.position.z), 
                        Quaternion.identity);

                    resource.transform.parent = resourceFondler.transform;
                    break;
                case ResourceType.Plastic:
                    resource = Instantiate(plastics[randomNum],
                        new Vector3(this.transform.position.x, 0, this.transform.position.z),
                        Quaternion.identity);

                    resource.transform.parent = resourceFondler.transform;
                    break;
            }
        }
    }
}
