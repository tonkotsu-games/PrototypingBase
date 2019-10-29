using UnityEngine;

public class GravityArtificial
{
    private float gravity = 0;

    public float Gravity { get => gravity; set => gravity = value; }

    public void GravityUpdate(bool grounded, float gravityMax, float jumpGravity)
    {
        if (!grounded)
        {
            if (gravity > gravityMax)
            {
                gravity += jumpGravity * Time.deltaTime;
            }
            else
            {
                gravity = gravityMax;
            }
        }
    }
}