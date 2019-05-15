using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour, IRecyclable
{
    public int maxHealth;
    private HealthBehavior health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.isNotDead())
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
        Debug.Log("Minion dying");
        gameObject.SetActive(false);
    }

    private void DebugMovement()
    {
        transform.Translate(0, 0, 5.0f * Time.deltaTime);
    }

    public void Recycle()
    {
        health.setHealth(maxHealth);
    }
}
