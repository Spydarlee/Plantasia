using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a pool of GameObjects for a given Prefab.
/// The GameObjectPooler will use these as its initial pools.
/// </summary>
[System.Serializable]
public class GameObjectPoolInfo 
{
    public GameObject       Prefab;
    public List<GameObject> Instances = new List<GameObject>();
}