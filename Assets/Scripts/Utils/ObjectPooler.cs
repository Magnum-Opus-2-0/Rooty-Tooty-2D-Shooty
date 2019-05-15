using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPooler<T> where T : MonoBehaviour
{
    /// <summary>
    /// The prefab of the GameObject that is will be requested.
    /// </summary>
    protected GameObject requestPrefab;
    public GameObject RequestPrefab
    {
        get
        {
            return requestPrefab;
        }
        set
        {
            requestPrefab = value;
        }
    }
    /// <summary>
    /// The Transform of the GameObject that will hold all requested GameObjects
    /// in the Hierarchy.
    /// </summary>
    protected Transform fondler;
    public Transform Fondler
    {
        get
        {
            return fondler;
        }
        set
        {
            fondler = value;
        }
    }

    /// <summary>
    /// The maximum number of GameObjects allowed to be instantiated.
    /// </summary>
    protected int maxAllowed;
    public int MaxAllowed
    {
        get
        {
            return maxAllowed;
        }

        set
        {
            maxAllowed = value;
        }
    }

    protected List<T> objects;

    protected ObjectPooler(GameObject prefab, Transform parent, int max)
    {
        objects = new List<T>(maxAllowed);
        requestPrefab = prefab;
        fondler = parent;
        maxAllowed = max;
    }

    abstract public bool Request(Vector3 at, Quaternion dir);

    abstract public bool Request(Transform transform);

    abstract protected void Recycle(Vector3 at, Quaternion dir);

    abstract public bool CanRequest();

    /// <summary>
    /// Gets the number of objects that are active in the Hierarchy.
    /// </summary>
    /// <returns>The number of active objects.</returns>
    public int GetNumActive()
    {
        int total = 0;
        foreach (T t in objects)
        {
            if (t.gameObject.activeInHierarchy)
            {
                total++;
            }
        }

        return total;
    }

    /// <summary>
    /// Convenience method that gets the number of objects that have been
    /// instantiated regardless of whether they are active in the Hierarchy.
    /// Equivalent to <c>objects.Count</c>
    /// </summary>
    /// <returns>The number of instantiated objects.</returns>
    public int GetNumInstantiated()
    {
        return objects.Count;
    }

    /// <summary>
    /// Gets the first object that is not active in the Hierarchy.
    /// </summary>
    /// <returns>The a GameObject reference to the disabled object, or <c>null</c>
    /// if all Minions are active.</returns>
    protected T GetFirstNotActive()
    {
        foreach (T t in objects)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                return t;
            }
        }

        return null;
    }
}
