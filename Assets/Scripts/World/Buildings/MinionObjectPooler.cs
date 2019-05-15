using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObjectPooler : MonoBehaviour
{
    public GameObject minionPrefab;

    public int maxAllowedMinions;
    private int numInstantiated;

    private List<MinionController> minions;

    // Start is called before the first frame update
    void Start()
    {
        numInstantiated = 0;
        // We know how many we will ultimately want (unless things change mid-game?)
        minions = new List<MinionController>(maxAllowedMinions);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Requests a minion from the object pool. If one is available an active
    /// Minion GameObject there is placed there.
    /// </summary>
    /// <returns><c>true</c>, if a minion is available, <c>false</c> otherwise.</returns>
    /// <param name="at">The position at which to place the Minion.</param>
    /// <param name="dir">The direction at which to point the Minion.</param>
    public bool RequestMinion(Vector3 at, Quaternion dir)
    {
        if (numInstantiated < 0)
        {
            Instantiate(minionPrefab, at, dir, transform);
            return true;
        } 

        if (GetNumActive() < maxAllowedMinions)
        {
            RecycleMinion(GetFirstNotActive(), at, dir);
            return true;
        }

        return false;
    }

    private void RecycleMinion(GameObject minion, Vector3 at, Quaternion dir)
    {
        minion.transform.SetPositionAndRotation(at, dir);
        minion.SetActive(true);
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
