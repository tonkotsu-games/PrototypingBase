using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisionNode : BaseNode
{
    float collisionRange;
    IBehaviorAI behaviorAI;
    LayerMask layerMask;

    public CheckCollisionNode(float collisionRange, IBehaviorAI behaviorAI, LayerMask layerMask)
    {
        this.collisionRange = collisionRange;
        this.behaviorAI = behaviorAI;
        this.layerMask = layerMask;
    }

    public override NodeStates Evaluate()
    {
        if(OnCollisionCheck())
        {
            Debug.Log("Collide");
            return NodeStates.FAILURE;
        }
        else
        {
            return NodeStates.SUCCESS;
        }
    }

    public bool OnCollisionCheck()
    {
        return Physics.CheckBox(new Vector3(behaviorAI.GetAITransform().position.x, 0.1f + behaviorAI.GetAITransform().localScale.y / 2 + behaviorAI.GetAITransform().position.y, behaviorAI.GetAITransform().position.z), 
                                new Vector3(collisionRange/2,behaviorAI.GetAITransform().localScale.y/2,collisionRange/2),Quaternion.identity,layerMask);
    }
}