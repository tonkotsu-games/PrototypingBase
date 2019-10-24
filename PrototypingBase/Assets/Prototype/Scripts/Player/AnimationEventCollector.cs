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
        player.SubStancesCheck(PlayerController.Stances.Gun, PlayerController.Stances.Idle);
    }

    public void AttackReset()
    {
        player.SubStancesCheck(PlayerController.Stances.Attack, PlayerController.Stances.Idle);
    }

}