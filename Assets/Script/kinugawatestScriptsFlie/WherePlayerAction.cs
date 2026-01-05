using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerWhere", story: "Get Player Postion", category: "Action", id: "a62e04ca0c53011c4a50493a0d669622")]
public partial class WherePlayerAction : Action
{
    private bool TestFlag;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        TestFlag = BehaviourTest.Instance.WherePlayer();
        if (TestFlag)
        {
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }

    protected override void OnEnd()
    {
        
    }
}

