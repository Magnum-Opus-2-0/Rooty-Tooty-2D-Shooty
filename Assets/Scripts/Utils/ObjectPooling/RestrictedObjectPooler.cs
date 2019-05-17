using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictedObjectPooler<T> : ObjectPooler<T> where T : MonoBehaviour, IRecyclable
{

    public RestrictedObjectPooler(GameObject prefab, Transform parent, int max) 
        : base(prefab, parent, max)
    {}

    /// <summary>
    /// Requests an object from the pool. If one is available an active
    /// clone of the <see cref="base.requestPrefab"/> is placed there.
    /// </summary>
    /// <returns>A reference to the object if one is available, <c>null</c> otherwise.</returns>
    /// <param name="at">The position at which to place the object.</param>
    /// <param name="dir">The direction at which to point the object.</param>
    override public T Request(Vector3 at, Quaternion dir)
    {
        T ret = null;
        if (GetNumInstantiated() < maxAllowed)
        {
            GameObject temp = Object.Instantiate(requestPrefab, at, dir, fondler) as GameObject;
            temp.SetActive(true);
            ret = temp.GetComponent<T>();
            objects.Add(ret);
        }
        else if (CanRequest())
        {
            ret = Recycle(at, dir);
        }

        return ret;
    }

    /// <summary>
    /// Convenience method that enables the requesting of an object with a <see cref="Transform"/>.
    /// Equivalent to <c>Request(<paramref name="transform"/>.position, <paramref name="transform"/>.rotation)</c>
    /// </summary>
    /// <returns>A reference to the object if one is available, <c>null</c> otherwise.</returns>
    /// <param name="transform">A Transform representing the position and rotation of the request.</param>
    public override T Request(Transform transform)
    {
        return Request(transform.position, transform.rotation);
    }

    /// <summary>
    /// Recycles the first object that is not active in the Hierarchy. That is,
    /// this function sets the objects's position and rotation, calls its Recycle method
    /// and then sets it active.
    /// </summary>
    /// <returns>A reference to the object if one is available, <c>null</c> otherwise.</returns>
    /// <param name="at">The position at which to set the object.</param>
    /// <param name="dir">The rotation in which to point the object.</param>
    protected override T Recycle(Vector3 at, Quaternion dir)
    {
        T t = GetFirstNotActive();
        if (t)
        {
            t.Recycle();
            t.transform.SetPositionAndRotation(at, dir);
            t.gameObject.SetActive(true);
        }
        return t;
    }

    /// <summary>
    /// Convenience method that checks whether the number of objects active in
    /// the Hierarchy is less than the maximum allowed. 
    /// Equivalent to <c>GetNumActive() < maxAllowed</c>
    /// </summary>
    /// <returns><c>true</c>, if the request will be accepted, <c>false</c>
    /// otherwise.</returns>
    public override bool CanRequest()
    {
        return GetNumActive() < maxAllowed;
    }
}
