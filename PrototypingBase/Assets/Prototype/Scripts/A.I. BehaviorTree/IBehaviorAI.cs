using UnityEngine;

public interface IBehaviorAI
{
    bool SetTarget(bool target = false);
    Transform GetAITransform();
    bool GetTarget();
}