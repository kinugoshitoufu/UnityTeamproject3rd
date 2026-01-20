using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveAttack1", story: "MoveAttack1", category: "Action", id: "ebc61daa7588db7ed2ab5e35901c2e50")]
public partial class MoveAttack1Action : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;

    bool done = false;
    bool first = false;
    protected override Status OnStart()
    {
        Variable.Value = false;
        CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.Wait(SnakeScript.SnakeInstance.MoveAttack1(), () => Variable.Value = true));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //return Variable.Value ? Status.Success : Status.Running;

        return (Elephant.elephant.EventEnd)? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

