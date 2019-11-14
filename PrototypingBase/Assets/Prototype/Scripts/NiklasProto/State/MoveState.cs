using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    float inputHorizontal;
    float inputVertical;
    float movementSpeed;
    Vector3 moveVector;
    Vector3 heading;
    PlayerNiklas player;
    Rigidbody rigidbody;
    GameObject charakterModell;
    Animator animator;

    public MoveState(GameObject player)
    {
        this.player = player.GetComponent<PlayerNiklas>();
        charakterModell = player.transform.GetChild(0).gameObject;
        animator = player.GetComponentInChildren<Animator>();
        rigidbody = player.GetComponent<Rigidbody>();
        movementSpeed = player.GetComponent<PlayerNiklas>().MovementSpeed;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        moveVector = new Vector3(inputHorizontal, 0, inputVertical);
        if (moveVector != Vector3.zero)
        {
            charakterModell.transform.rotation = Quaternion.LookRotation(moveVector);
        }
        rigidbody.velocity = moveVector * movementSpeed;
        animator.SetFloat("speed", rigidbody.velocity.magnitude);        
    }

    public void Exit()
    {

    }
}