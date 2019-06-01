using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePositionController : MonoBehaviour
{
    private ParticleSystem ps;
    private Vector3 holdPosition;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isPlaying)
        {
            transform.position = holdPosition;
        }
        else
        {
            holdPosition = transform.parent.position;
        }
    }
}
