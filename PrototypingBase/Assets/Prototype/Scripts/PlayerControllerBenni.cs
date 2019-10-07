using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBenni : MonoBehaviour
{
    private enum Stances
    {
        Ground,
        Jump,
        Slide,
        Attack,
        Parry
    }

    #region Player

    #region Movement
    [Header("Movement Stick DeadZone")]
    [SerializeField]
    [Range(0, 1)]
    private float deadZone;
    [Header("Value for the Movement")]
    [SerializeField]
    private float movementSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private float move;

    private Vector3 haeding;
    private Vector3 moveVector;

    [SerializeField]
    private GameObject cam;
    #endregion

    #region Jumping
    [Header("If true Jump Values can changed while playing")]
    [SerializeField]
    bool jumpTest = false;

    [Header("New Jump Parable")]
    [SerializeField]
    private float jumpHight;
    [SerializeField]
    private float timeToHight;
    [Header("Air Jump")]
    [SerializeField]
    private float airJumpHight;
    [SerializeField]
    private float timeToAirJumpHight;
    private float gravity;
    private float jumpVelocity;
    private float airJumpVelocity;
    private float jumpGravity;
    private float airJumpGravity;
    private float jumpTime;
    private float jumpParable;
    private float currentJumpHight;
    private float highestJumpHight;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
    private bool reachedHighestPoint = false;
    private bool overJumpHight = false;
    private bool airJumpingGravity = false;
    #endregion

    #region Slide
    [Header("Time for Slide")]
    [SerializeField]
    private float slideTime;
    private float currentSlideTime = 0;
    [Header("Value for Sliding Speed")]
    [SerializeField]
    private float slidingSpeed;
    [Header("Value for Sliding Jump Speed")]
    [SerializeField]
    private float slidingJumpSpeed;

    private bool sliding = false;
    private bool slideJump = false;
    #endregion

    #region Attack
    private bool groundAttack = false;
    private bool airAttack = false;

    #endregion

    #endregion

    #region System
    private Animator anim;
    private Rigidbody rigi;

    private Stances currentStance;
    private Stances lastStance;

    #endregion

    void Start()
    {
        currentStance = Stances.Ground;
        lastStance = currentStance;
        jump = false;
        rigi = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        if (!jumpTest)
        {
            CalculateJump();

        }
    }

    void Update()
    {
        if (jumpTest)
        {
            CalculateJump();
        }

        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            lastStance = currentStance;
            currentStance = Stances.Jump;
            SubStancesCheck();
        }
        if (Input.GetButtonDown("Slide"))
        {
            lastStance = currentStance;
            currentStance = Stances.Slide;
            SubStancesCheck();
        }
        if (Input.GetButtonDown("Attack"))
        {
            lastStance = currentStance;
            currentStance = Stances.Attack;
            SubStancesCheck();
        }
        if (currentStance == Stances.Jump)
        {
            CalculateJumpHight();
        }
    }

    private void FixedUpdate()
    {
        if (currentStance == Stances.Ground)
        {
            MovementCalculation();
            Heading();
            Move();
        }
        else if (currentStance == Stances.Jump)
        {
            MovementCalculation();
            Heading();
            Move();
        }
        else if (currentStance == Stances.Slide)
        {
            MovementCalculation();
            Heading();
            Slide();
        }
        else if (currentStance == Stances.Attack)
        {
            if (grounded)
            {
                if (airAttack)
                {
                    anim.SetTrigger("meteor");
                    airAttack = false;
                    lastStance = currentStance;
                    currentStance = Stances.Ground;
                    SubStancesCheck();
                }
            }
        }
        else
        {
            Debug.LogError("WARNING NO STANCE");
        }
    }

    private void Heading()
    {

        if (moveHorizontal < -0.3f ||
           moveHorizontal > 0.3f ||
           moveVertical < -0.3 ||
           moveVertical > 0.3)
        {
            haeding = cam.transform.forward.normalized * Input.GetAxisRaw("Vertical") + cam.transform.right.normalized * Input.GetAxisRaw("Horizontal");
            haeding = haeding.normalized;
            haeding.y = 0;


            if (haeding == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;

            }
            else
            {
                transform.rotation = Quaternion.LookRotation(haeding);
            }
        }
    }

    void MovementCalculation()
    {
        if (moveHorizontal < -deadZone ||
            moveHorizontal > deadZone ||
            moveVertical < -deadZone ||
            moveVertical > deadZone)
        {
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
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

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        if (!grounded)
        {
            if (!airJumpingGravity)
            {
                gravity += jumpGravity * Time.deltaTime;
            }
            else
            {
                gravity += airJumpGravity * Time.deltaTime;
            }
        }

        if (!slideJump)
        {
            rigi.velocity = new Vector3(moveVector.x,
                                gravity,
                                moveVector.z);
        }
        else
        {
            rigi.velocity = new Vector3(transform.forward.x * slidingJumpSpeed,
                                        gravity,
                                        transform.forward.z * slidingJumpSpeed);
        }
    }

    private void Jump()
    {
        if (jump)
        {
            gravity = jumpVelocity;

            jump = false;
        }
        else if (airJumping)
        {
            gravity = airJumpVelocity;

            airJumping = false;
        }
    }

    private void CalculateJump()
    {
        jumpGravity = -(2 * jumpHight) / Mathf.Pow(timeToHight, 2);
        airJumpGravity = -(2 * airJumpHight) / Mathf.Pow(timeToAirJumpHight, 2);
        jumpVelocity = Mathf.Abs(jumpGravity) * timeToHight;
        airJumpVelocity = Mathf.Abs(airJumpGravity) * timeToAirJumpHight;
    }

    /// <summary>
    /// Calculating the Jump Hight for the Air jump.
    /// Maybe in the Update for better results.
    /// </summary>
    public void CalculateJumpHight()
    {
        currentJumpHight = transform.position.y;

        if (currentJumpHight > highestJumpHight)
        {
            overJumpHight = true;
        }

        if (!reachedHighestPoint)
        {
            if (currentJumpHight >= highestJumpHight)
            {
                highestJumpHight = currentJumpHight;
            }
            else
            {
                airJumping = true;
                reachedHighestPoint = true;
            }
        }
        else
        {
            if (currentJumpHight <= highestJumpHight && overJumpHight)
            {
                overJumpHight = false;
                airJumping = true;
            }
            if (currentJumpHight < highestJumpHight - 10)
            {
                highestJumpHight = currentJumpHight;
            }
        }
    }

    private void Slide()
    {
        if (sliding && grounded)
        {
            currentSlideTime -= Time.deltaTime;
            if (currentSlideTime > 0)
            {
                if (haeding == Vector3.zero)
                {
                    transform.rotation = Quaternion.identity;

                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(haeding);
                }

                rigi.velocity = new Vector3(transform.forward.x * slidingSpeed,
                                            rigi.velocity.y,
                                            transform.forward.z * slidingSpeed);
            }
            else
            {
                sliding = false;
                lastStance = currentStance;
                currentStance = Stances.Ground;
            }
        }
    }

    /// <summary>
    /// Checking the Substance for different results when you come in different Stances.
    /// You check first the Last stance you where then the current stance.
    /// Every substance brings you to another result.
    /// </summary>
    private void SubStancesCheck()
    {
        switch (lastStance)
        {
            case Stances.Ground:
                {
                    switch (currentStance)
                    {
                        case Stances.Jump:
                            {
                                jump = true;
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                currentSlideTime = slideTime;
                                sliding = true;
                                break;
                            }
                        case Stances.Attack:
                            {
                                groundAttack = true;
                                Attack();
                                break;
                            }
                    }
                    break;
                }
            case Stances.Jump:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
                                airJumpingGravity = false;
                                slideJump = false;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                gravity = 0;
                                break;
                            }
                        case Stances.Jump:
                            {
                                slideJump = false;
                                airJumpingGravity = true;
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                airJumpingGravity = false;
                                slideJump = false;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                rigi.velocity = new Vector3(transform.forward.x * 50,
                                                             Vector3.down.y * 100,
                                                             transform.forward.z);
                                currentSlideTime = slideTime;
                                sliding = true;
                                gravity = 0;
                                break;
                            }
                        case Stances.Attack:
                            {
                                airJumpingGravity = false;
                                slideJump = false;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                airAttack = true;
                                rigi.velocity = Vector3.down * 50;
                                Attack();
                                break;
                            }
                    }
                    break;
                }
            case Stances.Slide:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
                                break;
                            }
                        case Stances.Jump:
                            {
                                sliding = false;
                                if (grounded)
                                {
                                    slideJump = true;
                                    jump = true;
                                }
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                currentSlideTime = slideTime;
                                break;
                            }
                        case Stances.Attack:
                            {
                                sliding = false;
                                rigi.velocity = Vector3.zero;
                                groundAttack = true;
                                Attack();
                                break;
                            }
                    }
                    break;
                }
            case Stances.Attack:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (grounded)
                                {
                                    jump = true;
                                }
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                currentSlideTime = slideTime;
                                break;
                            }
                        case Stances.Attack:
                            {
                                groundAttack = true;
                                Attack();
                                break;
                            }
                    }
                    break;
                }
        }
    }

    public void Attack()
    {
        if (groundAttack)
        {
            anim.SetTrigger("groundAttack");
            groundAttack = false;
        }
        else if (airAttack)
        {
            anim.SetTrigger("airAttack");
        }
    }

    public void AttackReset()
    {
        lastStance = currentStance;
        currentStance = Stances.Ground;
        SubStancesCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentStance == Stances.Jump)
        {
            lastStance = currentStance;
            currentStance = Stances.Ground;
            SubStancesCheck();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            grounded = false;
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(10, 10, 400, 40), "Current Stance: " + currentStance, style);
        GUI.Label(new Rect(10, 40, 400, 40), "Last Stance: " + lastStance, style);
        GUI.Label(new Rect(10, 70, 400, 40), "Highest jump: " + highestJumpHight, style);
        GUI.Label(new Rect(10, 100, 400, 40), "Air Jump: " + airJumping, style);
        GUI.Label(new Rect(10, 130, 400, 40), "Reached Highest Point: " + reachedHighestPoint, style);
        GUI.Label(new Rect(10, 160, 400, 40), "Grounded: " + grounded, style);
    }
}