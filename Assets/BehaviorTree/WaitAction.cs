using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait", story: "Wait", category: "Action", id: "5a5ca87ce73ab0ee64e7f018e4d495da")]
public partial class WaitAction : Action
{
    bool done = false;
    bool first = false;
    protected override Status OnStart()
    {
        CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.Wait());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //if(first==false)CoroutineRunner.Wait(SnakeScript.SnakeInstance.Wait(), () => done = true);
        //first = true;
        //if (done) return Status.Success;
        //else return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

