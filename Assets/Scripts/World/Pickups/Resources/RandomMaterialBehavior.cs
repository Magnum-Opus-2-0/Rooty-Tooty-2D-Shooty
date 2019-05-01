using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomMaterialBehavior : MonoBehaviour
{
    protected Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        Randomize();
    }

    public abstract void Randomize();
}
