using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpNode : BaseNode
{
    float jumpForce;
    AIController enemy;
    Rigidbody rigidbody;

    public JumpNode(float jumpForce, GameObject enemy)
    {
        this.jumpForce = jumpForce;
        this.enemy = enemy.GetComponent<AIController>();
        rigidbody = enemy.GetComponent<Rigidbody>();
    }

    public override NodeStates Evaluate()
    {
        if(enemy.OnGround())
        {
            rigidbody.velocity = new Vector3(0, jumpForce, 0);
            Debug.Log("JUMPJUMP");
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }
}