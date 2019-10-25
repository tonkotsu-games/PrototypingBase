using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCollector : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void AfterShoot()
    {
        player.ChangeStanceTo(PlayerController.Stances.Idle);
    }

    public void AttackReset()
    {
        player.ChangeStanceTo(PlayerController.Stances.Idle);
    }

}