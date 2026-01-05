using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckPlayerPostion", story: "Check to Player Postion of Square Postion", category: "Conditions", id: "15f704d4f45d66f1cc5b1549e6f2deae")]
public partial class CheckPlayerPostionCondition : Condition
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
