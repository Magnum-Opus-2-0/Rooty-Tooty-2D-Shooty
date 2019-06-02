using System;   // needed for Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ExplodeBehavior : MonoBehaviour {

    public ExplodeEffectController efc;

    public bool doExplosionDemo;

    /// <summary>
    /// The constant number of pieces. Equal to 3. Use <see cref="pieces"/>.Length
    /// instead.
    /// </summary>
    //[Obsolete("NUM_OF_PIECES is a constant. Use pieces.Length instead.")]
    //public const int NUM_OF_PIECES = 3;

    private bool exploded;  // used to track state

    // Variables and references which are configured via Unity Editor
    public float forceOfExplosion;
    public float radiusOfExplosion;

    private Vector3 originalParentPosition;
    private Quaternion originalParentRotation;

    /// <summary>
    /// Pieces of the tank (or whatever!) which need to explode apart.
    /// </summary>
    public List<GameObject> pieces;

    /// <summary>
    /// The positions that each piece should be restored to.
    /// This can be either the tank's last unexploded position,
    /// or its spawn point.
    /// If you need to modify where the tank should be restored to,
    /// modify where you call RecordOriginalTransforms().
    /// </summary>
    private List<Vector3> originalPositions;

    /// <summary>
    /// The rotations that each piece should be restored to.
    /// Modify these the same way you would handle originalPositions.
    /// </summary>
    private List<Quaternion> originalRotations;

    /// <summary>
    /// Temporary Rigidbodies, required by the pieces for physics calculations.
    /// These actually cannot coexist with functional tanks,
    /// so they must be added and deleted when the tank explodes and is restored.
    /// Before an explosion, all elements in this array are null.
    /// </summary>
    private List<Rigidbody> tempRigidbodies;



    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {

    }


    /// <summary>
    /// Explodes the pieces so they all fall apart.
    /// This is done by assigning them each a Rigidbody at runtime,
    /// and then applying a pseudorandom force to each of them.
    /// </summary>
    public void Explode() {

        //Debug.Log("Entering explode on target " + name);

        if (exploded) return;

        System.Random r = new System.Random();

        Vector3 evenExplosionOffset = new Vector3(
            MapToRange((float)r.NextDouble(), 0f, 1f, -2f, 2f),    // x
            -0.5f,                                                 // y
            MapToRange((float)r.NextDouble(), 0f, 1f, -2f, 2f)     // z
            );

        Vector3 vertExplosionOffset = new Vector3(
            MapToRange((float)r.NextDouble(), 0f, 1f, -0.7f, 0.7f),    // x
            -1f,                                                       // y
            MapToRange((float)r.NextDouble(), 0f, 1f, -0.7f, 0.7f)     // z
            );


        for (int i = 0; i < pieces.Count; i++) {

            // First, add the temporary Rigidbody,
            // if it does not have one already
            if (pieces[i].GetComponent<Rigidbody>() == null) {
                tempRigidbodies.Add(pieces[i].AddComponent<Rigidbody>());
            }

            // else, if they already have Rigidbodies,
            // but 
            // just use theirs,
            // but make sure it will work properly with physics
            else {


                tempRigidbodies.Add(pieces[i].GetComponent<Rigidbody>());
                tempRigidbodies[i].useGravity = true;
                //Debug.Log(tempRigidbodies[i].gameObject.name)
                if(pieces[i].gameObject.tag != "Resource")
                    tempRigidbodies[i].constraints = RigidbodyConstraints.None;
                //tempRigidbodies[i].isKinematic = true;    // otherwise, NO COLLISION
            }

            Assert.IsTrue(i == tempRigidbodies.Count - 1,
                "tempRigidbodies.Count - 1 = " + (tempRigidbodies.Count - 1) + " but i is " + i);

            // Then, apply explosion forces
            if (i % 2 == 0) {
                tempRigidbodies[i].AddExplosionForce(forceOfExplosion,
                    pieces[i].transform.position + evenExplosionOffset, // Offset each explosion slightly for effect
                    radiusOfExplosion, 0.0f, ForceMode.Impulse);
            } else {
                tempRigidbodies[i].AddExplosionForce(forceOfExplosion,
                    pieces[i].transform.position + vertExplosionOffset, // Make some pieces spring up higher
                    radiusOfExplosion, 0.0f, ForceMode.Impulse);
            }
        }

        // Only play the explosion system if one was added. Not everything will
        // necessarily have one.
        if (efc)
        {
            efc.Play(); 
       }
        
        exploded = true;

        //Debug.Log("Exiting explode on target " + name);
    }

    /// <summary>
    /// Fixes an exploded bunch of pieces.
    /// The Rigidbodies are removed to restore functionality,
    /// and then the pieces' transforms are all reset to their values before their explosion.
    /// </summary>
    public void Restore() {

        if (!exploded) return;

        // First, remove tempRigidbodies,
        // which removes the effects of physics as well
        for (int i = 0; i < pieces.Count; i++) {

            //tempRigidbodies[i].transform.rotation = originalRotations[i];
            //tempRigidbodies[i].transform.position = originalPositions[i];

            tempRigidbodies[i].velocity = new Vector3(0f, 0f, 0f);

            // Remove tempRigidbodies,
            // which removes the effects of physics as well
            Destroy(tempRigidbodies[i]);

            // Then, restore original transforms
            pieces[i].transform.localRotation = originalRotations[i];
            pieces[i].transform.localPosition = originalPositions[i];
            // Transforms also have a scale, but these are unchanged
        }

        transform.position = originalParentPosition;
        transform.rotation = originalParentRotation;

        exploded = false;
    }


    /// <summary>
    /// Records the transforms for the individual pieces.
    /// These will be later used to restore the pieces once they explode.
    /// </summary>
    private void RecordOriginalTransforms() {

        originalParentPosition = transform.position;
        originalParentRotation = transform.rotation;

        for (int i = 0; i < pieces.Count; i++) {

            originalPositions.Add(pieces[i].transform.localPosition);
            originalRotations.Add(pieces[i].transform.localRotation);

        }

        Assert.IsTrue(originalPositions.Count == pieces.Count,
            "originalPositions is not the same size as pieces");
    }

    // Start is called before the first frame update
    void Start() {
        exploded = false;

        originalPositions = new List<Vector3>(pieces.Count);
        originalRotations = new List<Quaternion>(pieces.Count);
        tempRigidbodies = new List<Rigidbody>(pieces.Count);

        RecordOriginalTransforms();
    }

    // Update is called once per frame
    void Update() {

        if (doExplosionDemo)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0) && !exploded)
            {

                Debug.Log("Exploding!");
                Explode();

            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && exploded)
            {

                Debug.Log("Fixing!");
                Restore();
            }
        }
    }

    /// <summary>
    /// Adds piece to pieces,
    /// and to originalPositions, originalRotations, and tempRigidBodies.
    /// Piece should be at the same index
    /// in all of the above lists.
    /// </summary>
    /// <param name="piece">Piece.</param>
    public void AddPiece(GameObject piece)
    {
        pieces.Add(piece);
        originalPositions.Add(piece.transform.position);
        originalRotations.Add(piece.transform.rotation);
    }

    private float MapToRange(float x, float old_lo, float old_hi, float new_lo, float new_hi) {

        x -= old_lo;
        x /= old_hi - old_lo;

        x *= (new_hi - new_lo);
        x += new_lo;

        return x;
    }
}
