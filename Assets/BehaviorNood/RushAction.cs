using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Rush", story: "Player to Rush", category: "Action", id: "9d23f1b9e467b512874f82b8e12fed20")]
public partial class RushAction : Action
{
    [SerializeField] public BlackboardVariable<bool> Variable;

    protected override Status OnStart()
    {
        if (Elephant.elephant == null)
        {
            return Status.Failure;
        }
        CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.Wait( Elephant.elephant.MoveAttack1(),()=>Variable.Value=true));
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

