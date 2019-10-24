using UnityEngine;

public class MovementCalculation
{
    #region Jump
    private float jumpGravity = 0;
    private float airJumpGravity = 0;
    private float slideJumpGravity = 0;
    private float jumpVelocity = 0;
    private float airJumpVelocity = 0;
    private float slideJumpVelocity = 0;
    private float currentJumpHeight = 0;

    public float JumpGravity { get => jumpGravity; }
    public float AirJumpGravity { get => airJumpGravity; }
    public float SlideJumpGravity { get => slideJumpGravity; }
    public float JumpVelocity { get => jumpVelocity; }
    public float AirJumpVelocity { get => airJumpVelocity; }
    public float SlideJumpVelocity { get => slideJumpVelocity; }
    public float CurrentJumpHeight { get => currentJumpHeight; }

    //private float highestJumpHeight = 0;

    //private bool reachedHeighestPoint = false;
    //private bool overJumpHeight = false;
    #endregion

    #region Move
    private float move = 0;

    private Vector3 head = new Vector3(0, 0, 0);
    private Vector3 moveVector = new Vector3(0, 0, 0);

    public Vector3 Head { get => head; }
    public Vector3 MoveVector { get => moveVector; }
    #endregion

    public void Heading(float moveHorizontal, float moveVertical, float deadZone, Transform cam)
    {
        if (moveHorizontal < -deadZone ||
           moveHorizontal > deadZone ||
           moveVertical < -deadZone ||
           moveVertical > deadZone)
        {
            head = cam.transform.forward.normalized * moveVertical + cam.transform.right.normalized * moveHorizontal;
            head = head.normalized;
            head.y = 0;
        }
    }

    public void Movement(float moveHorizontal, float moveVertical, float deadZone, float movementSpeed, Transform cam)
    {
        if (moveHorizontal < -deadZone ||
            moveHorizontal > deadZone ||
            moveVertical < -deadZone ||
            moveVertical > deadZone)
        {
            move = new Vector2(moveHorizontal, moveVertical).magnitude;
            if (move > 1)
            {
                move = 1;
            }
            moveVector = cam.transform.forward.normalized * Input.GetAxisRaw("Vertical") + cam.transform.right.normalized * Input.GetAxisRaw("Horizontal");
            moveVector = moveVector.normalized * move * movementSpeed;
            moveVector.y = 0;
        }
        else
        {
            moveVector = Vector3.zero;
        }
    }

    public void Jump(float jumpHeight, float timeToHeight, float airJumpHeight, float timeToAirJumpHeight, float slideJumpHeight, float timeToSlideJumpHeight)
    {
        jumpGravity = -(2 * jumpHeight) / Mathf.Pow(timeToHeight, 2);
        airJumpGravity = -(2 * airJumpHeight) / Mathf.Pow(timeToAirJumpHeight, 2);
        slideJumpGravity = -(2 * slideJumpHeight) / Mathf.Pow(timeToSlideJumpHeight, 2);
        jumpVelocity = Mathf.Abs(jumpGravity) * timeToHeight;
        airJumpVelocity = Mathf.Abs(airJumpGravity) * timeToAirJumpHeight;
        slideJumpVelocity = Mathf.Abs(slideJumpGravity) * timeToSlideJumpHeight;
    }

    public void JumpHight(float startPosition, float positionY)
    {
        currentJumpHeight = startPosition - positionY;
        currentJumpHeight = Mathf.Abs(currentJumpHeight);
    }
}