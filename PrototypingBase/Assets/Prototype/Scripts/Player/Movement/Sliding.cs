using UnityEngine;

public class Sliding
{
    private bool inSliding = false;

    private float currentSlideTime = 0;
    private float slidingSpeed = 0;

    private Vector3 slideVelocity = new Vector3(0, 0, 0);
    private Transform slideRotation = null;

    public bool InSliding { get => inSliding; set => inSliding = value; }

    public float CurrentSlideTime { get => currentSlideTime; set => currentSlideTime = value; }
    public float SlidingSpeed {set => slidingSpeed = value; }

    public Vector3 SlideVelocity { get => slideVelocity; }
    public Transform SlideRotation { get => slideRotation; }

    public void SlideUpdate(bool grounded, PlayerController.Stances lastStance, float gravity, float gravityMax, Vector3 heading, Transform player)
    {
        if (inSliding && grounded && lastStance != PlayerController.Stances.Jump)
        {
            currentSlideTime -= Time.deltaTime;
            if (CurrentSlideTime >= 0)
            {
                Slide(heading, player, gravity);
            }
            else
            {
                inSliding = false;
            }
        }
        else
        {
            slideVelocity = new Vector3(player.forward.x * slidingSpeed,
                                        gravityMax - 70f,
                                        player.forward.z * slidingSpeed);
        }
    }

    private void Slide(Vector3 heading, Transform player, float gravity)
    {
        slideVelocity = new Vector3(player.forward.x * slidingSpeed,
                                    gravity,
                                    player.forward.z * slidingSpeed);
    }
}