using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObjectPooler : MonoBehaviour
{
    public GameObject minionPrefab;

    public int maxAllowedMinions;

    private List<MinionController> minions;

    // Start is called before the first frame update
    void Start()
    {
        // We know how many we will ultimately want (unless things change mid-game?)
        minions = new List<MinionController>(maxAllowedMinions);
    }

    /// <summary>
    /// Requests a minion from the object pool. If one is available an active
    /// Minion GameObject there is placed there.
    /// </summary>
    /// <returns><c>true</c>, if a minion is available, <c>false</c> otherwise.</returns>
    /// <param name="at">The position at which to place the Minion.</param>
    /// <param name="dir">The direction at which to point the Minion.</param>
    public bool Request(Vector3 at, Quaternion dir)
    {
        if (GetNumInstantiated() < maxAllowedMinions)
        {
            GameObject temp = Instantiate(minionPrefab, at, dir, transform) as GameObject;
            minions.Add(temp.GetComponent<MinionController>());
            return true;
        } 

        if (CanRequest())
        {
            Recycle(at, dir);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Convenience method that enables the requesting of a Minion with a <see cref="Transform"/>.
    /// </summary>
    /// <returns><c>true</c>, if a minion is available, <c>false</c> otherwise.</returns>
    /// <param name="transform">A Transform representing the position and rotation of the request.</param>
    public bool Request(Transform transform)
    {
        return Request(transform.position, transform.rotation);
    }

    /// <summary>
    /// Recycles the first minion that is not active in the Hierarchy. That is,
    /// this function sets the Minion's position and rotation and then sets it active.
    /// </summary>
    /// <param name="at">The position at which to set the Minion.</param>
    /// <param name="dir">The rotation in which to point the Minion.</param>
    private void Recycle(Vector3 at, Quaternion dir)
    {
        GameObject minion = GetFirstNotActive();
        minion.transform.SetPositionAndRotation(at, dir);
        minion.SetActive(true);
    }

    /// <summary>
    /// Convenience method that checks whether the number of Minions active in
    /// the Hierarchy is less than the maximum allowed.
    /// </summary>
    /// <returns><c>true</c>, if the request will be accepted, <c>false</c>
    /// otherwise.</returns>
    public bool CanRequest()
    {
        return GetNumActive() < maxAllowedMinions;
    }

    /// <summary>
    /// Gets the number of Minions whose GameObjects are active in the Hierarchy.
    /// </summary>
    /// <returns>The number of active Minions.</returns>
    public int GetNumActive()
    {
        int total = 0;
        foreach (MinionController m in minions)
        {
            if (m.gameObject.activeInHierarchy)
            {
                total++;
            }
        }
        return total;
    }

    /// <summary>
    /// Convenience method that gets the number of Minions that have been
    /// instantiated regardless of whether they are active in the Hierarchy.
    /// </summary>
    /// <returns>The number of instantiated minions.</returns>
    public int GetNumInstantiated()
    {
        return minions.Count;
    }

    /// <summary>
    /// Gets the first Minion whose GameObject is not active in the Hierarchy.
    /// </summary>
    /// <returns>The a GameObject reference to the disabled Minion, or <c>null</c>
    /// if all Minions are active.</returns>
    private GameObject GetFirstNotActive()
    {
        foreach (MinionController m in minions)
        {
            if (!m.gameObject.activeInHierarchy)
            {
                return m.gameObject;
            }
        }

        return null;
    }
}
