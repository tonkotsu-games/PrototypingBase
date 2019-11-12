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


    public Selected rootBehavior;
    public Sequence checkPosition;
    public Sequence walkTo;

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

        rootBehavior = new Selected(new List<BaseNode>
        {
            checkPosition,
            walkTo
        });

        new WayPointNode(this, searchWayPointRange);
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