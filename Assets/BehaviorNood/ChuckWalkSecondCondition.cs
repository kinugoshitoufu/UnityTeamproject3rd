using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ChuckWalkSecond", story: "Walking time", category: "Conditions", id: "9693ff0f9da9fb638226ebfe71414f0a")]
public partial class ChuckWalkSecondCondition : Condition
{

    public override bool IsTrue()
    {
        float timer= Elephant.elephant.GetWalktimer();
        if (timer > 5) { Elephant.elephant.ResetWalkTimer(); return true; }
        else return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
