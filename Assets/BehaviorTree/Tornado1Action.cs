using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Tornado1", story: "Tornado1", category: "Action", id: "f99edcdf17b0648befb9d84dc8c1be20")]
public partial class Tornado1Action : Action
{
    bool done = false;
    bool first = false;
    protected override Status OnStart()
    {
        CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.Tornado1());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //if(first==false)CoroutineRunner.Wait(SnakeScript.SnakeInstance.Tornado1(),()=>done=true);
        //first = true;
        //if(done)return Status.Success;
        //else return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

