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
    [Header("Value for Jump")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float airJumpForce;
    [SerializeField]
    private float fallingSpeed;
    [SerializeField]
    private float artificialGravitySpeed;
    private float currentJumpHight;
    private float highestJumpHight;
    public float artificialGravity;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
    private bool reachedHighestPoint = false;
    private bool overJumpHight = false;
    private bool airParry = false;
    #endregion

    #region Slide
    [Header("Time for Slide")]
    [SerializeField]
    private float slideTime;
    private float currentSlideTime = 0;
    [Header("Value for Sliding Speed")]
    [SerializeField]
    private float slidingSpeed;

    private bool sliding = false;
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
    }

    void Update()
    {
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
        if (Input.GetButtonDown("Parry"))
        {
            lastStance = currentStance;
            currentStance = Stances.Parry;
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
        else if (currentStance == Stances.Parry)
        {

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
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
        if (move > 1)
        {
            move = 1;
        }
            moveVector = cam.transform.forward.normalized * Input.GetAxisRaw("Vertical") + cam.transform.right.normalized * Input.GetAxisRaw("Horizontal");
            moveVector = moveVector.normalized * move * movementSpeed;
            moveVector.y = 0;
    }

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        if (!grounded && rigi.velocity.y < 0)
        {
            rigi.velocity = new Vector3(moveVector.x,
                                        Vector3.up.y * Physics.gravity.y * fallingSpeed,
                                        moveVector.z);
        }
        else if (rigi.velocity.y > 0)
        {
            artificialGravity -= artificialGravitySpeed;

            rigi.velocity = new Vector3(moveVector.x,
                                        rigi.velocity.y + artificialGravity,
                                        moveVector.z);
        }
        else
        {
            rigi.velocity = new Vector3(moveVector.x,
                                        rigi.velocity.y,
                                        moveVector.z);
        }
    }

    private void Jump()
    {
        if (jump)
        {
            rigi.velocity = Vector3.up * jumpForce;
            jump = false;
        }
        else if (airJumping)
        {
            artificialGravity = 0;
            rigi.velocity = Vector3.up * airJumpForce;
            airJumping = false;
        }
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
                if (moveVector == Vector3.zero)
                {
                    transform.rotation = Quaternion.identity;

                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(moveVector);
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
                        case Stances.Parry:
                            {
                                anim.SetTrigger("parry");
                                AttackReset();
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
                                artificialGravity = 0;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                break;
                            }
                        case Stances.Jump:
                            {
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                artificialGravity = 0;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                rigi.velocity = new Vector3(transform.forward.x * 50,
                                                             Vector3.down.y * 100,
                                                             transform.forward.z);
                                currentSlideTime = slideTime;
                                sliding = true;
                                break;
                            }
                        case Stances.Attack:
                            {
                                artificialGravity = 0;
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                airAttack = true;
                                rigi.velocity = Vector3.down * 50;
                                Attack();
                                break;
                            }
                        case Stances.Parry:
                            {
                                airParry = true;
                                anim.SetTrigger("parry");
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
                        case Stances.Parry:
                            {
                                anim.SetTrigger("parry");
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
                        case Stances.Parry:
                            {
                                anim.SetTrigger("parry");
                                break;
                            }
                    }
                    break;
                }
            case Stances.Parry:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
                                if (airParry)
                                {
                                    reachedHighestPoint = false;
                                    airJumping = false;
                                    highestJumpHight = 0;
                                }
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (!airParry)
                                {
                                    jump = true;
                                }
                                airParry = false;
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
                                if (airParry)
                                {
                                    reachedHighestPoint = false;
                                    airJumping = false;
                                    highestJumpHight = 0;
                                    airAttack = true;
                                    rigi.velocity = Vector3.down * 50;
                                    airParry = false;
                                }
                                else
                                {
                                    groundAttack = true;
                                }
                                Attack();
                                break;
                            }
                        case Stances.Parry:
                            {
                                anim.SetTrigger("parry");
                                if (!airParry)
                                {
                                    lastStance = currentStance;
                                    currentStance = Stances.Ground;
                                }
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
        if (currentStance == Stances.Jump || airParry)
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