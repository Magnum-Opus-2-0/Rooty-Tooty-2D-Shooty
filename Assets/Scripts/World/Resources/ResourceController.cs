using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.None;
    public int minAmount;
    public int maxAmount;

    private int amount;
    public int Amount
    {
        get
        {
            return amount;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1); //Max is exclusive
        Debug.Log(Amount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This function is called when any <see cref="Collider"/> enters this
    /// GameObject's trigger.
    /// <para>If that Collider is attached to a Tank the resource is added and
    /// this GameObject is destroyed.</para>
    /// </summary>
    /// <param name="other">Other.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            AddResource(other.GetComponent<TankController>());
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Determines whether the given collider is attached to a GameObject with
    /// a player tag. That is "Player_1" or "Player_2".
    /// </summary>
    /// <returns><c>true</c>, if GameObject was a player, <c>false</c> otherwise.</returns>
    /// <param name="c">C.</param>
    public static bool IsPlayer(Collider c)
    {
        return c.tag == "Player_1" || c.tag == "Player_2";
    }

    /// <summary>
    /// Adds the resource to the <see cref="TankController"/> depending on the
    /// <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="tc">Tc.</param>
    private void AddResource(TankController tc)
    {
        switch (resourceType)
        {
            case ResourceType.Fluff:
                tc.Fluff += amount;
                break;

            case ResourceType.Plastic:
                tc.Plastic += amount;
                break;

            case ResourceType.None:
                Debug.LogError("ResourceType has not been assigned.");
                break;
        }
    }
}
