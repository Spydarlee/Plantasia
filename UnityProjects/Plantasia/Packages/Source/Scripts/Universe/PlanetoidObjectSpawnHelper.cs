using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetoidObjectSpawnHelper : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static PlanetoidObjectSpawnHelper Instance = null;

    // -------------------------------------------------------------------------------

    public float            MaxSurfaceAngleDiff = 10.0f;
    public LayerMask        ObstaclesLayerMask = 0;
    public LayerMask        PlanetoidLayerMask = 0;
    public float            RayDistance = 20.0f;

    // -------------------------------------------------------------------------------

    private Vector3[]       mRaycastOrigins = new Vector3[4];
    private Ray             mRay = new Ray();
    private RaycastHit      mRaycastHit;

    // -------------------------------------------------------------------------------

    public void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------------------

    public bool GetRandomSpawnPosOnPlanetoid(float radius, out Vector3 position, out Vector3 normal, int maxAttempts = 10)
    {
        position = Vector3.zero;
        normal = Vector3.zero;

        bool foundValidSpawnPos = false;
        int numAttempts = 0;

        while (!foundValidSpawnPos && numAttempts < maxAttempts)
        {
            if (GetRandomPosOnPlanetoid(out position, out normal))
            {
                if (CanSpawnObject(radius, position, normal, out normal))
                {
                    foundValidSpawnPos = true;
                    break;
                }
                else
                {
                    numAttempts++;
                }
            }
            else
            {
                numAttempts++;
            }
        }

        return foundValidSpawnPos;
    }

    // -------------------------------------------------------------------------------

    public bool GetRandomPosOnPlanetoid(out Vector3 position, out Vector3 normal)
    {
        mRay.origin = new Vector3(
            Random.Range(-1f, 1f) * RayDistance,
            Random.Range(-1f, 1f) * RayDistance,
            Random.Range(-1f, 1f) * RayDistance);

        mRay.direction = (Vector3.zero - mRay.origin);

        if (Physics.Raycast(mRay, out mRaycastHit, RayDistance, PlanetoidLayerMask, QueryTriggerInteraction.Ignore))
        {
            normal = mRaycastHit.normal;
            position = mRaycastHit.point;
            return true;
        }

        normal = Vector3.zero;
        position = Vector3.zero;
        return false;
    }

    // -------------------------------------------------------------------------------

    public bool CanSpawnObject(float radius, Vector3 position, Vector3 groundNormal, out Vector3 averageGroundNormal)
    {
        averageGroundNormal = groundNormal;

        if (Physics.CheckSphere(position, radius, ObstaclesLayerMask))
        {
            // Can't spawn if we've overlapping with anything else
            return false;
        }

        var centerSurfaceAngle = Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg;

        // A point above (along the ground normal) the plant
        var raycastOriginCenter = position + (groundNormal * 10.0f);

        // Construct a reference frame aroudn the ground normal
        var right = Vector3.Cross(Vector3.forward, groundNormal);
        var up = Vector3.Cross(right, groundNormal);

        // Calcuate four points around the plant position
        mRaycastOrigins[0] = raycastOriginCenter + (right * radius);
        mRaycastOrigins[1] = raycastOriginCenter + (-right * radius);
        mRaycastOrigins[2] = raycastOriginCenter + (up * radius);
        mRaycastOrigins[3] = raycastOriginCenter + (-up * radius);

        mRay.direction = -groundNormal;

        for (int i = 0; i < 4; i++)
        {
            mRay.origin = mRaycastOrigins[i];
            if (Physics.Raycast(mRay, out mRaycastHit, 10.5f, PlanetoidLayerMask, QueryTriggerInteraction.Ignore))
            {
                var surfaceAngle = Mathf.Atan2(mRaycastHit.normal.y, mRaycastHit.normal.x) * Mathf.Rad2Deg;
                averageGroundNormal += mRaycastHit.normal;

                // We only want to spawn on ground that's "roughly" flat and not
                // like on a sheer edge or anything like that
                if (Mathf.Abs(surfaceAngle - centerSurfaceAngle) > MaxSurfaceAngleDiff)
                {
                    return false;
                }
            }
            else
            {
                // If there's no ground here we can't be spawning here tbh!
                return false;
            }
        }

        // Five normals (the original ground normal and our four extra checks)
        // went into this total, so divide 5 to get our average normal
        averageGroundNormal /= 5.0f;
        averageGroundNormal.Normalize();

        return true;
    }

    // -------------------------------------------------------------------------------

    public void UpdateCameraLookAtTarget()
    {
        mRay.origin = Vector3.up * 100.0f;
        mRay.direction = Vector3.down;

        if (Physics.Raycast(mRay, out mRaycastHit, 1000.0f, PlanetoidLayerMask, QueryTriggerInteraction.Ignore))
        {
            CameraController.Instance.LookAtTarget = mRaycastHit.point;
        }
    }

    // -------------------------------------------------------------------------------
}
