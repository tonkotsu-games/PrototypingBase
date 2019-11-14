using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Rigidbody rigidbody;
    private FreeLookCamera cam;

    private MovementCalculation calculator;
    private GravityArtificial gravityUpdate;

    private MovementUpdate moveUpdate;
    private HeadingUpdate headUpdate;

    public IdleState(Rigidbody rigidbody, FreeLookCamera cam, MovementCalculation calculator, GravityArtificial gravityUpdate, MovementUpdate moveUpdate, HeadingUpdate headUpdate)
    {
        this.rigidbody = rigidbody;
        this.cam = cam;
        this.calculator = calculator;
        this.gravityUpdate = gravityUpdate;
        this.moveUpdate = moveUpdate;
        this.headUpdate = headUpdate;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        moveUpdate.MoveUpdate(PlayerController.Stances.Idle, rigidbody, Vector3.zero, calculator.MoveVector, gravityUpdate.Gravity);
        headUpdate.Heading(calculator.Head, rigidbody.transform, cam);
    }

    public void Exit()
    {
        
    }
}