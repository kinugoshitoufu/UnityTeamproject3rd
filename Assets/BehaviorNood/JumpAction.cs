using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "Jump to [Player]", category: "Action", id: "3c2724edbade15350b9ff9dbbd12c058")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Player;
    [SerializeReference] public BlackboardVariable<UnityEngine.Transform> transform;

    protected override Status OnStart()
    {
        if(Elephant.elephant==null)
        {
            return Status.Failure;
        }
        Elephant.elephant.StartJumpAction();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Elephant.elephant.JumpFinished ? Status.Success:Status.Running;
    }

    protected override void OnEnd()
    {
    }
   
}

