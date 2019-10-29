using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUpdate
{
    public void MoveUpdate(PlayerController.Stances currentStance, Rigidbody rigidbody, Vector3 slideVelocity, Vector3 moveVector, float gravity)
    {
        if (currentStance == PlayerController.Stances.Slide)
        {
            rigidbody.velocity = slideVelocity;
        }
        else
        {
            rigidbody.velocity = new Vector3(moveVector.x,
                                             gravity,
                                             moveVector.z);
        }
    }
}