using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IBehaviorAI
{
    [SerializeField]
    private float moveSpeed = 0;
    [SerializeField]
    private float searchWayPointRange = 0;

    private Rigidbody rigidbody;
    
    public Vector3 wayPointPosition = Vector3.zero;


    private Selector rootBehavior;
    private Sequence checkPosition;
    private Sequence walkTo;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        checkPosition = new Sequence(new List<BaseNode>
        {
            new CheckPositionNode(this),
            new WayPointNode(this, searchWayPointRange)
        });

        walkTo = new Sequence(new List<BaseNode>
        {
            new WalkToNode(this, 100f, moveSpeed, rigidbody)
        });

        rootBehavior = new Selector(new List<BaseNode>
        {
            checkPosition,
            walkTo
        });
    }

    private void FixedUpdate()
    {
        rootBehavior.Evaluate();
    }

    public Vector3 SetTargetPosition(Vector3 targetPosition)
    {
        wayPointPosition = targetPosition;
        return wayPointPosition;
    }

    public Transform GetAITransform()
    {
        return transform;
    }

    public Vector3 GetWayPointPosition()
    {
        return wayPointPosition;
    }
}