using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    private MovementCalculation calculator = new MovementCalculation();
    private MovementUpdate moveUpdate = new MovementUpdate();
    private Jumping jump = new Jumping();
    private HeadingUpdate headingUpdate = new HeadingUpdate();
    private GravityArtificial gravityUpdate = new GravityArtificial();

    private Animator animator = null;

    private bool inSliding = false;
    private bool grounded = false;

    private float maxJumpHeight = 0;

    public JumpState(bool inSliding, bool grounded, float maxJumpHeight, Animator animator)
    {
        this.inSliding = inSliding;
        this.grounded = grounded;
        this.maxJumpHeight = maxJumpHeight;
        this.animator = animator;
    }

    public void Enter(IState previousState)
    {
        //switch(previousState.ToString())
        //{
        //    case "IdleState":
        //        {
        //            gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
        //            break;
        //        }
        //    case "JumpState":
        //        {
        //            if (calculator.CurrentJumpHeight < maxJumpHeight)
        //            {
        //                if (beat.IsOnBeat(reactionTime, timeWindow))
        //                {
        //                    //invulnerable
        //                }
        //                animator.SetTrigger("airJump");
        //                gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Air);
        //            }
        //            break;
        //        }
        //    case "SlideState":
        //        {
        //            if (beat.IsOnBeat(reactionTime, timeWindow))
        //            {
        //                if (grounded)
        //                {
        //                    animator.SetTrigger("jumping");
        //                    gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Slide);
        //                }
        //            }
        //            else
        //            {
        //                if (grounded)
        //                {
        //                    animator.SetTrigger("jumping");
        //                    gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
        //                }
        //            }
        //            break;
        //        }
        //    case "AttackState":
        //        {
        //            if (beat.IsOnBeat(reactionTime, timeWindow))
        //            {
        //                //invulnerable
        //            }
        //            gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
        //            break;
        //        }
        //    case "GunState":
        //        {
        //            if (beat.IsOnBeat(reactionTime, timeWindow))
        //            {
        //                //invulnerable
        //            }
        //
        //            if (grounded)
        //            {
        //                gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
        //            }
        //            else
        //            {
        //                gravityUpdate.Gravity = jump.Jump(inSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Air);
        //            }
        //            break;
        //        }
        //}
    }

    public void Execute()
    {
        //calculator.CalculateHeading(horizontalInput, verticalInput, deadZone, cam.transform);
        //calculator.CalcualteMovement(horizontalInput, verticalInput, deadZone, movementSpeed, cam.transform);        
        //headingUpdate.Heading();
        //moveUpdate.MoveUpdate();
        //gravityUpdate.GravityUpdate();
    }

    public void Exit()
    {

    }
}