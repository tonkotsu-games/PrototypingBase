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
    private float jumpForce = 0;
    private float startPosition = 0;

    private bool grounded = true;
    private bool jump = false;
    private bool airJumping = false;
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
    [HideInInspector]
    public int attackChain = 0;

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
    private MovementCalculation calculate = new MovementCalculation();
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
            calculate.Jump(jumpHeight, timeToHeight, airJumpHeight, timeToAirJumpHeight, slideJumpHeight, timeToSlideJumpHeight);
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
            calculate.Jump(jumpHeight, timeToHeight, airJumpHeight, timeToAirJumpHeight, slideJumpHeight, timeToSlideJumpHeight);
        }

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
            calculate.JumpHight(startPosition, transform.position.y);
        }

        if (calculate.CurrentJumpHeight > maxJumpHeight)
        {
            rigi.transform.position = new Vector3(transform.position.x, startPosition + maxJumpHeight, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.F4))
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
            calculate.Heading(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), deadZone, cam.transform);
            calculate.Movement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), deadZone, movementSpeed, cam.transform);
            Heading();
            if (!airGun)
            {
                Move();
                Gravity();
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
        if (!target.lockOn)
        {
            if (calculate.Head == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(calculate.Head);
            }
        }
        else if (target.lockOn)
        {
            target.lookAt.y = 0;
            transform.rotation = Quaternion.LookRotation(target.lookAt);
        }
    }

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        rigi.velocity = new Vector3(calculate.MoveVector.x,
                                    gravity,
                                    calculate.MoveVector.z);
    }

    private void Jump()
    {
        if (jump)
        {
            gravity = calculate.JumpVelocity;
            jumpForce = calculate.JumpVelocity;
            jump = false;
        }
        else if (airJumping && !airGun)
        {
            gravity = calculate.AirJumpVelocity;
            jumpForce = calculate.AirJumpVelocity;
            airJumping = false;
        }
        else if (slideJump)
        {
            gravity = calculate.SlideJumpVelocity;
            jumpForce = calculate.SlideJumpVelocity;
            slideJump = false;
        }
        else if (airGun)
        {
            rigi.velocity = new Vector3(0,
                                        0,
                                        0);
        }
    }

    private void Slide()
    {
        if (sliding && (grounded || lastStance == Stances.Idle || lastStance == Stances.Slide || lastStance == Stances.Gun))
        {
            currentSlideTime -= Time.deltaTime;
            if (currentSlideTime > 0)
            {
                if (calculate.Head == Vector3.zero)
                {
                    transform.rotation = Quaternion.identity;
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(calculate.Head);
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
                                    anim.SetTrigger("jumping");
                                    jump = true;
                                    Jump();
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    anim.SetTrigger("jumping");
                                    jump = true;
                                    Jump();
                                }
                                break;
                            }
                        case Stances.Slide:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("slide");
                                    currentSlideTime = slideTime;
                                    sliding = true;
                                    bodyCollider.enabled = false;
                                }
                                else
                                {
                                    anim.SetTrigger("slide");
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
                                    anim.SetTrigger("swordAttack(onB)1");
                                    attackChain++;
                                }
                                else
                                {
                                    anim.SetTrigger("swordAttack");
                                    attackChain = 0;
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gunAttack(onB)");
                                }
                                else
                                {
                                    anim.SetTrigger("gunAttack");
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
                                anim.SetTrigger("landing");
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
                                    anim.SetTrigger("airJump");
                                    bodyCollider.enabled = false;
                                    slideJump = false;
                                    airJumpingGravity = true;
                                    Jump();
                                }
                                else
                                {
                                    anim.SetTrigger("airJump");
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
                                    anim.SetTrigger("slide");
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
                                    anim.SetTrigger("slide");
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
                                    rigi.velocity = new Vector3(0,
                                                                gravityMax,
                                                                0);
                                    airAttack = true;
                                    anim.SetTrigger("airSwordAttack(onB)");
                                }
                                else
                                {
                                    airJumpingGravity = false;
                                    slideJump = false;
                                    //reachedHeighestPoint = false;
                                    airJumping = false;
                                    //highestJumpHeight = 0;
                                    rigi.velocity = new Vector3(0,
                                                                gravityMax,
                                                                0);
                                    airAttack = true;
                                    anim.SetTrigger("airSwordAttack");
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    //Debug.Log("Gun on beat");
                                    airGun = true;
                                    anim.SetTrigger("airGunAttack(onB)");
                                    Jump();
                                }
                                else
                                {
                                    anim.SetTrigger("airGunAttack");
                                    //Debug.Log("Gun off beat");
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
                                        anim.SetTrigger("jumping");
                                        sliding = false;
                                        slideJump = true;
                                        Jump();
                                    }
                                }
                                else
                                {
                                    if (grounded)
                                    {
                                        anim.SetTrigger("jumping");
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
                                    anim.SetTrigger("slide");
                                    bodyCollider.enabled = false;
                                    Heading();
                                    currentSlideTime = slideTime;
                                }
                                else
                                {
                                    anim.SetTrigger("slide");
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
                                    anim.SetTrigger("slideSwordAttack(onB)");
                                    slideAttackTime = currentSlideTime;
                                }
                                else
                                {
                                    anim.SetTrigger("slideSwordAttack");
                                    sliding = false;
                                    rigi.velocity = Vector3.zero;
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("slideGunAttack(onB)");
                                }
                                else
                                {
                                    anim.SetTrigger("slideGunAttack");
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
                                    switch(attackChain)
                                    {
                                        case 1:
                                            {
                                                anim.SetTrigger("swordAttack(onB)2");
                                                attackChain++;
                                                break;
                                            }
                                        case 2:
                                            {
                                                anim.SetTrigger("swordAttack(onB)3");
                                                attackChain = 0;
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    anim.SetTrigger("swordAttack");
                                    attackChain = 0;
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gunAttack(onB)");
                                }
                                else
                                {
                                    anim.SetTrigger("gunAttack");
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
                                        anim.SetTrigger("airSwordAttack(onB)");
                                    }
                                    else
                                    {
                                        anim.SetTrigger("swordAttack(onB)1");
                                        attackChain++;
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
                                        anim.SetTrigger("airSwordAttack");
                                    }
                                    else
                                    {
                                        anim.SetTrigger("swordAttack");
                                        attackChain = 0;
                                    }
                                }
                                break;
                            }
                        case Stances.Gun:
                            {
                                if (beat.IsOnBeat(reactionTime, timeWindow))
                                {
                                    anim.SetTrigger("gunAttack(onB)");
                                }
                                else
                                {
                                    anim.SetTrigger("gunAttack");
                                }
                                break;
                            }
                    }
                    break;
                }
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
                    gravity += calculate.JumpGravity * Time.deltaTime;
                }
                else
                {
                    gravity += calculate.AirJumpGravity * Time.deltaTime;
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