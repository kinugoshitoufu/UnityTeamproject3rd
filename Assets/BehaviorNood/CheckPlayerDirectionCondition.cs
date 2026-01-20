using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckPlayerDirection", story: "CheckPlayerDirection", category: "Conditions", id: "531ff30fb0dd75130042a9ff716b7458")]
public partial class CheckPlayerDirectionCondition : Condition
{
    public override bool IsTrue()
    {
       return Elephant.elephant.CheckPlayerDirection();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
