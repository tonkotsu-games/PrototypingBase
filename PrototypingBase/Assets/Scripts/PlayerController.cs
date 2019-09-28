using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveHorizontal;
    private float moveVertical;

    public float MaskWeight;
    private AnimatorClipInfo[] clipInfo;

    public LayerMask ground;
    public static bool runattack1DONE;

    BoxCollider groundCollider;

    [Header("Speed for the Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float acceleration;

    [Header("Dash settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] GameObject dashParticlesPrefab;

    public Vector3 heading;

    private Vector3 dashdirection;
    private Vector3 moveVector;

    private Transform closest = null;

    bool grounded = true;

    private bool jump = false;
    private bool dashing = false;
    private bool knockBackAOE = false;
    public bool triggerLeft = false;

    public static bool show = false;
    public static bool attack = false;
    public static bool dancing = false;
    public static bool attack1DONE;

    public bool chargedDash = false;
    bool sliding = false;

    private Animator anim;
    private Rigidbody rigi;
    private BoxCollider boxCol;

    private float move;
    [SerializeField]
    ParticleSystem[] bloodSplatter;

    //gibt an, welcher Dancemove abgespielt werden soll
    private int dancemove;
    private int fixedUpdateTicks = 0;

    void Start()
    {
        dashing = false;
        jump = false;
        show = false;
        attack = false;
        dancing = false;
        attack1DONE = false;
        runattack1DONE = false;

        dancemove = 0;

        rigi = gameObject.GetComponentInChildren<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
        groundCollider = gameObject.GetComponent<BoxCollider>();
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");
        jump = Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        anim.SetFloat("speed", rigi.velocity.magnitude);
        MovementCalculation();

        if(Input.GetMouseButton(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            sliding = true;
            anim.SetTrigger("sliding");
        }
        else if( Input.GetMouseButton(0) && sliding)
        {
            Debug.LogError("Slide");
            float slide = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
            if (slide > 1)
            {
                slide = 1;
            }
            Vector3 slideVector = new Vector3(moveHorizontal, moveVertical, 0f);

            slideVector = (slideVector.normalized * -1) * slide * 20f;
            slideVector.z = 20f;

            rigi.velocity = slideVector;
        }
        else
        {
            sliding = false;
            if(grounded && jump)
            {
                anim.SetTrigger("jumping");
            }

            if (!dashing && !dancing && grounded && !chargedDash)
            {
                Move();
            }
            if ((Input.GetAxisRaw("Horizontal") >= 0.1 ||
                 Input.GetAxisRaw("Horizontal") <= -0.1 ||
                 Input.GetAxisRaw("Vertical") >= 0.1 ||
                 Input.GetAxisRaw("Vertical") <= -0.1) &&
                 !dashing &&
                 !dancing)
            {
                heading = new Vector3(Input.GetAxisRaw("Horizontal"),
                                      0,
                                      Input.GetAxisRaw("Vertical"));

                heading = heading.normalized;


                anim.SetBool("running", true);
            }
            else
            {
                anim.SetBool("running", false);
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
        moveVector = new Vector3(moveHorizontal, 0f, moveVertical);

        moveVector = moveVector.normalized * move * movementSpeed;
    }

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        rigi.velocity = new Vector3(moveVector.x,
                                    0,
                                    moveVector.z);
        anim.SetFloat("speed", rigi.velocity.magnitude);
    }

    public void AfterAttack()
    {
        attack = false;
        boxCol.enabled = false;
    }

    public void AfterDancing()
    {
        dancing = false;
        anim.SetBool("dance", false);
        anim.SetBool("dance2", false);
        anim.SetBool("dance3", false);
    }

    public void AfterDash()
    {
        jump = false;
    }

    //private void SpawnDashParticles()
    //{
    //    GameObject particlesInstance = Instantiate(dashParticlesPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
    //    //particlesInstance.GetComponent<FollowPosition>().followTarget = gameObject.transform;
    //    particlesInstance.gameObject.transform.Rotate(-90, 0, 0);
    //    ParticleSystem parts = particlesInstance.GetComponentInChildren<ParticleSystem>();
    //    float totalDuration = parts.main.duration + parts.main.startLifetime.constant + parts.main.startDelay.constant;
    //    Destroy(particlesInstance, totalDuration);
    //}

    public void attacking()
    {
        if (attack1DONE == false)
        {
            anim.Play("Attack", 0, 0);
            attack = true;
            attack1DONE = true;
            runattack1DONE = true;
        }
        else
        {
            anim.Play("Attack2", 0, 0);
            attack = true;
            attack1DONE = false;
            runattack1DONE = false;
        }
    }

    public void runattacking()
    {
        anim.SetTrigger("runattack2");
        attack = true;
        attack1DONE = false;
        runattack1DONE = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            anim.SetBool("grounded", true);
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            anim.SetBool("grounded", false);
            grounded = false;
        }
    }
}