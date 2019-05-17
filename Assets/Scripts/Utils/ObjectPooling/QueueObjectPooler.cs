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
    /// <returns>A reference to the requested object.</returns>
    /// <param name="at">At.</param>
    /// <param name="dir">Dir.</param>
    public override T Request(Vector3 at, Quaternion dir)
    {
        T ret;
        if (GetNumInstantiated() < maxAllowed)
        {
            GameObject temp = Object.Instantiate(requestPrefab, at, dir, fondler) as GameObject;
            temp.SetActive(true);
            ret = temp.GetComponent<T>();
            objects.Add(ret);
        }
        else
        {
            ret = Recycle(at, dir);
        }

        return ret;
    }

    /// <summary>
    /// Convenience method that enables the requesting of an object with a <see cref="Transform"/>.
    /// Equivalent to <c>Request(<paramref name="transform"/>.position, <paramref name="transform"/>.rotation)</c>
    /// </summary>
    /// <returns>A reference to the requested object.</returns>
    /// <param name="transform">A Transform representing the position and rotation of the request.</param>
    public override T Request(Transform transform)
    {
        return Request(transform.position, transform.rotation);
    }

    /// <summary>
    /// Recycle the next object in the queue by calling its recycle method,
    /// setting its position and rotation, and setting it active.
    /// </summary>
    /// <returns>The recycle.</returns>
    /// <param name="at">At.</param>
    /// <param name="dir">Dir.</param>
    protected override T Recycle(Vector3 at, Quaternion dir)
    {
        T t = GetNext();
        t.Recycle();
        t.transform.SetPositionAndRotation(at, dir);
        t.gameObject.SetActive(true);
        return t;
    }

    private T GetNext()
    {
        T t = objects[nextObject++ % maxAllowed];
        return t;
    }
}
