using ActionGraph;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaitForCondition : Action
{
    // -------------------------------------------------------------------------------

    public Condition    ConditionToWaitFor = null;
    public bool         InvertCondition = false;

    // -------------------------------------------------------------------------------

    public override string DisplayName
    {
        get { return (InvertCondition) ? "Wait For !" + ConditionToWaitFor : "Wait For " + ConditionToWaitFor; }
    }

    // -------------------------------------------------------------------------------

    protected override void OnStart()
    {
        if (ConditionToWaitFor == null)
        {
            Debug.LogWarning("WaitForCondition has no condition! Ending action...");
            FinishAction();
        }
        else
        {
            var checkResult = ConditionToWaitFor.Check();
            if (checkResult != InvertCondition)
            {
                FinishAction();
            }
        }
    }

    // -------------------------------------------------------------------------------

    protected override void OnUpdate()
    {
        var checkResult = ConditionToWaitFor.Check();
        if (checkResult != InvertCondition)
        {
            FinishAction();
        }
    }

    // -------------------------------------------------------------------------------

    #if UNITY_EDITOR
    public override bool OnGUI()
    {
        if (ConditionToWaitFor == null)
        {
            EditorGUIHelper.CreateConditionButton((condition) =>
            {
                ConditionToWaitFor = condition;
            });

        }
        else
        {
            EditorGUIHelper.ShowConditionInspector(ConditionToWaitFor);
            if (GUILayout.Button("Delete Condition"))
            {
                ConditionToWaitFor = null;
            }
            InvertCondition = EditorGUILayout.Toggle("Invert Condition", InvertCondition);
        }

        return true;
    }
    #endif

    // -------------------------------------------------------------------------------
}
