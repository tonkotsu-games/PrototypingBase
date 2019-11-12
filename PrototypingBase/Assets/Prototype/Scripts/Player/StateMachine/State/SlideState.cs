using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideState : IState
{
    private Rigidbody rigidbody;
    private Animator animator;


    private MovementCalculation calculator;
    private GravityArtificial gravityUpdate;
    private MovementUpdate moveUpdate;
    private Sliding sliding;

    private float gravityMax;

    public SlideState(Rigidbody rigidbody, Animator animator, MovementCalculation calculator, GravityArtificial gravityUpdate, 
                      MovementUpdate moveUpdate, Sliding sliding, float gravityMax)
    {
        this.rigidbody = rigidbody;
        this.animator = animator;
        this.calculator = calculator;
        this.gravityUpdate = gravityUpdate;
        this.moveUpdate = moveUpdate;
        this.sliding = sliding;
        this.gravityMax = gravityMax;
    }

    public void Enter()
    {
        animator.SetTrigger("slide");
    }

    public void Execute()
    {
        sliding.SlideUpdate(gravityUpdate.Gravity, gravityMax, calculator.Head, rigidbody.transform);
        moveUpdate.MoveUpdate(PlayerController.Stances.Slide, rigidbody, sliding.SlideVelocity, Vector3.zero, gravityUpdate.Gravity);
    }

    public void Exit()
    {

    }
}