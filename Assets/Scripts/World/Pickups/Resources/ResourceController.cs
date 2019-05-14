﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : PickupController
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
    }

    /// <summary>
    /// This function is called when any <see cref="Collider"/> enters this
    /// GameObject's trigger.
    /// <para>If that Collider is attached to a Tank the resource is added and
    /// this GameObject is destroyed.</para>
    /// </summary>
    /// <param name="other">Other.</param>
    override protected void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            // The collider is on the body of the tank, so search the parent for
            // the TankController
            TankController tc = other.GetComponentInParent<TankController>();
            // If the TankController wasn't found, then we hit a turret component,
            // so search the Turret's parent.
            if (tc == null)
            {
                tc = other.transform.parent.GetComponentInParent<TankController>();
            }

            if (tc.State == TankStates.Alive && tc.CanGrabResource(resourceType))
            {
                AddResource(tc);
                BuildMenuController bmc = tc.bmc;
                GameObject buildHolo = bmc.buildHolo;
                CreateBuilding cb = buildHolo.GetComponent<CreateBuilding>();
                cb.OnTriggerExit(other);
                base.OnTriggerEnter(other);
            }
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
                tc.FluffCapped += amount;
                break;

            case ResourceType.Plastic:
                tc.PlasticCapped += amount;
                break;

            case ResourceType.None:
                Debug.LogError("ResourceType has not been assigned.");
                break;
        }
    }


}
