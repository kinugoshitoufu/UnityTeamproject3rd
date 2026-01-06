using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckElephantHP", story: "IF HP50%", category: "Conditions", id: "1509c8f0ad35668a9726b0eff70abb55")]
public partial class CheckElephantHpCondition : Condition
{

    public override bool IsTrue()
    {
        return Elephant.elephant.CheckHP();
    }

    public override void OnStart()
    {

    }

    public override void OnEnd()
    {

    }
}
