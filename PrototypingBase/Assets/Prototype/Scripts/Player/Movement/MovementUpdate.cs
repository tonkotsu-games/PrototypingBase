using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUpdate
{
    public void MoveUpdate(PlayerController.Stances currentStance, Rigidbody rigidbody, Vector3 slideVelocity, Vector3 moveVector, float gravity, float attackStrafe = 0)
    {
        if (currentStance == PlayerController.Stances.Slide)
        {
            rigidbody.velocity = slideVelocity;
        }
        else if(currentStance == PlayerController.Stances.Attack)
        {
            if(attackStrafe >= 0)
            {
                attackStrafe -= Time.deltaTime;
                if(attackStrafe <= 0)
                {
                    attackStrafe = 0;
                }
            }
            rigidbody.velocity = new Vector3(moveVector.x * attackStrafe,
                                 0,
                                 moveVector.z * attackStrafe);
        }
        else
        {
            rigidbody.velocity = new Vector3(moveVector.x,
                                             gravity,
                                             moveVector.z);
        }
    }
}