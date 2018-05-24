using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManipulator : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public Collider Collider;

    // -------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        var seed = other.GetComponent<Seed>();
        if (seed)
        {
            seed.ApplyRandomForce();
        }
    }

    // -------------------------------------------------------------------------------
}
