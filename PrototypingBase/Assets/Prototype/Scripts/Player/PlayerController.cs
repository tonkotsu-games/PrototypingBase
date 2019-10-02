using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveHorizontal;
    private float moveVertical;

    [Header("Speed for the Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;

    private Vector3 moveVector;
    private Vector3 jumpVector;

    private bool grounded = true;
    private bool jump = false;

    public bool triggerLeft = false;

    private Animator anim;
    private Rigidbody rigi;

    private float move;

    void Start()
    {
        jump = false;

        rigi = gameObject.GetComponentInChildren<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
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