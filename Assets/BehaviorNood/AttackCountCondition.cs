using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "AttackCount", story: "AttackCount", category: "Conditions", id: "f262e5a97405e546e4793b787d35c970")]
public partial class AttackCountCondition : Condition
{

    public override bool IsTrue()
    {
        int Count = Elephant.elephant.GetAttackCount();
        if (Count > 2) return true;
        else return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
