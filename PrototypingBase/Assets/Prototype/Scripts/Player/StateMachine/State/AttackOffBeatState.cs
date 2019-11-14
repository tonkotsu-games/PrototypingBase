using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOffBeatState : IState
{
    private Rigidbody rigidbody;
    private FreeLookCamera cam;
    private Animator animator;

    private MovementCalculation calculator;
    private GravityArtificial gravityUpdate;

    private MovementUpdate moveUpdate;
    private HeadingUpdate headUpdate;

    private float attackStrafe;
    private float attackStrafeUpdate;

    public AttackOffBeatState(Rigidbody rigidbody, FreeLookCamera cam, Animator animator, MovementCalculation calculator,
                       GravityArtificial gravityUpdate, MovementUpdate moveUpdate, HeadingUpdate headUpdate, float attackStrafe)
    {
        this.rigidbody = rigidbody;
        this.cam = cam;
        this.animator = animator;
        this.calculator = calculator;
        this.gravityUpdate = gravityUpdate;
        this.moveUpdate = moveUpdate;
        this.headUpdate = headUpdate;
        this.attackStrafe = attackStrafe;
    }

    public void Enter()
    {
        animator.SetTrigger("swordAttack");
        attackStrafeUpdate = attackStrafe;
    }

    public void Execute()
    {
        headUpdate.Heading(calculator.Head, rigidbody.transform, cam);
        moveUpdate.MoveUpdate(PlayerController.Stances.Attack, rigidbody, Vector3.zero, calculator.MoveVector, gravityUpdate.Gravity, attackStrafeUpdate);
    }

    public void Exit()
    {

    }
}