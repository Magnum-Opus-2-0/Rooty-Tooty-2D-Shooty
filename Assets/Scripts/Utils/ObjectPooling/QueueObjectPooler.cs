using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueObjectPooler<T> : ObjectPooler<T> where T : MonoBehaviour, IRecyclable
{
    private int nextObject;
    public int NextObject
    {
        get
        {
            return nextObject;
        }

        set
        {
            nextObject = value;
        }
    }

    public QueueObjectPooler(GameObject prefab, Transform parent, int max)
        : base(prefab, parent, max)
    {
        nextObject = 0;
    }

    /// <summary>
    /// An object can always be requested from a QueueObjectPooler.
    /// </summary>
    /// <returns><c>true</c></returns>
    public override bool CanRequest()
    {
        return true;
    }

    /// <summary>
    /// Places the next object in the queue at the specified position and rotation and
    /// active in the Hierarchy. 
    /// </summary>
    /// <returns><c>true</c>.</returns>
    /// <param name="at">At.</param>
    /// <param name="dir">Dir.</param>
    public override bool Request(Vector3 at, Quaternion dir)
    {
        if (GetNumInstantiated() < maxAllowed)
        {
            GameObject temp = Object.Instantiate(requestPrefab, at, dir, fondler) as GameObject;
            objects.Add(temp.GetComponent<T>());
        }
        else
        {
            Recycle(at, dir);
        }

        return true;
    }

    public override bool Request(Transform transform)
    {
        return Request(transform.position, transform.rotation);
    }

    protected override void Recycle(Vector3 at, Quaternion dir)
    {
        T t = GetNext();
        t.Recycle();
        t.transform.SetPositionAndRotation(at, dir);
        t.gameObject.SetActive(true);
    }

    private T GetNext()
    {
        return objects[nextObject % maxAllowed];
    }
}
