using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionGraph;

public class DespawnAllClouds : Action
{
    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        Universe.Instance.DespawnAllClouds();
        FinishAction();
    }

    // -------------------------------------------------------------------------------
}
