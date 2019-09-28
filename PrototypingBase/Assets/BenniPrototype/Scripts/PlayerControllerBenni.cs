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

    #region Movement
    [Header("Value for the Movement")]
    [SerializeField]
    private float movementSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private float move;

    private Vector3 moveVector;

    #region Jumping
    [Header("Value for Jump")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float airJumpForce;
    private float currentJumpHight;
    private float highestJumpHight;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
    private bool reachedHighestPoint = false;
    private bool overJumpHight = false;
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

        if (Input.GetButtonDown("Jump") && (grounded || lastStance == Stances.Jump))
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
    }

    private void FixedUpdate()
    {
        if (currentStance == Stances.Ground)
        {
            MovementCalculation();
            Move();
        }
        else if (currentStance == Stances.Jump)
        {
            MovementCalculation();
            Move();
            CalculateJumpHight();
        }
        else if (currentStance == Stances.Slide)
        {
            Slide();
        }
        else if (currentStance == Stances.Attack)
        {

        }
        else if (currentStance == Stances.Parry)
        {

        }
        else
        {
            Debug.LogError("WARNING NO STANCE");
        }
    }

    void MovementCalculation()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
        if (move > 1)
        {
            move = 1;
        }
        moveVector = new Vector3(moveHorizontal, 0, moveVertical);

        //moveVector = transform.forward * moveVector.x + transform.right * moveVector.z;

        moveVector = moveVector.normalized * move * movementSpeed;
    }

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        rigi.velocity = new Vector3(moveVector.x,
                        rigi.velocity.y,
                        moveVector.z);
    }

    private void Jump()
    {
        if (jump)
        {
            rigi.AddForce(Vector3.up * 10000 * jumpForce);
            jump = false;
        }
        else if (airJumping)
        {
            rigi.AddForce(Vector3.up * 10000 * airJumpForce);
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
            if (currentJumpHight < highestJumpHight - 1)
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
                rigi.velocity = Vector3.forward * slidingSpeed;
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
                    }
                    break;
                }
            case Stances.Jump:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
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
                                reachedHighestPoint = false;
                                airJumping = false;
                                highestJumpHight = 0;
                                currentSlideTime = slideTime;
                                sliding = true;
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
                                jump = true;
                                Jump();
                                break;
                            }
                        case Stances.Slide:
                            {
                                currentSlideTime = slideTime;
                                break;
                            }
                    }
                    break;
                }
        }
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
    }
}