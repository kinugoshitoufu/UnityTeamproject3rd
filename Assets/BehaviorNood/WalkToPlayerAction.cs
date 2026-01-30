using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Walk", story: "Walk to [Player]", category: "Action", id: "4d9b4c2311a3f75a6af0814691ba4e3b")]
public partial class WalkToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Player;

    protected override Status OnStart()
    {
        if (Elephant.elephant == null)
        {
            Elephant.elephant = GameObject.FindAnyObjectByType<Elephant>();
        }
        CoroutineRunner.Instance.StartCoroutine(Elephant.elephant.WalkCoroutine());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return (Elephant.elephant.EventEnd) ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

