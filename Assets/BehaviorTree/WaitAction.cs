using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting.Antlr3.Runtime;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait", story: "Wait", category: "Action", id: "5a5ca87ce73ab0ee64e7f018e4d495da")]
public partial class WaitAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;
    bool done = false;
    bool first = false;

    protected override Status OnStart()
    {
        Variable.Value = false;
        if (SnakeScript.SnakeInstance == null)
        {
            return Status.Failure;
        }
        CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.Wait(SnakeScript.SnakeInstance.Wait(), () => Variable.Value = true));
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