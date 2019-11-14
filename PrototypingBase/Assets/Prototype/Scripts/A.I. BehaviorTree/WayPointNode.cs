using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointNode : BaseNode
{
    float range;
    IBehaviorAI behaviorAI;

    public WayPointNode(IBehaviorAI behaviorAI, float range)
    {
        this.range = range;
        this.behaviorAI = behaviorAI;
    }

    public override NodeStates Evaluate()
    {
        Vector3 newTarget = Random.insideUnitSphere * range;
        newTarget.y = 0;

        behaviorAI.SetTargetPosition(newTarget);

        return NodeStates.SUCCESS;
    }
}