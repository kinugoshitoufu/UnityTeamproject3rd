using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TestLog", story: "TestLog", category: "Action", id: "d1099d9d423f6c743e6dd4a6fa582c94")]
public partial class TestLogAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("Heaven");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("Inferno");
        return Status.Success;
    }

    protected override void OnEnd()
    {
        Debug.Log("â‰àßêl");
    }
}

