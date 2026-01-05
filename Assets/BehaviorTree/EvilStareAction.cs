using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EvilStare", story: "EvilStare", category: "Action", id: "f888f38a7af689b28f0f1a6fc23f9b68")]
public partial class EvilStareAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;
    protected override Status OnStart()
    {
        Variable.Value = false;
        CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.Wait(SnakeScript.SnakeInstance.EvilStare(), () => Variable.Value = true));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Variable.Value ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

