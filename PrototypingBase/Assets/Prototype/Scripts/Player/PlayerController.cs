using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageAble
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
    [SerializeField]
    private int health = 0;
    private int currentHealth = 0;
    private Renderer render = null;
    #region Movement
    [Header("Movement Stick DeadZone")]
    [SerializeField]
    [Range(0, 1)]
    private float deadZone = 0.1f;
    [Header("Value for the Movement")]
    [SerializeField]
    private float movementSpeed = 0;
    [Header("Attackmovement")]
    [SerializeField]
    private float attackStrafe = 0;
    private float attackStrafeUpdate = 0;
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

    private bool grounded = false;
    #endregion

    #region Slide
    [Header("Time for Slide")]
    [SerializeField]
    private float slideTime = 0;
    [SerializeField]
    private float slideAttackTime = 0;
    [Header("Value for Sliding Speed")]
    [SerializeField]
    private float slidingSpeed = 0;
    #endregion

    #region Attack
    private BoxCollider weaponCollider = null;

    [HideInInspector]
    public int attackChain = 0;

    private bool airAttack = false;
    #endregion
    #endregion

    #region System
    [Header("Beatanalyse")]
    [SerializeField]
    private BeatAnalyse beat = null;
    private Animator animator = null;
    private Rigidbody rigidbody = null;
    private Timer lockTimer = new Timer(), materialChangeTimer = new Timer();
    private MovementCalculation calculator = new MovementCalculation();
    private MovementUpdate moveUpdate = new MovementUpdate();
    private Jumping jump = new Jumping();
    private Sliding sliding = new Sliding();
    private HeadingUpdate headingUpdate = new HeadingUpdate();
    private GravityArtificial gravityUpdate = new GravityArtificial();
    [SerializeField]
    private Transform respawner;
    [SerializeField]
    private GameObject cam = null;
    [SerializeField]
    private FreeLookCamera target = null;

    private StateMachine stateMachine = new StateMachine();

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
    [SerializeField]
    private bool TURNOFFTOFIX = false;

    private Stances currentStance = Stances.Idle;
    private Stances lastStance = Stances.Idle;

    private bool debugMode = false;

    [Header("Material for player beat hit")]
    [SerializeField]
    private List<Material> material;
    [SerializeField]
    private float materialChangeTime = 0;
    private bool materialChanged = false;
    #endregion

    void Start()
    {
        weaponCollider = GetComponentInChildren<BoxCollider>();
        weaponCollider.enabled = false;
        animator = transform.GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        render = transform.GetComponentInChildren<Renderer>();
        if (!jumpTest)
        {
            calculator.CalcualteJump(jumpHeight, timeToHeight, airJumpHeight, timeToAirJumpHeight, slideJumpHeight, timeToSlideJumpHeight);
        }

        CalculateAndSetLockTime(beatTimeLockPercent);
        currentHealth = health;
        SetSlideValue();
    }

    void Update()
    {
        bool jumpPressed = Input.GetButtonDown("Jump");
        bool attackPressed = Input.GetButtonDown("Attack");
        bool gunPressed = Input.GetButtonDown("Gun");
        bool slidePressed = Input.GetButtonDown("Slide");

        if (TURNOFFTOFIX)
        {
            jumpPressed = Random.value > 0.5f;
            attackPressed = Random.value > 0.5f;
            gunPressed = Random.value > 0.5f;
            slidePressed = Random.value > 0.5f;
        }

        if(materialChangeTimer.timeMax != materialChangeTime)
        {
            materialChangeTimer.timeMax = materialChangeTime;
        }

        if(materialChangeTimer.timeCurrent <= 0 && materialChanged)
        {
            materialChanged = false;
            ChangeMaterial(false);
        }
        else
        {
            materialChangeTimer.Tick();
        }

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
            if (jumpPressed)
            {
                if (grounded)
                {
                    calculator.StartPosition = transform.position.y;
                }

                ChangeStanceTo(Stances.Jump);
            }

            if (attackPressed)
            {
                ChangeStanceTo(Stances.Attack);
            }

            if (gunPressed && !airAttack)
            {
                ChangeStanceTo(Stances.Gun);
            }
        }
        else
        {
            lockTimer.Tick();
        }

        if (slidePressed)
        {
            sliding.InSliding = true;
            ChangeStanceTo(Stances.Slide, resetLockTimer: false);
        }

        if (currentStance == Stances.Jump)
        {
            calculator.JumpHight(transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            debugMode = !debugMode;
        }
    }

    public void ChangeStanceTo(Stances newStance, bool resetLockTimer = true)
    {
        lastStance = currentStance;
        currentStance = newStance;
        RunStanceChangeTransitions(lastStance, currentStance);

        if (resetLockTimer)
        {
            lockTimer.ResetTimer();
        }

        if (weaponCollider.enabled == true)
        {
            weaponCollider.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (TURNOFFTOFIX)
        {
            horizontalInput = Random.value * 2 - 1;
            verticalInput = Random.value * 2 - 1;
        }

        if (currentStance == Stances.Idle ||
            currentStance == Stances.Jump ||
            currentStance == Stances.Gun)
        {
            calculator.CalculateHeading(horizontalInput, verticalInput, deadZone, cam.transform);
            calculator.CalcualteMovement(horizontalInput, verticalInput, deadZone, movementSpeed, cam.transform);
            headingUpdate.Heading(calculator.Head,transform,target);
            moveUpdate.MoveUpdate(currentStance,rigidbody,sliding.SlideVelocity,calculator.MoveVector,gravityUpdate.Gravity,attackStrafe);
            gravityUpdate.GravityUpdate(grounded, gravityMax,calculator.JumpGravity);
        }
        else if (currentStance == Stances.Slide)
        {
            if (sliding.CurrentSlideTime >= 0)
            {
                sliding.SlideUpdate(grounded, lastStance, gravityUpdate.Gravity, gravityMax, calculator.Head, transform);
            }
            else
            {
                ChangeStanceTo(Stances.Idle);
            }
            moveUpdate.MoveUpdate(currentStance, rigidbody, sliding.SlideVelocity, calculator.MoveVector, gravityUpdate.Gravity,attackStrafe);
        }
        else if (currentStance == Stances.Attack)
        {
            if (weaponCollider.enabled == false)
            {
                weaponCollider.enabled = true;
            }
            calculator.CalculateHeading(horizontalInput, verticalInput, deadZone, cam.transform);
            headingUpdate.Heading(calculator.Head, transform, target);
            if (grounded)
            {
                if (airAttack)
                {
                    animator.SetTrigger("meteorAttack");
                    airAttack = false;
                    ChangeStanceTo(Stances.Idle);
                }
                moveUpdate.MoveUpdate(currentStance, rigidbody, sliding.SlideVelocity, calculator.MoveVector, gravityUpdate.Gravity, attackStrafeUpdate);
            }
        }
        else
        {
            Debug.LogError("WARNING NO STANCE");
        }
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
                        //invulerabilty
                        ChangeMaterial(true);
                    }
                    animator.SetTrigger("jumping");
                    gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        //invulnerable
                        ChangeMaterial(true);
                    }
                    animator.SetTrigger("slide");
                    sliding.CurrentSlideTime = slideTime;
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        attackStrafeUpdate = attackStrafe;
                        animator.SetTrigger("swordAttack(onB)1");
                        attackChain++;
                    }
                    else
                    {
                        attackStrafeUpdate = attackStrafe;
                        animator.SetTrigger("swordAttack");
                        attackChain = 0;
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
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
                    TriggerReset();
                    gravityUpdate.Gravity = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (calculator.CurrentJumpHeight < maxJumpHeight)
                    {
                        if (beat.IsOnBeat(reactionTime, timeWindow))
                        {
                            ChangeMaterial(true);
                            //invulnerable
                        }

                        animator.SetTrigger("airJump");
                        gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Air);
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }
                    animator.SetTrigger("slide");
                    sliding.CurrentSlideTime = slideTime;
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        rigidbody.velocity = new Vector3(0, gravityMax, 0);
                        airAttack = true;
                        animator.SetTrigger("airSwordAttack(onB)");
                    }
                    else
                    {
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
                        ChangeMaterial(true);
                        animator.SetTrigger("airGunAttack(onB)");
                        gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Air);
                    }
                    else
                    {
                        animator.SetTrigger("airGunAttack");
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
                    TriggerReset();
                    gravityUpdate.Gravity = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        if (grounded)
                        {
                            ChangeMaterial(true);
                            animator.SetTrigger("jumping");
                            gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Slide);
                        }
                    }
                    else
                    {
                        if (grounded)
                        {
                            animator.SetTrigger("jumping");
                            gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
                        }
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }

                    animator.SetTrigger("slide");
                    headingUpdate.Heading(calculator.Head, transform, target);
                    sliding.CurrentSlideTime = slideTime;
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        animator.SetTrigger("slideSwordAttack(onB)");
                        slideAttackTime = slideTime;
                    }
                    else
                    {
                        animator.SetTrigger("swordAttack");
                        sliding.InSliding = false;
                        rigidbody.velocity = Vector3.zero;
                    }
                    break;
                }
            case Stances.Gun:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
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
                    TriggerReset();
                    attackChain = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }
                    gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }
                    sliding.CurrentSlideTime = slideTime;
                    break;
                }
            case Stances.Attack:
                {
                    attackStrafeUpdate = 0;
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
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
                        ChangeMaterial(true);
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
                    TriggerReset();
                    gravityUpdate.Gravity = 0;
                    break;
                }
            case Stances.Jump:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }

                    if (grounded)
                    {
                        gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Normal);
                    }
                    else
                    {
                        gravityUpdate.Gravity = jump.Jump(sliding.InSliding, calculator.JumpVelocity, calculator.AirJumpVelocity, calculator.SlideJumpVelocity, Jumping.JumpType.Air);
                    }
                    break;
                }
            case Stances.Slide:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        //invulnerable
                    }
                    if (!grounded)
                    {
                        rigidbody.velocity = new Vector3(transform.forward.x * 50,
                                                     Vector3.down.y * 100,
                                                     transform.forward.z);
                        sliding.CurrentSlideTime = slideTime;
                        gravityUpdate.Gravity = 0;
                    }
                    else
                    {
                        sliding.CurrentSlideTime = slideTime;
                    }
                    break;
                }
            case Stances.Attack:
                {
                    if (beat.IsOnBeat(reactionTime, timeWindow))
                    {
                        ChangeMaterial(true);
                        if (!grounded)
                        {
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
                        ChangeMaterial(true);
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

    private void CalculateAndSetLockTime(float percent)
    {
        lockTime = (beat.sampleTimeInSec * percent) / 100;
        lockTimer.Set(lockTime);
        lockTimer.timeCurrent = 0;
    }

    private void TriggerReset()
    {
        animator.ResetTrigger("jumping");
        animator.ResetTrigger("airJump");
        animator.ResetTrigger("slide");
        animator.ResetTrigger("swordAttack");
        animator.ResetTrigger("slideSwordAttack(onB)");
        animator.ResetTrigger("swordAttack(onB)1");
        animator.ResetTrigger("swordAttack(onB)2");
        animator.ResetTrigger("swordAttack(onB)3");
        animator.ResetTrigger("airSwordAttack(onB)");
        animator.ResetTrigger("gunAttack");
        animator.ResetTrigger("gunAttack(onB)");
        animator.ResetTrigger("airGunAttack");
        animator.ResetTrigger("airGunAttack(onB)");
        animator.ResetTrigger("slideGunAttack(onB)");
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = health;
            transform.position = respawner.position;
        }
    }

    private void SetSlideValue()
    {
        sliding.SlidingSpeed = slidingSpeed;
    }

    private void ChangeMaterial(bool beatHit)
    {
        if (beatHit)
        {
            materialChanged = true;
            render.material = material[1];
            materialChangeTimer.ResetTimer();
        }
        else
        {
            render.material = material[0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger("landing");
        if (currentStance == Stances.Jump)
        {
            ChangeStanceTo(Stances.Idle, resetLockTimer: false);
        }
        else if (currentStance == Stances.Slide)
        {
            lastStance = currentStance;
        }
        gravityUpdate.Gravity = 0;
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
        animator.ResetTrigger("landing");
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos.y = maxJumpHeight;
        Gizmos.DrawCube(pos, new Vector3(5, 0, 5));
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
            GUI.Label(new Rect(10, 100, 400, 40), "Jump State: " + jump.JumpTypeDisplay, style);
            GUI.Label(new Rect(10, 160, 400, 40), "Grounded: " + grounded, style);
            GUI.Label(new Rect(10, 190, 400, 40), "Gravity: " + gravityUpdate.Gravity, style);
            GUI.Label(new Rect(10, 220, 400, 40), "JumpForce: " + jump.JumpForce, style);
            GUI.Label(new Rect(10, 250, 400, 40), "Slide Time: " + sliding.CurrentSlideTime, style);
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