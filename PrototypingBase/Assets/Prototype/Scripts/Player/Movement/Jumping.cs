public class Jumping
{
    public enum JumpType { None, Normal, Air, Slide }
    private JumpType jumpTypeDisplay = JumpType.None;

    private float gravity = 0;
    private float jumpForce = 0;

    public float Gravity { get => gravity; }
    public float JumpForce { get => jumpForce; }
    public JumpType JumpTypeDisplay { get => jumpTypeDisplay; }

    public float Jump(bool inSliding, float jumpVelocity, float airJumpVelocity, float slideJumpVelocity, JumpType jumpState)
    {
        inSliding = false;
        if (jumpState == JumpType.Normal)
        {
            gravity = jumpVelocity;
            jumpForce = jumpVelocity;
        }
        else if (jumpState == JumpType.Air)
        {
            gravity = airJumpVelocity;
            jumpForce = airJumpVelocity;
        }
        else if (jumpState == JumpType.Slide)
        {
            gravity = slideJumpVelocity;
            jumpForce = slideJumpVelocity;
        }
        return gravity;
    }
}