using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    protected HealthBehavior health;
    [SerializeField]
    protected float timeToDeath = 3.5f;
    private bool diedOnce;


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
            StartCoroutine(WaitForDeath(timeToDeath));
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

    /// <summary>
    /// Coroutine that waits a specified amount of time before destroying the
    /// GameObject it is attached to.
    /// </summary>
    /// <param name="time">The amount of time to wait in seconds.</param>
    protected IEnumerator WaitForDeath(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
