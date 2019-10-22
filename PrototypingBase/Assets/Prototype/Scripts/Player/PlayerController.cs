using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Stances
    {
        Idle,
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
    private float deadZone = 0.1f;
    [Header("Value for the Movement")]
    [SerializeField]
    private float movementSpeed = 0;
    private float moveHorizontal = 0;
    private float moveVertical = 0;
    private float move = 0;

    private Vector3 haeding = new Vector3(0, 0, 0);
    private Vector3 moveVector = new Vector3(0, 0, 0);
    #endregion

    #region Jumping
    [Header("If true Jump Values can changed while playing")]
    [SerializeField]
    bool jumpTest = false;

    [Header("New jump parable")]
    [SerializeField]
    private float jumpHeight = 0;
    [SerializeField]
    private float timeToHeight = 0;

    [Header("Air jump")]
    [SerializeField]
    private float airJumpHeight = 0;
    [SerializeField]
    private float timeToAirJumpHeight = 0;

    [Header("Slide jump")]
    [SerializeField]
    private float slideJumpHeight = 0;
    [SerializeField]
    private float timeToSlideJumpHeight = 0;

    [Header("Max. Jump Height")]
    [SerializeField]
    private float maxJumpHeight = 0;

    [Header("Max. gravity. Negative value needed")]
    [SerializeField]
    private float gravityMax = 0;

    private float gravity = 0;
    private float jumpVelocity = 0;
    private float airJumpVelocity = 0;
    private float slideJumpVelocity = 0;
    private float jumpGravity = 0;
    private float airJumpGravity = 0;
    private float slideJumpGravity = 0;
    private float currentJumpHeight = 0;
    //private float highestJumpHeight = 0;
    private float jumpForce = 0;
    private float startPosition = 0;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
    //private bool reachedHeighestPoint = false;
    //private bool overJumpHeight = false;
    private bool airJumpingGravity = false;
    #endregion

    #region Slide
    [Header("Time for Slide")]
    [SerializeField]
    private float slideTime = 0;
    [SerializeField]
    private float slideAttackTime = 0;
    private float currentSlideTime = 0;
    [Header("Value for Sliding Speed")]
    [SerializeField]
    private float slidingSpeed = 0;

    private bool sliding = false;
    private bool slideJump = false;
    #endregion

    #region Attack
    private bool groundAttack = false;
    private bool airAttack = false;
    #endregion

    #region Gun
    private bool airGun = false;
    #endregion
    #endregion

    #region System
    [Header("Beatanalyse")]
    [SerializeField]
    private BeatAnalyse beat = null;
    private Animator anim = null;
    private Rigidbody rigi = null;
    private CapsuleCollider bodyCollider = null;
    private Timer lockTimer = new Timer();
    [SerializeField]
    private GameObject cam = null;
    [SerializeField]
    private FreeLookCamera target = null;

    [Range(0, 100)]
    [SerializeField]
    private float beatTimeLockPercent = 0;
    private float beatTimeLockPercentOld = 0;
    private float lockTime = 0;

    [Header("Reaction time for Player")]
    [SerializeField]
    private int reactionTime = 0;
    [Header("Time window for beat")]
    [SerializeField]
    private int timeWindow = 0;

    private Stances currentStance = Stances.Idle;
    private Stances lastStance = Stances.Idle;

    private bool debugMode = false;
    #endregion

    void Start()
    {
        anim = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider>();
        rigi = GetComponent<Rigidbody>();
        if (!jumpTest)
        {
            CalculateJump();
        }
        lockTimer.CountingDown = true;
        LockTimeCalculate(beatTimeLockPercent);
    }

    void Update()
    {
        if(beatTimeLockPercentOld != beatTimeLockPercent)
        {
            beatTimeLockPercentOld = beatTimeLockPercent;
            LockTimeCalculate(beatTimeLockPercent);
        }

        if (jumpTest)
        {
            CalculateJump();
        }

        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        if(lockTimer.timeCurrent <= 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                lastStance = currentStance;
                currentStance = Stances.Jump;
                if (grounded)
                {
                    startPosition = transform.position.y;
                }
                else
                {
                    airJumping = true;
                }
                SubStancesCheck(lastStance, currentStance);
                lockTimer.ResetTimer();
            }

            if (Input.GetButtonDown("Attack"))
            {
                lastStance = currentStance;
                currentStance = Stances.Attack;
                SubStancesCheck(lastStance, currentStance);
                lockTimer.ResetTimer();

            }

            if (Input.GetButtonDown("Gun") && !airAttack)
            {
                lastStance = currentStance;
                currentStance = Stances.Gun;
                SubStancesCheck(lastStance, currentStance);
                lockTimer.ResetTimer();
            }
        }
        else
        {
            lockTimer.Tick();
        }

        if (Input.GetButtonDown("Slide"))
        {
            lastStance = currentStance;
            currentStance = Stances.Slide;
            SubStancesCheck(lastStance, currentStance);
        }

        if (currentStance == Stances.Jump || airGun)
        {
            CalculateJumpHight();
        }

        if(Input.GetKeyDown(KeyCode.F4))
        {
            debugMode = !debugMode;
        }
    }

    private void FixedUpdate()
    {
        if (currentStance == Stances.Idle ||
            currentStance == Stances.Jump ||
            currentStance == Stances.Gun)
        {
            MovementCalculation();
            Heading();
            if (!airGun)
            {
                Move();
                Gravity();
            }
            else if (airGun && grounded)
            {
                anim.SetTrigger("gun");
                airGun = false;
            }
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
                    currentStance = Stances.Idle;
                    SubStancesCheck(lastStance, currentStance);
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
           moveVertical > deadZone) &&
           !target.lockOn)
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
        rigi.velocity = new Vector3(moveVector.x,
                                    gravity,
                                    moveVector.z);
    }

    private void Jump()
    {
        if (jump)
        {
            gravity = jumpVelocity;
            jumpForce = jumpVelocity;
            jump = false;
        }
        else if (airJumping && !airGun)
        {
            gravity = airJumpVelocity;
            jumpForce = airJumpVelocity;
            anim.SetTrigger("jumpFlip");
            airJumping = false;
        }
        else if (slideJump)
        {
            gravity = slideJumpVelocity;
            jumpForce = slideJumpVelocity;
            slideJump = false;
        }
        else if (airGun)
        {
            rigi.velocity = new Vector3(0,
                                        0,
                                        0);
            anim.SetTrigger("gun");
        }
    }

    private void CalculateJump()
    {
        jumpGravity = -(2 * jumpHeight) / Mathf.Pow(timeToHeight, 2);
        airJumpGravity = -(2 * airJumpHeight) / Mathf.Pow(timeToAirJumpHeight, 2);
        slideJumpGravity = -(2 * slideJumpHeight) / Mathf.Pow(timeToSlideJumpHeight, 2);
        jumpVelocity = Mathf.Abs(jumpGravity) * timeToHeight;
        airJumpVelocity = Mathf.Abs(airJumpGravity) * timeToAirJumpHeight;
        slideJumpVelocity = Mathf.Abs(slideJumpGravity) * timeToSlideJumpHeight;
    }

    /// <summary>
    /// Calculating the Jump Hight for the Air jump.
    /// Maybe in the Update for better results.
    /// </summary>
    public void CalculateJumpHight()
    {
        currentJumpHeight = startPosition - transform.position.y;
        currentJumpHeight = Mathf.Abs(currentJumpHeight);

        if(currentJumpHeight > maxJumpHeight)
        {
            rigi.transform.position = new Vector3(transform.position.x, startPosition + maxJumpHeight, transform.position.z);
        }

        ///<summary>
        /// Old Calculation for the double jump
        /// 
        /// if (currentJumpHeight > highestJumpHeight)
        /// {
        ///     overJumpHeight = true;
        /// }
        /// 
        /// if (!reachedHeighestPoint)
        /// {
        ///     if (currentJumpHeight >= highestJumpHeight)
        ///     {
        ///         highestJumpHeight = currentJumpHeight;
        ///     }
        ///     else
        ///     {
        ///         airJumping = true;
        ///         reachedHeighestPoint = true;
        ///     }
        /// }
        /// else
        /// {
        ///     if (currentJumpHeight <= highestJumpHeight && overJumpHeight)
        ///     {
        ///         overJumpHeight = false;
        ///         airJumping = true;
        ///     }
        ///     if (currentJumpHeight < highestJumpHeight - 10)
        ///     {
        ///         highestJumpHeight = currentJumpHeight;
        ///     }
        /// }
        ///</ summary >
    }

    private void Slide()
    {
        if (sliding && (grounded || lastStance == Stances.Idle || lastStance == Stances.Slide || lastStance == Stances.Gun))
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
                bodyCollider.enabled = true;
                lastStance = currentStance;
                currentStance = Stances.Idle;
            }
        }
    }

    /// <summary>
    /// Checking the Substance for different results when you come in different Stances.
    /// You check first the Last stance you where then the current stance.
    /// Every substance brings you to another result.
    /// </summary>
    public void SubStancesCheck(Stances lastStanceCheck, Stances currentStanceCheck)
    {
        if (currentStance != currentStanceCheck)
        {
            lastStance = lastStanceCheck;
            currentStance = currentStanceCheck;
        }

        switch (lastStanceCheck)
        {
            case Stances.Idle:
                {
                    switch (currentStanceCheck)
                    {
                        case Stances.Jump:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    jump = true;
                                    Jump();
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    jump = true;
                                    Jump();
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    bodyCollider.enabled = true;
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    groundAttack = true;
                                    Attack();
                                }
                                else
                                {
                                    groundAttack = true;
                                    Attack();
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gun");
                                }
                                else
                                {
                                    anim.SetTrigger("gun");
                                }
                                break;
                            }
                    }
                    break;
                }
            case Stances.Jump:
                {
                    switch (currentStanceCheck)
                    {
                        case Stances.Idle:
                            {
                                airGun = false;
                                airJumpingGravity = false;
                                slideJump = false;
                                //reachedHeighestPoint = false;
                                airJumping = false;
                                //highestJumpHeight = 0;
                                gravity = 0;
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    bodyCollider.enabled = false;
                                    slideJump = false;
                                    airJumpingGravity = true;
                                    Jump();
                                }
                                else
                                {
                                    bodyCollider.enabled = true;
                                    slideJump = false;
                                    airJumpingGravity = true;
                                    Jump();
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    rigi.velocity = new Vector3(transform.forward.x * 50,
                                                                 gravityMax,
                                                                 transform.forward.z);
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    gravity = 0;
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    rigi.velocity = new Vector3(transform.forward.x * 50,
                                                                 gravityMax,
                                                                 transform.forward.z);
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    gravity = 0;
                                    bodyCollider.enabled = true;
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    airAttack = true;
                                    rigi.velocity = new Vector3(0,
                                                                gravityMax,
                                                                0);
                                    Attack();
                                }
                                else
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    airAttack = true;
                                    rigi.velocity = new Vector3(0,
                                                                gravityMax,
                                                                0);
                                    Attack();
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    Debug.Log("Gun on beat");
                                    airGun = true;
                                    Jump();
                                }
                                else
                                {
                                    Debug.Log("Gun off beat");
                                    airGun = true;
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    rigi.velocity = new Vector3(0,
                                                                gravityMax,
                                                                0);
                                }
                                break;
                            }
                    }
                    break;
                }
            case Stances.Slide:
                {
                    switch (currentStanceCheck)
                    {
                        case Stances.Idle:
                            {
                                gravity = 0;
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    if (grounded)
                                    {
                                        sliding = false;
                                        slideJump = true;
                                        Jump();
                                    }
                                }
                                else
                                {
                                    if (grounded)
                                    {
                                        sliding = false;
                                        jump = true;
                                        Jump();
                                    }
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    bodyCollider.enabled = false;
                                    Heading();
                                    currentSlideTime = slideTime;
                                }
                                else
                                {
                                    bodyCollider.enabled = true;
                                    Heading();
                                    currentSlideTime = slideTime;
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    slideAttackTime = currentSlideTime;
                                    groundAttack = true;
                                    Attack();
                                }
                                else
                                {
                                    sliding = false;
                                    rigi.velocity = Vector3.zero;
                                    groundAttack = true;
                                    Attack();
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gun");
                                }
                                else
                                {
                                    anim.SetTrigger("gun");
                                }
                                break;
                            }
                    }
                    break;
                }
            case Stances.Attack:
                {
                    switch (currentStanceCheck)
                    {
                        case Stances.Idle:
                            {
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    jump = true;
                                    Jump();
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    jump = true;
                                    Jump();
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    bodyCollider.enabled = true;
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    groundAttack = true;
                                    Attack();
                                }
                                else
                                {
                                    groundAttack = true;
                                    Attack();
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gun");
                                }
                                else
                                {
                                    anim.SetTrigger("gun");
                                }
                                break;
                            }
                    }
                    break;
                }
            case Stances.Gun:
                {
                    switch (currentStanceCheck)
                    {
                        case Stances.Idle:
                            {
                                gravity = 0;
                                airGun = false;
                                break;
                            }
                        case Stances.Jump:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    if (grounded)
                                    {
                                        bodyCollider.enabled = false;
                                        jump = true;
                                        Jump();
                                    }
                                    else
                                    {
                                        bodyCollider.enabled = false;
                                        airJumpingGravity = true;
                                        airGun = false;
                                        Jump();
                                    }
                                }
                                else
                                {
                                    if (grounded)
                                    {
                                        bodyCollider.enabled = true;
                                        jump = true;
                                        Jump();
                                    }
                                    else
                                    {
                                        airJumpingGravity = true;
                                        airGun = false;
                                        Jump();
                                    }
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    if (!grounded)
                                    {
                                        bodyCollider.enabled = false;
                                        airJumpingGravity = false;
                                        slideJump = false;
                                        //reachedHeighestPoint = false;
                                        airJumping = false;
                                        //highestJumpHeight = 0;
                                        rigi.velocity = new Vector3(transform.forward.x * 50,
                                                                     Vector3.down.y * 100,
                                                                     transform.forward.z);
                                        currentSlideTime = slideTime;
                                        sliding = true;
                                        gravity = 0;
                                    }
                                    else
                                    {
                                        bodyCollider.enabled = false;
                                        currentSlideTime = slideTime;
                                        sliding = true;
                                    }

                                }
                                else
                                {
                                    if (!grounded)
                                    {
                                        bodyCollider.enabled = true;
                                        airJumpingGravity = false;
                                        slideJump = false;
                                        //reachedHeighestPoint = false;
                                        airJumping = false;
                                        //highestJumpHeight = 0;
                                        rigi.velocity = new Vector3(transform.forward.x * 50,
                                                                     Vector3.down.y * 100,
                                                                     transform.forward.z);
                                        currentSlideTime = slideTime;
                                        sliding = true;
                                        gravity = 0;
                                    }
                                    else
                                    {
                                        bodyCollider.enabled = true;
                                        currentSlideTime = slideTime;
                                        sliding = true;
                                    }
                                }
                                break;
                            }
                        case Stances.Attack:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    if (!grounded)
                                    {
                                        airJumpingGravity = false;
                                        slideJump = false;
                                        //reachedHeighestPoint = false;
                                        airJumping = false;
                                        //highestJumpHeight = 0;
                                        airAttack = true;
                                        rigi.velocity = Vector3.down * 50;
                                        Attack();
                                    }
                                    else
                                    {
                                        groundAttack = true;
                                        Attack();
                                    }
                                }
                                else
                                {
                                    if (!grounded)
                                    {
                                        airJumpingGravity = false;
                                        slideJump = false;
                                        //reachedHeighestPoint = false;
                                        airJumping = false;
                                        //highestJumpHeight = 0;
                                        airAttack = true;
                                        rigi.velocity = Vector3.down * 50;
                                        Attack();
                                    }
                                    else
                                    {
                                        groundAttack = true;
                                        Attack();
                                    }
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

    private void Gravity()
    {
        if (!grounded)
        {
            if (gravity > gravityMax)
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
            else
            {
                gravity = gravityMax;
            }
        }
    }

    private void LockTimeCalculate(float percent)
    {
        lockTime = (beat.sampleTimeInSec * percent) / 100;
        lockTimer.Set(lockTime);
        lockTimer.timeCurrent = 0;
        //Debug.Log(lockTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentStance == Stances.Jump)
        {
            lastStance = currentStance;
            currentStance = Stances.Idle;
            SubStancesCheck(lastStance, currentStance);
        }
        else if (currentStance == Stances.Slide)
        {
            lastStance = currentStance;
        }
        bodyCollider.enabled = true;
        airGun = false;
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
        if (debugMode)
        {

            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.red;

            GUI.Label(new Rect(10, 10, 400, 40), "Current Stance: " + currentStance, style);
            GUI.Label(new Rect(10, 40, 400, 40), "Last Stance: " + lastStance, style);
            //GUI.Label(new Rect(10, 70, 400, 40), "Highest jump: " + highestJumpHeight, style);
            GUI.Label(new Rect(10, 100, 400, 40), "Air Jump: " + airJumping, style);
            //GUI.Label(new Rect(10, 130, 400, 40), "Reached Highest Point: " + reachedHeighestPoint, style);
            GUI.Label(new Rect(10, 160, 400, 40), "Grounded: " + grounded, style);
            GUI.Label(new Rect(10, 190, 400, 40), "Gravity: " + gravity, style);
            GUI.Label(new Rect(10, 220, 400, 40), "JumpForce: " + jumpForce, style);
            GUI.Label(new Rect(10, 250, 400, 40), "Slide Time: " + currentSlideTime, style);
        }

        if(beat == null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 80;
            style.normal.textColor = Color.red;

            GUI.Label(new Rect(300, 200, 400, 160), "Beatdeatection is null", style);
        }
    }
}