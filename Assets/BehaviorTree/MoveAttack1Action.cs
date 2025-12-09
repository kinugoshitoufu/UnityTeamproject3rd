using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveAttack1", story: "MoveAttack1", category: "Action", id: "ebc61daa7588db7ed2ab5e35901c2e50")]
public partial class MoveAttack1Action : Action
{
    bool done = false;
    bool first = false;
    protected override Status OnStart()
    {
        CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.MoveAttack1());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //if(first==false)CoroutineRunner.Wait(SnakeScript.SnakeInstance.MoveAttack1(),()=>done=true);
        //first = true;
        //if(done)return Status.Success;
        //else return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

