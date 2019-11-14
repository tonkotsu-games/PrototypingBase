using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToStartNode : BaseNode
{
    float moveSpeed;
    float gravity;
    Rigidbody rigidbody;
    Vector3 startPosition;
    IBehaviorAI behaviorAI;
    AIController enemy;

    public WalkToStartNode(float moveSpeed, float gravity, GameObject enemy, Vector3 startPosition, IBehaviorAI behaviorAI)
    {
        this.moveSpeed = moveSpeed;
        this.gravity = gravity;
        rigidbody = enemy.GetComponent<Rigidbody>();
        this.startPosition = startPosition;
        this.behaviorAI = behaviorAI;
        this.enemy = enemy.GetComponent<AIController>();
    }

    public override NodeStates Evaluate()
    {
        Vector3 agentPosition = behaviorAI.GetAITransform().position;
        Vector3 direction = startPosition - agentPosition;
        float distance = Vector3.Distance(agentPosition, startPosition);

        if(distance < 1)
        {
            rigidbody.velocity = Vector3.zero;
            return NodeStates.SUCCESS;
        }else
        {
            if (enemy.OnGround())
            {
                rigidbody.velocity = direction * moveSpeed;
            }
            else
            {
                rigidbody.velocity = new Vector3(direction.x * moveSpeed, gravity, direction.z * moveSpeed);
            }
            return NodeStates.SUCCESS;
        }
    }
}
