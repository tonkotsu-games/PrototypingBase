using UnityEngine;

public interface IBehaviorAI
{
    Vector3 SetTargetPosition(Vector3 targetPosition);
    Transform GetAITransform();
    Vector3 GetWayPointPosition();
}