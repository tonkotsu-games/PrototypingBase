using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToNode : BaseNode
{
    IBehaviorAI behaviorAI;
    Rigidbody rigidbody;
    float range;
    float moveSpeed;

    public WalkToNode(IBehaviorAI behaviorAI, float range, float moveSpeed, Rigidbody rigidbody)
    {
        this.behaviorAI = behaviorAI;
        this.range = range;
        this.moveSpeed = moveSpeed;
        this.rigidbody = rigidbody;
    }

    public override NodeStates Evaluate()
    {
        Vector3 agentPosition = behaviorAI.GetAITransform().position;
        Vector3 wayPointPosition = behaviorAI.GetWayPointPosition();

        float distance = Vector3.Distance(agentPosition, wayPointPosition);
        Vector3 direction = wayPointPosition - agentPosition;
        rigidbody.velocity = direction * moveSpeed;

        return NodeStates.SUCCESS;
    }
}