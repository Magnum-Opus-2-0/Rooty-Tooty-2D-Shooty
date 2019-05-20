using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    protected HealthBehavior health;
    [SerializeField]
    protected float timeToDeath = 3.5f;
    protected bool diedOnce;


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        health = GetComponent<HealthBehavior>();
        diedOnce = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!IsNotDead() && !diedOnce)
        {
            Destroy(this.gameObject, timeToDeath);
            diedOnce = true;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }

    /// <summary>
    /// Convenience function that returns the HealthBehavior's isNotDead value.
    /// </summary>
    /// <returns><c>true</c>, if the object's health is greater than 0, <c>false</c>
    /// otherwise.</returns>
    public bool IsNotDead()
    {
        return health.isNotDead();
    }
}
