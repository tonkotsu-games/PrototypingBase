using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRangeNode : BaseNode
{
    float detectionRange;
    Transform player;
    IBehaviorAI behaviorAI;

    public PlayerInRangeNode(float range, Transform player, IBehaviorAI behaviorAI)
    {
        this.detectionRange = range;
        this.player = player;
        this.behaviorAI = behaviorAI;
    }

    public override NodeStates Evaluate()
    {
        Vector3 agentPosition = behaviorAI.GetAITransform().position;
        float distance = Vector3.Distance(agentPosition, player.position);

        if(distance < detectionRange)
        {
            behaviorAI.SetTarget(true);
            return NodeStates.SUCCESS;
        }
        else
        {
            behaviorAI.SetTarget();
            return NodeStates.FAILURE;
        }
    }
}