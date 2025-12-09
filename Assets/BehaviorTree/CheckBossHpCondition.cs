using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckBossHP", story: "CheckBossHP", category: "Conditions", id: "7dded3518613f5641e873374f359329c")]
public partial class CheckBossHpCondition : Condition
{

    public override bool IsTrue()
    {
        return SnakeScript.SnakeInstance.CheckHP();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
