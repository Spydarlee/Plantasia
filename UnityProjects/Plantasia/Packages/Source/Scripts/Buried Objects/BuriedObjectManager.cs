using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuriedObjectManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public List<BuriedPlanetoidProxy> PlanetoidProxyBuriedObjects = new List<BuriedPlanetoidProxy>();

    // -------------------------------------------------------------------------------

    private List<BuriedObject> mCurrentlySpawnedBuriedObjects = new List<BuriedObject>();

    // -------------------------------------------------------------------------------

    void Start()
    {
        Universe.Instance.OnPlanetoidChange += OnPlanetoidChange;

        // Turn off all our editor setup buried objects before we start!
        foreach (var buriedObjet in PlanetoidProxyBuriedObjects)
        {
            buriedObjet.gameObject.SetActive(false);
        }
    }

    // -------------------------------------------------------------------------------

    private void OnPlanetoidChange(Planetoid newPlanetoid)
    {
        // Make sure the new planetoid is enabled to receive ray hits
        newPlanetoid.gameObject.SetActive(true);

        // Turn off any previously buried objects when we change planets
        foreach (var buriedObjet in mCurrentlySpawnedBuriedObjects)
        {
            buriedObjet.gameObject.SetActive(false);
        }

        // Do we want to spawn a home planetoid proxy buried objects on this planet?
        if (!SaveGameManager.Instance.SaveGameData.IsHomePlanetoidTypeUnlocked(newPlanetoid.PlanetoidType))
        {
            foreach (var buriedPlanetoidProxy in PlanetoidProxyBuriedObjects)
            {
                if (buriedPlanetoidProxy.PlanetoidType == newPlanetoid.PlanetoidType)
                {
                    Vector3 position;
                    Vector3 normal;

                    if (PlanetoidObjectSpawnHelper.Instance.GetRandomSpawnPosOnPlanetoid(buriedPlanetoidProxy.Radius, out position, out normal))
                    {
                        buriedPlanetoidProxy.gameObject.SetActive(true);
                        buriedPlanetoidProxy.ObjectToBury.SetActive(true);

                        buriedPlanetoidProxy.transform.position = position;
                        buriedPlanetoidProxy.transform.up = normal;
                        buriedPlanetoidProxy.transform.SetParent(newPlanetoid.transform);
                    }
                    else
                    {
                        Debug.Log("Couldn't find anywhere to spawn " + buriedPlanetoidProxy + ". Deactivating");
                        buriedPlanetoidProxy.gameObject.SetActive(false);
                    }
                }
            }
        }

        // Planets should be switched off during transitions so back to how it was!
        newPlanetoid.gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------------
}
