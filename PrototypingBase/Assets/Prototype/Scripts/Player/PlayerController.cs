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

    private bool grounded = false;
    private bool normalJump = false;
    private bool airJumping = false;

    private JumpType currentJumpType;
    private bool canJump = false;

    private enum JumpType { None, Normal, Air, Slide}
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

    private bool inSliding = false;
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
    private Animator animator = null;
    private Rigidbody rigidbody = null;
    private Timer lockTimer = new Timer();
    private MovementCalculation calculator = new MovementCalculation();
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
        animator = transform.GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        if (!jumpTest)
        {
            calculator.CalcualteJump(jumpHeight, timeToHeight, airJumpHeight, timeToAirJumpHeight, slideJumpHeight, timeToSlideJumpHeight);
        }

        lockTimer.CountingDown = true;
        CalculateAndSetLockTime(beatTimeLockPercent);
    }

    void Update()
    {
        if (beatTimeLockPercentOld != beatTimeLockPercent)
        {
            beatTimeLockPercentOld = beatTimeLockPercent;
            CalculateAndSetLockTime(beatTimeLockPercent);
        }

        if (jumpTest)
        {
            calculator.CalcualteJump(jumpHeight, timeToHeight, airJumpHeight, timeToAirJumpHeight, slideJumpHeight, timeToSlideJumpHeight);
        }

        if (lockTimer.timeCurrent <= 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (grounded)
                {
                    startPosition = transform.position.y;
                }

                ChangeStanceTo(Stances.Jump);
            }

            if (Input.GetButtonDown("Attack"))
            {
                ChangeStanceTo(Stances.Attack);
            }

            if (Input.GetButtonDown("Gun") && !airAttack)
            {
                ChangeStanceTo(Stances.Gun);
            }
        }
        else
        {
            lockTimer.Tick();
        }

        if (Input.GetButtonDown("Slide"))
        {
            ChangeStanceTo(Stances.Slide, resetLockTimer: false);
        }

        if (currentStance == Stances.Jump || airGun)
        {
            calculator.JumpHight(startPosition, transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            debugMode = !debugMode;
        }
    }

    private void ChangeStanceTo(Stances newStance, bool resetLockTimer = true)
    {
        lastStance = currentStance;
        currentStance = newStance;
        RunStanceChangeTransitions(lastStance, currentStance);

        if (resetLockTimer)
        {
            lockTimer.ResetTimer();
        }
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (currentStance == Stances.Idle ||
            currentStance == Stances.Jump ||
            currentStance == Stances.Gun)
        {
            calculator.CalculateHeading(horizontalInput, verticalInput, deadZone, cam.transform);
            calculator.CalcualteMovement(horizontalInput, verticalInput, deadZone, movementSpeed, cam.transform);

            HeadingUpdate();
            if (!airGun)
            {
                MoveUpdate();
                GravityUpdate();
            }
        }
        else if (currentStance == Stances.Slide)
        {
            SlideUpdate();
        }
        else if (currentStance == Stances.Attack)
        {
            calculator.CalculateHeading(horizontalInput, verticalInput, deadZone, cam.transform);
            HeadingUpdate();
            if (grounded)
            {
                if (airAttack)
                {
                    animator.SetTrigger("meteorAttack");
                    airAttack = false;
                    ChangeStanceTo(Stances.Idle);
                }
                rigidbody.velocity = new Vector3(0, gravity, 0);
            }
        }
        else
        {
            Debug.LogError("WARNING NO STANCE");
        }
    }

    private void HeadingUpdate()
    {
        if (!target.lockOn)
        {
            if (calculator.Head == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(calculator.Head);
            }
        }
        else if (target.lockOn)
        {
            target.lookAt.y = 0;
            transform.rotation = Quaternion.LookRotation(target.lookAt);
        }
    }

    void MoveUpdate()
    {
        rigidbody.velocity = new Vector3(calculator.MoveVector.x,
                                    gravity,
                                    calculator.MoveVector.z);
    }

    private void Jump()
    {
        if (normalJump)
        {
            gravity = calculator.JumpVelocity;
            jumpForce = calculator.JumpVelocity;
            normalJump = false;
        }
        else if (airJumping && !airGun)
        {
            gravity = calculator.AirJumpVelocity;
            jumpForce = calculator.AirJumpVelocity;
            airJumping = false;
        }
        else if (slideJump)
        {
            gravity = calculator.SlideJumpVelocity;
            jumpForce = calculator.SlideJumpVelocity;
            slideJump = false;
        }
        else if (airGun)
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
        }
    }

    private void SlideUpdate()
    {
        if (inSliding && (grounded ||
                        lastStance == Stances.Idle ||
                        lastStance == Stances.Slide ||
                        lastStance == Stances.Gun))
        {
            currentSlideTime -= Time.deltaTime;
            if (currentSlideTime > 0)
            {
                Slide();
            }
            else
            {
                inSliding = false;
                ChangeStanceTo(Stances.Idle);
            }
        }
        else
        {
            rigidbody.velocity = new Vector3(0, gravityMax, 0);
        }
    }

    private void Slide()
    {
        if (calculator.Head == Vector3.zero)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(calculator.Head);
        }

        rigidbody.velocity = new Vector3(transform.forward.x * slidingSpeed,
                                    gravity,
                                    transform.forward.z * slidingSpeed);
    }

    public void RunStanceChangeTransitions(Stances lastStanceCheck, Stances currentStanceCheck)
    {
        switch (lastStanceCheck)
        {
            case Stances.Idle:
                    IdleTransition(currentStanceCheck);
                    break;

            case Stances.Jump:
                    JumpTransitions(currentStanceCheck);
                    break;

            case Stances.Slide:
                    SlideTransitions(currentStanceCheck);
                    break;

            case Stances.Attack:
                    AttackTransition(currentStanceCheck);
                    break;

            case Stances.Gun:
                    GunTransitions(currentStanceCheck);
                    break;
        }
    }

    private void IdleTransition(Stances currentStance)
    {
        switch (currentStance)
        {
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("jumping");
                        normalJump = true;
                        currentJumpType = JumpType.Normal;
                        Jump();
                    }
                    else
                    {
                        animator.SetTrigger("jumping");
                        normalJump = true;
                        currentJumpType = JumpType.Normal;
                        Jump();
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("slide");
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    else
                    {
                        animator.SetTrigger("slide");
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("swordAttack(onB)1");
                        attackChain++;
                    }
                    else
                    {
                        animator.SetTrigger("swordAttack");
                        attackChain = 0;
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("gunAttack(onB)");
                    }
                    else
                    {
                        animator.SetTrigger("gunAttack");
                    }
                    break;
                }
        }
    }

    private void JumpTransitions(Stances currentStance)
    {
        switch (currentStance)
        {
            case Stances.Idle:
                {
                    airGun = false;
                    slideJump = false;
                    airJumping = false;
                    gravity = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        if (calculator.CurrentJumpHeight > maxJumpHeight && !inSliding)
                        {
                            airJumping = false;
                        }
                        else
                        {
                            animator.SetTrigger("airJump");
                            slideJump = false;
                            airJumping = true;
                            Jump();
                        }
                    }
                    else
                    {
                        if (calculator.CurrentJumpHeight > maxJumpHeight && !inSliding)
                        {
                            airJumping = false;
                        }
                        else
                        {
                            animator.SetTrigger("airJump");
                            slideJump = false;
                            Jump();
                        }
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("slide");
                        slideJump = false;
                        airJumping = false;
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    else
                    {
                        animator.SetTrigger("slide");
                        slideJump = false;
                        airJumping = false;
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        slideJump = false;
                        airJumping = false;
                        rigidbody.velocity = new Vector3(0, gravityMax, 0);
                        airAttack = true;
                        animator.SetTrigger("airSwordAttack(onB)");
                    }
                    else
                    {
                        slideJump = false;
                        airJumping = false;
                        rigidbody.velocity = new Vector3(0, gravityMax, 0);
                        airAttack = true;
                        animator.SetTrigger("airSwordAttack");
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        airGun = true;
                        animator.SetTrigger("airGunAttack(onB)");
                        Jump();
                    }
                    else
                    {
                        animator.SetTrigger("airGunAttack");
                        slideJump = false;
                        airJumping = false;
                        rigidbody.velocity = new Vector3(0, gravityMax, 0);
                    }
                    break;
                }
        }
    }

    private void SlideTransitions(Stances currentStance)
    {
        switch (currentStance)
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
                            animator.SetTrigger("jumping");
                            inSliding = false;
                            slideJump = true;
                            Jump();
                        }
                    }
                    else
                    {
                        if (grounded)
                        {
                            animator.SetTrigger("jumping");
                            inSliding = false;
                            normalJump = true;
                            Jump();
                        }
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("slide");
                        HeadingUpdate();
                        currentSlideTime = slideTime;
                    }
                    else
                    {
                        animator.SetTrigger("slide");
                        HeadingUpdate();
                        currentSlideTime = slideTime;
                    }
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("slideSwordAttack(onB)");
                        slideAttackTime = currentSlideTime;
                    }
                    else
                    {
                        animator.SetTrigger("swordAttack");
                        inSliding = false;
                        rigidbody.velocity = Vector3.zero;
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("slideGunAttack(onB)");
                    }
                    else
                    {
                        animator.SetTrigger("gunAttack");
                    }
                    break;
                }
        }
    }

    private void AttackTransition(Stances currentStance)
    {
        switch (currentStance)
        {
            case Stances.Idle:
                {
                    attackChain = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        normalJump = true;
                        Jump();
                    }
                    else
                    {
                        normalJump = true;
                        Jump();
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    else
                    {
                        currentSlideTime = slideTime;
                        inSliding = true;
                    }
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        switch (attackChain)
                        {
                            case 1:
                                {
                                    animator.SetTrigger("swordAttack(onB)2");
                                    attackChain++;
                                    break;
                                }
                            case 2:
                                {
                                    animator.SetTrigger("swordAttack(onB)3");
                                    attackChain = 0;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        animator.SetTrigger("swordAttack");
                        attackChain = 0;
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("gunAttack(onB)");
                    }
                    else
                    {
                        animator.SetTrigger("gunAttack");
                    }
                    break;
                }
        }
    }

    private void GunTransitions(Stances currentStance)
    {
        switch (currentStance)
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
                            normalJump = true;
                            Jump();
                        }
                        else
                        {
                            airGun = false;
                            Jump();
                        }
                    }
                    else
                    {
                        if (grounded)
                        {
                            normalJump = true;
                            Jump();
                        }
                        else
                        {
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
                            slideJump = false;
                            airJumping = false;
                            rigidbody.velocity = new Vector3(transform.forward.x * 50,
                                                         Vector3.down.y * 100,
                                                         transform.forward.z);
                            currentSlideTime = slideTime;
                            inSliding = true;
                            gravity = 0;
                        }
                        else
                        {
                            currentSlideTime = slideTime;
                            inSliding = true;
                        }

                    }
                    else
                    {
                        if (!grounded)
                        {
                            slideJump = false;
                            airJumping = false;
                            rigidbody.velocity = new Vector3(transform.forward.x * 50,
                                                         Vector3.down.y * 100,
                                                         transform.forward.z);
                            currentSlideTime = slideTime;
                            inSliding = true;
                            gravity = 0;
                        }
                        else
                        {
                            currentSlideTime = slideTime;
                            inSliding = true;
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
                            slideJump = false;
                            airJumping = false;
                            airAttack = true;
                            rigidbody.velocity = Vector3.down * 50;
                            animator.SetTrigger("airSwordAttack(onB)");
                        }
                        else
                        {
                            animator.SetTrigger("swordAttack(onB)1");
                            attackChain++;
                        }
                    }
                    else
                    {
                        if (!grounded)
                        {
                            slideJump = false;
                            airJumping = false;
                            airAttack = true;
                            rigidbody.velocity = Vector3.down * 50;
                            animator.SetTrigger("airSwordAttack");
                        }
                        else
                        {
                            animator.SetTrigger("swordAttack");
                            attackChain = 0;
                        }
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        animator.SetTrigger("gunAttack(onB)");
                    }
                    else
                    {
                        animator.SetTrigger("gunAttack");
                    }
                    break;
                }
        }
    }

    private void GravityUpdate()
    {
        if (!grounded)
        {
            if (gravity > gravityMax)
            {
                gravity += calculator.JumpGravity * Time.deltaTime;
            }
            else
            {
                gravity = gravityMax;
            }
        }
    }

    private void CalculateAndSetLockTime(float percent)
    {
        lockTime = (beat.sampleTimeInSec * percent) / 100;
        lockTimer.Set(lockTime);
        lockTimer.timeCurrent = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentStance == Stances.Jump)
        {
            ChangeStanceTo(Stances.Idle, resetLockTimer: false);
        }
        else if (currentStance == Stances.Slide)
        {
            lastStance = currentStance;
        }
        animator.SetTrigger("landing");
        animator.ResetTrigger("jumping");
        slideJump = false;
        airJumping = false;

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
            GUI.Label(new Rect(10, 100, 400, 40), "Air Jump: " + airJumping, style);
            GUI.Label(new Rect(10, 160, 400, 40), "Grounded: " + grounded, style);
            GUI.Label(new Rect(10, 190, 400, 40), "Gravity: " + gravity, style);
            GUI.Label(new Rect(10, 220, 400, 40), "JumpForce: " + jumpForce, style);
            GUI.Label(new Rect(10, 250, 400, 40), "Slide Time: " + currentSlideTime, style);
        }

        if (beat == null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 80;
            style.normal.textColor = Color.red;

            GUI.Label(new Rect(300, 200, 400, 160), "Beatdeatection is null", style);
        }
    }
}