using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPositionNode : BaseNode
{
    IBehaviorAI behaviorAI;


    public CheckPositionNode(IBehaviorAI behaviorAI)
    {
        this.behaviorAI = behaviorAI;
    }

    public override NodeStates Evaluate()
    {
        Vector3 agentPosition = behaviorAI.GetAITransform().position;
        Vector3 wayPointPosition = behaviorAI.GetWayPointPosition();
        float distance = Vector3.Distance(agentPosition, wayPointPosition);

        if(distance < 10f)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }
}