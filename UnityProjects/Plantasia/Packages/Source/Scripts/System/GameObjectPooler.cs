using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameObjectPooler owns a collection of GameObject pools that help prevent/limit the need for
/// runtime instiation of GameObjects. Pass in a reference to the prefab for the object
/// you want an instance of and the Pooler will return an existing one if one is free, or create
/// a new one on demand. These can then be returned the pool after they've been used.
/// Initial pools are defined by the list of PoolInfos which can be configured from the inspector.
/// </summary>
public class GameObjectPooler : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static GameObjectPooler Instance;

    // -------------------------------------------------------------------------------

    public List<GameObjectPoolInfo> PoolInfos = new List<GameObjectPoolInfo>();

    // -------------------------------------------------------------------------------

    private Dictionary<GameObject, List<GameObject>>    mGameObjectPools = new Dictionary<GameObject, List<GameObject>>();  // Key: Prefab, Value: Instances
    private Dictionary<GameObject, GameObject>          mGameObjectsInUse = new Dictionary<GameObject, GameObject>();       // Key: Instance, Value: Prefab

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        // Create our initial pools from the PoolInfos setup in the editor
        foreach (var poolInfo in PoolInfos)
        {
            mGameObjectPools.Add(poolInfo.Prefab, poolInfo.Instances);
        }
    }

    // -------------------------------------------------------------------------------

    public GameObject GetPooledInstance(GameObject prefab, Transform parent = null)
    {
        return GetPooledInstance(prefab, prefab.transform.position, prefab.transform.rotation, parent);
    }

    // -------------------------------------------------------------------------------

    public GameObject GetPooledInstance(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject instanceToReturn = null;

        if (prefab != null)
        {
            List<GameObject> pool;

            // Do we have a pool for this prefab yet?
            if (mGameObjectPools.TryGetValue(prefab, out pool))
            {
                // There's a pool, so return an unused instance or create a new one as needed
                if (pool.Count > 0)
                {
                    instanceToReturn = pool[0];
                }
                else
                {
                    // Create a new instance and add it to the pool
                    instanceToReturn = GameObject.Instantiate(prefab, position, rotation, parent);
                    mGameObjectPools[prefab].Add(instanceToReturn);
                }
            }
            else
            {
                // We don't have a pool, so create one plus a new instance
                instanceToReturn = GameObject.Instantiate(prefab, position, rotation, parent);
                mGameObjectPools.Add(prefab, new List<GameObject> { instanceToReturn });
            }
        }
        else
        {
            Debug.LogError("GameObjectPooler can not create an instance of a null prefab!", gameObject);
        }

        // We've got an instance to return, make sure it's setup and tracked 
        if (instanceToReturn != null)
        {
            instanceToReturn.transform.position = position;
            instanceToReturn.transform.rotation = rotation;
            instanceToReturn.transform.SetParent(parent);
            instanceToReturn.SetActive(true);

            mGameObjectPools[prefab].Remove(instanceToReturn);
            mGameObjectsInUse.Add(instanceToReturn, prefab);
        }

        return instanceToReturn;
    }

    // -------------------------------------------------------------------------------

    public void ReturnToPool(GameObject instance)
    {
        // Is this an object we have loaned out from a pool?
        if (mGameObjectsInUse.ContainsKey(instance))
        {
            // Yup, great, grab the prefab this is an instance of
            var prefab = mGameObjectsInUse[instance];

            // And put this object back in the pool while removing it from the "in use" list
            mGameObjectPools[prefab].Add(instance);
            mGameObjectsInUse.Remove(instance);

            // Finally, switch the object off and pull it back under the pooler
            instance.transform.SetParent(transform);
            instance.SetActive(false);
        }
    }

    // -------------------------------------------------------------------------------
}
