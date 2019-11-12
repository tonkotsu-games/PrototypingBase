using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCollector : MonoBehaviour
{
    private PlayerNiklas player;
    [SerializeField] GameObject mainCam;

    private void Start()
    {

        player = Locator.instance.GetPlayerGameObject().GetComponent<PlayerNiklas>();
    }

  //  public void AfterShoot()
  //  {
  //      player.ChangeStanceTo(PlayerController.Stances.Idle);
  //  }
  //
  //  public void AttackReset()
  //  {
  //      player.ChangeStanceTo(PlayerController.Stances.Idle);
  //  }
    public void IntroDone()
    {
        player.enabled = true;       
        mainCam.SetActive(true);
        gameObject.SetActive(false);
    }
    public void afterMeteorattack()
    {
        gameObject.GetComponent<Animator>().SetBool("meteorAttackBool", false);

    }

}