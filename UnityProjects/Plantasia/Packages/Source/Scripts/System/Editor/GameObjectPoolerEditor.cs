using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(GameObjectPooler))]
public class GameObjectPoolerEditor : Editor
{
    // -------------------------------------------------------------------------------

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("The GameObjectPooler can't be modified while the game is running!");
            return;
        }

        GameObjectPooler gameObjectPooler = (GameObjectPooler)target;
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Add Pool Info"))
        {
            gameObjectPooler.PoolInfos.Add(new GameObjectPoolInfo());
        }

        for (int i = 0; i < gameObjectPooler.PoolInfos.Count; i++)
        {
            var poolInfo = gameObjectPooler.PoolInfos[i];

            EditorGUILayout.BeginHorizontal();

            var newPrefab = (GameObject)EditorGUILayout.ObjectField((GameObject)poolInfo.Prefab, typeof(GameObject), false);
            var chosenNewPrefab = (newPrefab != poolInfo.Prefab);

            // Make sure we don't already have a pool for this prefab before selecting it
            if (chosenNewPrefab)
            {
                foreach (var otherPoolInfo in gameObjectPooler.PoolInfos)
                {
                    if (otherPoolInfo != poolInfo && otherPoolInfo.Prefab == newPrefab)
                    {
                        Debug.LogWarning("You can't create two GameObjectPoolInfos with the same prefab, friend!", gameObjectPooler);
                        chosenNewPrefab = false;
                        break;
                    }
                }
            }

            if (chosenNewPrefab)
            {
                DestroyAllInstances(poolInfo);
                poolInfo.Prefab = newPrefab;
            }            

            // Has the user requested a chance in the number of instances?
            var numInstances = EditorGUILayout.DelayedIntField(poolInfo.Instances.Count);
            if (numInstances != poolInfo.Instances.Count)
            {
                if (poolInfo.Prefab != null)
                {
                    DestroyAllInstances(poolInfo);
                    poolInfo.Instances.Clear();

                    for (int j = 0; j < numInstances; j++)
                    {
                        var newInstance = Instantiate(poolInfo.Prefab, gameObjectPooler.transform);
                        poolInfo.Instances.Add(newInstance);
                        newInstance.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogWarning("Please add a Prefab to the PoolInfo before trying to instantiate!", gameObjectPooler);
                }
            }

            // Delete the pool and all instances owned by it?
            if (GUILayout.Button("X"))
            {
                DestroyAllInstances(poolInfo);
                gameObjectPooler.PoolInfos.RemoveAt(i);
                break;
            }

            gameObjectPooler.PoolInfos[i] = poolInfo;
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gameObjectPooler, "Changed GameObjectPooler Pool Infos");
            EditorUtility.SetDirty(gameObjectPooler);
            EditorSceneManager.MarkSceneDirty(gameObjectPooler.gameObject.scene);
        }
    }

    // -------------------------------------------------------------------------------

    private void DestroyAllInstances(GameObjectPoolInfo poolInfo)
    {
        foreach (var instance in poolInfo.Instances)
        {
            DestroyImmediate(instance);
        }

        poolInfo.Instances.Clear();
    }

    // -------------------------------------------------------------------------------
}
