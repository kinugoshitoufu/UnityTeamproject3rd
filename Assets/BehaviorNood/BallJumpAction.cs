using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BallJump", story: "Jump and Ball", category: "Action", id: "ea65e7cf1a19b8fd943bbb92f4389187")]
public partial class BallJumpAction : Action
{

    protected override Status OnStart()
    {
        if (Elephant.elephant == null)
        {
            Elephant.elephant = GameObject.FindAnyObjectByType<Elephant>();
            //return Status.Failure;
        }
            CoroutineRunner.Instance.StartCoroutine(Elephant.elephant.BallandJump());
            return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Elephant.elephant.EventEnd ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

