using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckToStartNode : BaseNode
{
    IBehaviorAI behaviorAI;

    public CheckToStartNode(IBehaviorAI behaviorAI)
    {
        this.behaviorAI = behaviorAI;
    }

    public override NodeStates Evaluate()
    {
        switch(behaviorAI.GetTarget())
        {
            case true:
                {
                    return NodeStates.FAILURE;
                }
            case false:
                {
                    return NodeStates.SUCCESS;
                }
            default:
                {
                    return NodeStates.FAILURE;
                }
        }
    }
}