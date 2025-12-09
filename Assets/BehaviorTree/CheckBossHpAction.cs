using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckBossHP", story: "CheckBossHP", category: "Action/Conditional", id: "e655a21879de7b409e3da7adeb68e186")]
public partial class CheckBossHpAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;

    protected override Status OnStart()
    {
        Variable.Value = SnakeScript.SnakeInstance.CheckHP();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        flag();
    }

    bool flag()
    {
        return Variable.Value;
    }
}

