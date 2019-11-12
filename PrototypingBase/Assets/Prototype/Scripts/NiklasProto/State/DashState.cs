using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : IState
{
    Timer dashTimer = new Timer();
    Vector3 startPosition;
    PlayerNiklas player;
    Rigidbody rigidbody;
    StateMachine stateMachine;
    Transform characterModel;

    public DashState(GameObject player)
    {
        this.player = player.GetComponent<PlayerNiklas>();
        characterModel = player.transform.GetChild(0).gameObject.transform;
        rigidbody = player.GetComponent<Rigidbody>();
        stateMachine = player.GetComponent<PlayerNiklas>().StateMachine;
    }

    void IState.Enter()
    {
        Physics.IgnoreLayerCollision(10, 11, true);
        dashTimer.Set(0.2f);
        startPosition = player.transform.position;
        rigidbody.velocity = characterModel.forward * player.DashSpeed;
    }

    void IState.Execute()
    {
        dashTimer.Tick();
        if (Vector3.Distance(startPosition, player.transform.position) > player.DashDistance)
        {
            rigidbody.velocity = Vector3.zero;
            Physics.IgnoreLayerCollision(10, 11, false);
            stateMachine.ChangeState(new MoveState(PlayerNiklas.Player));
        }
        else
        {
            if (dashTimer.timeCurrent <= 0)
            {
                Physics.IgnoreLayerCollision(10, 11, false);
                stateMachine.ChangeState(new MoveState(PlayerNiklas.Player));
            }
        }
    }

    void IState.Exit()
    {

    }
}