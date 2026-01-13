using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckBossHPZero", story: "If HP Zero", category: "Conditions", id: "e54a1e8c10f96823cf1c4194de0cb346")]
public partial class CheckBossHpZeroCondition : Condition
{

    public override bool IsTrue()
    {
        return Elephant.elephant.CheckDeath();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
