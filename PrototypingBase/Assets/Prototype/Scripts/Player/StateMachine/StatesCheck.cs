using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatesCheck
{
    private Rigidbody rigidbody;
    private FreeLookCamera cam;
    private Animator animator;

    private StateMachine stateMachine;
    private MovementCalculation calculator;
    private GravityArtificial gravityUpdate;
    private MovementUpdate moveUpdate;
    private HeadingUpdate headUpdate;
    private Sliding sliding;

    private float gravityMax;

    private bool grounded;

    public bool Grounded { set => grounded = value; }

    public StatesCheck(Rigidbody rigidbody, FreeLookCamera cam, Animator animator, StateMachine stateMachine, MovementCalculation calculator,
                       GravityArtificial gravityUpdate, MovementUpdate moveUpdate, HeadingUpdate headUpdate, Sliding sliding, float gravityMax)
    {
        this.rigidbody = rigidbody;
        this.cam = cam;
        this.animator = animator;
        this.calculator = calculator;
        this.stateMachine = stateMachine;
        this.gravityUpdate = gravityUpdate;
        this.moveUpdate = moveUpdate;
        this.headUpdate = headUpdate;
        this.sliding = sliding;
    }

    public void StateToSubstateCheck(PlayerController.Stances currentStance, PlayerController.Stances previousStance, bool beat, float attackStrafe = 0)
    {
        switch (previousStance)
        {
            case PlayerController.Stances.Idle:
                {
                    switch (currentStance)
                    {
                        case PlayerController.Stances.Jump:
                            {
                                stateMachine.ChangeState(new JumpState(rigidbody, cam, animator, calculator,
                                                                       gravityUpdate, moveUpdate, headUpdate));
                                break;
                            }
                        case PlayerController.Stances.Slide:
                            {
                                stateMachine.ChangeState(new SlideState(rigidbody, animator, calculator, gravityUpdate, moveUpdate, sliding, gravityMax));
                                break;
                            }
                        case PlayerController.Stances.Attack:
                            {
                                if (beat)
                                {
                                    stateMachine.ChangeState(new AttackOnBeatState(rigidbody, cam.GetComponent<FreeLookCamera>(), animator, calculator,
                                                                                   gravityUpdate, moveUpdate, headUpdate, attackStrafe));
                                }
                                else
                                {
                                    stateMachine.ChangeState(new AttackOffBeatState(rigidbody, cam.GetComponent<FreeLookCamera>(), animator, calculator,
                                                                                    gravityUpdate, moveUpdate, headUpdate, attackStrafe));
                                }
                                break;
                            }
                        case PlayerController.Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
            case PlayerController.Stances.Jump:
                {
                    switch (currentStance)
                    {
                        case PlayerController.Stances.Idle:
                            {
                                stateMachine.ChangeState(new IdleState(rigidbody, cam, calculator, gravityUpdate, moveUpdate, headUpdate));
                                break;
                            }
                        case PlayerController.Stances.Jump:
                            {
                                break;
                            }
                        case PlayerController.Stances.Slide:
                            {
                                stateMachine.ChangeState(new SlideState(rigidbody, animator, calculator, gravityUpdate, moveUpdate, sliding, gravityMax));
                                break;
                            }
                        case PlayerController.Stances.Attack:
                            {
                                break;
                            }
                        case PlayerController.Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
            case PlayerController.Stances.Slide:
                {
                    switch (currentStance)
                    {
                        case PlayerController.Stances.Idle:
                            {
                                stateMachine.ChangeState(new IdleState(rigidbody, cam, calculator, gravityUpdate, moveUpdate, headUpdate));
                                break;
                            }
                        case PlayerController.Stances.Jump:
                            {
                                if (grounded)
                                {
                                    stateMachine.ChangeState(new JumpState(rigidbody, cam, animator, calculator,
                                                                           gravityUpdate, moveUpdate, headUpdate));
                                }
                                break;
                            }
                        case PlayerController.Stances.Slide:
                            {
                                stateMachine.ChangeState(new SlideState(rigidbody, animator, calculator, gravityUpdate, moveUpdate, sliding, gravityMax));
                                break;
                            }
                        case PlayerController.Stances.Attack:
                            {
                                break;
                            }
                        case PlayerController.Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
            case PlayerController.Stances.Attack:
                {
                    switch (currentStance)
                    {
                        case PlayerController.Stances.Idle:
                            {
                                stateMachine.ChangeState(new IdleState(rigidbody, cam, calculator, gravityUpdate, moveUpdate, headUpdate));
                                break;
                            }
                        case PlayerController.Stances.Jump:
                            {
                                stateMachine.ChangeState(new JumpState(rigidbody, cam, animator, calculator,
                                                                       gravityUpdate, moveUpdate, headUpdate));
                                break;
                            }
                        case PlayerController.Stances.Slide:
                            {
                                stateMachine.ChangeState(new SlideState(rigidbody, animator, calculator, gravityUpdate, moveUpdate, sliding, gravityMax));
                                break;
                            }
                        case PlayerController.Stances.Attack:
                            {
                                if (beat)
                                {
                                    stateMachine.ChangeState(new AttackOnBeatState(rigidbody, cam.GetComponent<FreeLookCamera>(), animator, calculator,
                                                gravityUpdate, moveUpdate, headUpdate, attackStrafe));
                                }
                                else
                                {
                                    stateMachine.ChangeState(new AttackOffBeatState(rigidbody, cam.GetComponent<FreeLookCamera>(), animator, calculator,
                                                                                    gravityUpdate, moveUpdate, headUpdate, attackStrafe));
                                }
                                break;
                            }
                        case PlayerController.Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
            case PlayerController.Stances.Gun:
                {
                    switch (currentStance)
                    {
                        case PlayerController.Stances.Idle:
                            {
                                break;
                            }
                        case PlayerController.Stances.Jump:
                            {
                                break;
                            }
                        case PlayerController.Stances.Slide:
                            {
                                break;
                            }
                        case PlayerController.Stances.Attack:
                            {
                                break;
                            }
                        case PlayerController.Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
        }
    }
}