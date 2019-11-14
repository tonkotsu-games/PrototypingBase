using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IBehaviorAI
{
    [SerializeField]
    private float moveSpeed = 0;
    [SerializeField]
    [Range(0, 100)]
    float detectionRange;
    [SerializeField]
    [Range(0, 10)]
    float stopDistance;
    [SerializeField]
    [Range(0, 10)]
    float collisionRange;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    float gravity;
    [SerializeField]
    float jumpForce;
    [SerializeField]
    private GameObject player;

    private Rigidbody rigidbody;

    Vector3 startPosition;

    private bool target = false;

    private Selector rootBehavior;
    private Sequence CheckPlayerDistance;
    private Sequence Jump;
    private Sequence BackToStartPosition;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        startPosition = gameObject.transform.position;

        CheckPlayerDistance = new Sequence(new List<BaseNode>
        {
            new PlayerInRangeNode(detectionRange, player.transform,this),
            new WalkToPlayerNode(stopDistance,moveSpeed,player,this,rigidbody,gravity,this),
            new CheckCollisionNode(collisionRange,this,layerMask)
        });

        Jump = new Sequence(new List<BaseNode>
         {
             new JumpNode(jumpForce,gameObject)
         });


        BackToStartPosition = new Sequence(new List<BaseNode>
        {
            new CheckToStartNode(this),
            new WalkToStartNode(moveSpeed,gravity,gameObject,startPosition, this),
            new CheckCollisionNode(collisionRange,this,layerMask)
        });

        rootBehavior = new Selector(new List<BaseNode>
        {
            CheckPlayerDistance,
            BackToStartPosition,
            Jump
        });
    }

    private void FixedUpdate()
    {
        rootBehavior.Evaluate();
    }

    public bool SetTarget(bool target = false)
    {
        this.target = target;
        return this.target;
    }

    public Transform GetAITransform()
    {
        return transform;
    }

    public bool GetTarget()
    {
        return target;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, 0.1f + transform.localScale.y / 2 + transform.position.y, transform.position.z), new Vector3(collisionRange, transform.localScale.y, collisionRange));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), 0.1f);
    }
    public bool OnGround()
    {
        return Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), 0.1f, layerMask);
    }
}