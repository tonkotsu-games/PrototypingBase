using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastSlashState : IState
{
    BoxCollider hitBox;
    Vector3 startPosition;
    Rigidbody rigidbody;
    PlayerNiklas player;
    Animator animator;
    StateMachine stateMachine;
    Transform characterModel;

    public FastSlashState(GameObject player)
    {
        hitBox = player.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>();
        rigidbody = player.GetComponent<Rigidbody>();
        this.player = player.GetComponent<PlayerNiklas>();
        animator = player.GetComponentInChildren<Animator>();
        stateMachine = player.GetComponent<PlayerNiklas>().StateMachine;
        characterModel = player.transform.GetChild(0).gameObject.transform;
    }

    public void Enter()
    {
        startPosition = player.transform.position;
        hitBox.enabled = true;
        rigidbody.velocity = Vector3.zero;
        InitiateAttackMove();
        if (player.slashRight)
        {
            animator.SetTrigger("slash1");
            player.slashRight = !player.slashRight;
        }
        else
        {
            animator.SetTrigger("slash2");
            player.slashRight = !player.slashRight;
        }
    }

    public void Execute()
    {
        if (Vector3.Distance(player.transform.position, startPosition) > player.AttackStepDistance)
        {
            rigidbody.velocity = Vector3.zero;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            stateMachine.ChangeState(new MoveState(PlayerNiklas.Player));
        }
    }

    public void Exit()
    {
        hitBox.enabled = false;
    }

    private void InitiateAttackMove()
    {
        rigidbody.AddForce(characterModel.forward * player.StepSpeed, ForceMode.Impulse);
    }
}