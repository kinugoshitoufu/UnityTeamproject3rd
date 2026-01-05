using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckMoveAttackCountCondition", story: "CheckMoveAttackCountCondition", category: "Conditions", id: "ec702599eb5c89a0a3d1556f66832374")]
public partial class CheckMoveAttackCountCondition : Condition
{

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
