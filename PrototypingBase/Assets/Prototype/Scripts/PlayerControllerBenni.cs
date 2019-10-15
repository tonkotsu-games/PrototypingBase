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
        Gun
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
    private float jumpHeight;
    [SerializeField]
    private float timeToHeight;

    [Header("Air Jump")]
    [SerializeField]
    private float airJumpHeight;
    [SerializeField]
    private float timeToAirJumpHeight;

    private float gravity;
    private float jumpVelocity;
    private float airJumpVelocity;
    private float jumpGravity;
    private float airJumpGravity;
    private float jumpTime;
    private float jumpParable;
    private float currentJumpHeight;
    private float highestJumpHeight;
    private float jumpForce = 0;
    private float startPosition = 0;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
    private bool reachedHeighestPoint = false;
    private bool overJumpHeight = false;
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

    #region Gun
    [Header("Gun Gameobjects")]
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject bullet;
    private GameObject instanceBullet;

    [SerializeField]
    private float bulletSpeed;

    private bool airGun = false;
    #endregion

    #endregion

    #region System
    private Animator anim;
    private Rigidbody rigi;
    private BeatAnalyse beat;

    private Stances currentStance;
    private Stances lastStance;

    [SerializeField]
    private FreeLookCamera target;
    #endregion

    void Start()
    {
        beat = GameObject.FindWithTag("Beat").GetComponent<BeatAnalyse>();
        anim = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        currentStance = Stances.Ground;
        lastStance = currentStance;
        jump = false;
        rigi = GetComponent<Rigidbody>();
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
            if(grounded)
            {
                startPosition = transform.position.y;
            }
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
        if (Input.GetButtonDown("Gun") && !airAttack)
        {
            lastStance = currentStance;
            currentStance = Stances.Gun;
            anim.SetTrigger("gun");

            SubStancesCheck();
        }
        if (currentStance == Stances.Jump || airGun)
        {
            CalculateJumpHight();
        }

    }

    private void FixedUpdate()
    {
        if (currentStance == Stances.Ground ||
            currentStance == Stances.Jump ||
            currentStance == Stances.Gun)
        {
            MovementCalculation();
            Heading();
            Move();
        }
        else if (currentStance == Stances.Slide)
        {
            Gravity();
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

        if ((moveHorizontal < -deadZone ||
           moveHorizontal > deadZone ||
           moveVertical < -deadZone ||
           moveVertical > deadZone) && !target.lockOn)
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
        else if (target.lockOn)
        {
            target.lookAt.y = 0;
            transform.rotation = Quaternion.LookRotation(target.lookAt);
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
            jumpForce = jumpVelocity;
            anim.SetTrigger("jumpFlip");
            jump = false;
        }
        else if (airJumping)
        {
            gravity = airJumpVelocity;
            jumpForce = airJumpVelocity;
            airJumping = false;
        }
    }

    private void CalculateJump()
    {
        jumpGravity = -(2 * jumpHeight) / Mathf.Pow(timeToHeight, 2);
        airJumpGravity = -(2 * airJumpHeight) / Mathf.Pow(timeToAirJumpHeight, 2);
        jumpVelocity = Mathf.Abs(jumpGravity) * timeToHeight;
        airJumpVelocity = Mathf.Abs(airJumpGravity) * timeToAirJumpHeight;
    }

    /// <summary>
    /// Calculating the Jump Hight for the Air jump.
    /// Maybe in the Update for better results.
    /// </summary>
    public void CalculateJumpHight()
    {
        currentJumpHeight = startPosition - transform.position.y;
        currentJumpHeight = Mathf.Abs(currentJumpHeight);

        if (currentJumpHeight > highestJumpHeight)
        {
            overJumpHeight = true;
        }

        if (!reachedHeighestPoint)
        {
            if (currentJumpHeight >= highestJumpHeight)
            {
                highestJumpHeight = currentJumpHeight;
            }
            else
            {
                airJumping = true;
                reachedHeighestPoint = true;
            }
        }
        else
        {
            if (currentJumpHeight <= highestJumpHeight && overJumpHeight)
            {
                overJumpHeight = false;
                airJumping = true;
            }
            if (currentJumpHeight < highestJumpHeight - 10)
            {
                highestJumpHeight = currentJumpHeight;
            }
        }
    }

    private void Slide()
    {
        if (sliding && (grounded || lastStance == Stances.Ground || lastStance == Stances.Slide || lastStance == Stances.Gun))
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
                                            gravity,
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
                        case Stances.Gun:
                            {

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
                                airGun = false;
                                airJumpingGravity = false;
                                slideJump = false;
                                reachedHeighestPoint = false;
                                airJumping = false;
                                highestJumpHeight = 0;
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
                                reachedHeighestPoint = false;
                                airJumping = false;
                                highestJumpHeight = 0;
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
                                reachedHeighestPoint = false;
                                airJumping = false;
                                highestJumpHeight = 0;
                                airAttack = true;
                                rigi.velocity = Vector3.down * 50;
                                Attack();
                                break;
                            }
                        case Stances.Gun:
                            {
                                airGun = true;
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
                                gravity = 0;
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
                                Heading();
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
                        case Stances.Gun:
                            {
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
                        case Stances.Gun:
                            {
                                break;
                            }
                    }
                    break;
                }
            case Stances.Gun:
                {
                    switch (currentStance)
                    {
                        case Stances.Ground:
                            {
                                gravity = 0;
                                airGun = false;
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (grounded)
                                {
                                    jump = true;
                                    Jump();
                                    airGun = false;
                                }
                                else
                                {
                                    airJumpingGravity = true;
                                    Jump();
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (!grounded)
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    reachedHeighestPoint = false;
                                    airJumping = false;
                                    highestJumpHeight = 0;
                                    rigi.velocity = new Vector3(transform.forward.x * 50,
                                                                 Vector3.down.y * 100,
                                                                 transform.forward.z);
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    gravity = 0;
                                }
                                else
                                {
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (!grounded)
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    reachedHeighestPoint = false;
                                    airJumping = false;
                                    highestJumpHeight = 0;
                                    airAttack = true;
                                    rigi.velocity = Vector3.down * 50;
                                    Attack();
                                }
                                else
                                {
                                    groundAttack = true;
                                    Attack();
                                }
                                break;
                            }
                    }
                    break;
                }
        }
    }

    private void Attack()
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

    public void Shoot()
    {
        instanceBullet = Instantiate(bullet, gun.transform.position, Quaternion.LookRotation(haeding));
        instanceBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
    }

    private void Gravity()
    {
        if (!grounded)
        {
            gravity += jumpGravity * Time.deltaTime;
            if (gravity <= -20)
            {
                gravity = -20;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentStance == Stances.Jump ||
            currentStance == Stances.Gun)
        {
            lastStance = currentStance;
            currentStance = Stances.Ground;
            SubStancesCheck();
        }
        gravity = 0;
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
        GUI.Label(new Rect(10, 70, 400, 40), "Highest jump: " + highestJumpHeight, style);
        GUI.Label(new Rect(10, 100, 400, 40), "Air Jump: " + airJumping, style);
        GUI.Label(new Rect(10, 130, 400, 40), "Reached Highest Point: " + reachedHeighestPoint, style);
        GUI.Label(new Rect(10, 160, 400, 40), "Grounded: " + grounded, style);
        GUI.Label(new Rect(10, 190, 400, 40), "Gravity: " + gravity, style);
        GUI.Label(new Rect(10, 220, 400, 40), "JumpForce: " + jumpForce, style);
        GUI.Label(new Rect(10, 250, 400, 40), "Slide Time: " + currentSlideTime, style);
    }
}