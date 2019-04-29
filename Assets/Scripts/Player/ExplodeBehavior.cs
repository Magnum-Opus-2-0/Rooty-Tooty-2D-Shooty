using System;   // needed for Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehavior : MonoBehaviour {

    public bool doExplosionDemo;

    public const int NUM_OF_PIECES = 3;
    private bool exploded;  // used to track state

    // Variables and references which are configured via Unity Editor
    public float forceOfExplosion;
    public float radiusOfExplosion;

    private Vector3 originalParentPosition;
    private Quaternion originalParentRotation;

    /// <summary>
    /// Pieces of the tank (or whatever!) which need to explode apart.
    /// </summary>
    public GameObject[] pieces = new GameObject[NUM_OF_PIECES];

    /// <summary>
    /// The positions that each piece should be restored to.
    /// This can be either the tank's last unexploded position,
    /// or its spawn point.
    /// If you need to modify where the tank should be restored to,
    /// modify where you call RecordOriginalTransforms().
    /// </summary>
    private Vector3[] originalPositions = new Vector3[NUM_OF_PIECES];

    /// <summary>
    /// The rotations that each piece should be restored to.
    /// Modify these the same way you would handle originalPositions.
    /// </summary>
    private Quaternion[] originalRotations = new Quaternion[NUM_OF_PIECES];

    /// <summary>
    /// Temporary Rigidbodies, required by the pieces for physics calculations.
    /// These actually cannot coexist with functional tanks,
    /// so they must be added and deleted when the tank explodes and is restored.
    /// Before an explosion, all elements in this array are null.
    /// </summary>
    private Rigidbody[] rigidbodies = new Rigidbody[NUM_OF_PIECES];

    /// <summary>
    /// Explodes the pieces so they all fall apart.
    /// This is done by assigning them each a Rigidbody at runtime,
    /// and then applying a pseudorandom force to each of them.
    /// </summary>
    public void Explode() {

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

        for (int i = 0; i < NUM_OF_PIECES; i++) {

            // First, add the temporary Rigidbody
            rigidbodies[i] = pieces[i].AddComponent<Rigidbody>();

            //rigidbodies[i].drag = Mathf.Infinity;

            // Then, apply explosion forces
            if (i % 2 == 0) {
                rigidbodies[i].AddExplosionForce(forceOfExplosion,
                    pieces[i].transform.position + evenExplosionOffset, // Offset each explosion slightly for effect
                    radiusOfExplosion, 0.0f, ForceMode.Impulse);
            } else {
                rigidbodies[i].AddExplosionForce(forceOfExplosion,
                    pieces[i].transform.position + vertExplosionOffset, // Make some pieces spring up higher
                    radiusOfExplosion, 0.0f, ForceMode.Impulse);
            }


        }

        exploded = true;
    }

    /// <summary>
    /// Fixes an exploded bunch of pieces.
    /// The Rigidbodies are removed to restore functionality,
    /// and then the pieces' transforms are all reset to their values before their explosion.
    /// </summary>
    public void Restore() {

        if (!exploded) return;

        Debug.Log("should fall through");

        // First, remove rigidbodies,
        // which removes the effects of physics as well
        for (int i = 0; i < NUM_OF_PIECES; i++) {

            //rigidbodies[i].transform.rotation = originalRotations[i];
            //rigidbodies[i].transform.position = originalPositions[i];

            rigidbodies[i].velocity = new Vector3(0f, 0f, 0f);

            // Remove rigidbodies,
            // which removes the effects of physics as well
            Destroy(rigidbodies[i]);

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

        for (int i = 0; i < NUM_OF_PIECES; i++) {

            originalPositions[i] = pieces[i].transform.localPosition;
            originalRotations[i] = pieces[i].transform.localRotation;

        }
    }

    // Start is called before the first frame update
    void Start() {
        exploded = false;

        RecordOriginalTransforms();
    }

    // Update is called once per frame
    void Update() {

        if (doExplosionDemo) {

            if (Input.GetKeyDown(KeyCode.Mouse0) && !exploded) {

                Debug.Log("Exploding!");
                Explode();

            } else if (Input.GetKeyDown(KeyCode.Mouse1) && exploded) {

                Debug.Log("Fixing!");
                Restore();
            }
        }

    }

    private float MapToRange(float x, float old_lo, float old_hi, float new_lo, float new_hi) {

        x -= old_lo;
        x /= old_hi - old_lo;

        x *= (new_hi - new_lo);
        x += new_lo;

        return x;
    }
}
