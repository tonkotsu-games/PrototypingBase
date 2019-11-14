using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    private Rigidbody rigidbody;
    private FreeLookCamera cam;
    private Animator animator;

    private MovementCalculation calculator;
    private GravityArtificial gravityUpdate;

    private MovementUpdate moveUpdate;
    private HeadingUpdate headUpdate;

    public JumpState(Rigidbody rigidbody, FreeLookCamera cam, Animator animator, MovementCalculation calculator, 
                     GravityArtificial gravityUpdate, MovementUpdate moveUpdate, HeadingUpdate headUpdate)
    {
        this.rigidbody = rigidbody;
        this.cam = cam;
        this.animator = animator;
        this.calculator = calculator;
        this.gravityUpdate = gravityUpdate;
        this.moveUpdate = moveUpdate;
        this.headUpdate = headUpdate;
    }

    public void Enter()
    {
        animator.SetTrigger("jumping");
        gravityUpdate.Gravity = calculator.JumpVelocity;
    }

    public void Execute()
    {
        moveUpdate.MoveUpdate(PlayerController.Stances.Jump, rigidbody, Vector3.zero, calculator.MoveVector, gravityUpdate.Gravity);
        headUpdate.Heading(calculator.Head, rigidbody.transform, cam);
    }

    public void Exit()
    {

    }
}