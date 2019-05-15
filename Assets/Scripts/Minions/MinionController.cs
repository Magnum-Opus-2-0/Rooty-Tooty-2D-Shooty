using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    private HealthBehavior hb;

    // Start is called before the first frame update
    void Start()
    {
        hb = GetComponent<HealthBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hb.isNotDead())
        {
            Die();
        }

        DebugMovement();
    }

    /// <summary>
    /// Performs any necessary routines before murdering this minion.
    /// </summary>
    public void Die()
    {
        gameObject.SetActive(false);
    }

    private void DebugMovement()
    {
        transform.Translate(0, 0, 5.0f * Time.deltaTime);
    }
}
