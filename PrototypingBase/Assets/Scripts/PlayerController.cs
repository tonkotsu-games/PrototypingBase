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

    BoxCollider groundCollider;

    [Header("Speed for the Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;

    public Vector3 heading;

    private Vector3 moveVector;
    private Vector3 jumpVector;

    private bool grounded = true;
    private bool jump = false;

    public bool triggerLeft = false;

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
        jump = false;

        dancemove = 0;

        rigi = gameObject.GetComponentInChildren<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
        groundCollider = gameObject.GetComponent<BoxCollider>();
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        MovementCalculation();
        Move();

        if ((Input.GetAxisRaw("Horizontal") >= 0.1 ||
             Input.GetAxisRaw("Horizontal") <= -0.1 ||
             Input.GetAxisRaw("Vertical") >= 0.1 ||
             Input.GetAxisRaw("Vertical") <= -0.1))
        {
            heading = new Vector3(Input.GetAxisRaw("Horizontal"),
                                  0,
                                  Input.GetAxisRaw("Vertical"));

            heading = heading.normalized;
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

        moveVector = moveVector.normalized * move * movementSpeed;

    }

    /// <summary>
    /// Function for the Movement
    /// </summary>
    void Move()
    {
        if (jump)
        {
            rigi.AddForce(Vector3.up * 10000 * jumpForce);
            jump = false;
        }
        rigi.velocity = new Vector3(moveVector.x,
                        rigi.velocity.y,
                        moveVector.z);
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
}