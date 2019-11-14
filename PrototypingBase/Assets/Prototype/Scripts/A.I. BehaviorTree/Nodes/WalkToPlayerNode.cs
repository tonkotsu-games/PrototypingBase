using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPlayerNode : BaseNode
{
    private float stopDistance;
    private float moveSpeed;
    private float gravity;
    private Transform playerPosition;
    private IBehaviorAI behaviorAI;
    private Rigidbody rigidbody;
    private AIController enemy;

    public WalkToPlayerNode(float stopDistance, float moveSpeed, GameObject player, IBehaviorAI behaviorAI, Rigidbody rigidbody,float gravity,AIController enemy)
    {
        this.stopDistance = stopDistance;
        this.moveSpeed = moveSpeed;
        playerPosition = player.transform;
        this.behaviorAI = behaviorAI;
        this.rigidbody = rigidbody;
        this.enemy = enemy;
        this.gravity = gravity;
    }

    public override NodeStates Evaluate()
    {
        Vector3 agentPosition = behaviorAI.GetAITransform().position;
        Vector3 direction = playerPosition.position - agentPosition;
        float distance = Vector3.Distance(agentPosition, playerPosition.position);
        
        if(distance>stopDistance)
        {
            if (enemy.OnGround())
            {
                rigidbody.velocity = direction * moveSpeed;
            }
            else
            {
                //gravity += gravity * Time.deltaTime;
                rigidbody.velocity = new Vector3(direction.x * moveSpeed,gravity,direction.z*moveSpeed);
            }
            return NodeStates.SUCCESS;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            return NodeStates.SUCCESS;
        }
    }
}