using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNiklas : MonoBehaviour
{

    float inputHorizontal;
    float inputVertical;
    Rigidbody playerRigidbody;
    Vector3 moveVector;
    bool inDash;
    Vector3 dashStartPosition;
    Timer dashTimer = new Timer();
    GroundCheckNiklas groundCheck;
    Animator playerAnim;
    SlashCheckNiklas slashScript;
    BoxCollider slashHitbox;

    [HideInInspector]
    public bool slashRight = false;

    [SerializeField]
    float movementSpeed = 1f;
    [SerializeField]
    bool baseMovementEnabled = true;
    [SerializeField]
    GameObject playerBody;
    [SerializeField]
    float dashDistance = 5f;

    [SerializeField]
    float dashSpeed = 50f;

    [SerializeField]
    float gravity = -60f;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckNiklas>();
        dashTimer.Set(1);
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        slashScript = gameObject.GetComponentInChildren<SlashCheckNiklas>();
        slashHitbox = slashScript.gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAnim.ResetTrigger("slash1");
        playerAnim.ResetTrigger("slash2");

        if (groundCheck.isGrounded)
        {
        


            if (Input.GetButtonDown("Dash"))
            {
                InitiateDash();

            }

            else if (Input.GetButtonDown("Slash"))
            {
                slashHitbox.enabled = true;
                if (slashRight)
                {
                    playerAnim.SetTrigger("slash1");
                }
                else
                {
                    playerAnim.SetTrigger("slash2");
                }
            }

            else if (!inDash)
            {
                BaseMovement();
            }

            else
            {
                dashTimer.Tick();

                DashUntilTick();
            }
        }
        else
        {
            playerRigidbody.AddForce(0, gravity, 0);
        }

        playerAnim.SetFloat("speed", playerRigidbody.velocity.magnitude);

    }

    private void DashUntilTick()
    {
        if (Vector3.Distance(dashStartPosition, transform.position) > dashDistance)
        {
            playerRigidbody.velocity = Vector3.zero;
            inDash = false;
        }
        else
        {
            if (dashTimer.timeCurrent <= 0)
            {
                inDash = false;
            }
        }
    }

    private void InitiateDash()
    {
        if (!inDash)
        {
            dashTimer.ResetTimer();
            inDash = true;
            dashStartPosition = gameObject.transform.position;
            playerRigidbody.velocity = moveVector * dashSpeed;
        }
    }

    private void BaseMovement()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        moveVector = new Vector3(inputHorizontal, 0, inputVertical);
        if (moveVector != Vector3.zero)
        {
            playerBody.transform.rotation = Quaternion.LookRotation(moveVector);
        }
        if (baseMovementEnabled)
        {
            playerRigidbody.velocity = moveVector * movementSpeed;
        }
    }

    public void DisableSlashHitbox()
    {
        slashHitbox.enabled = false;
    }
}
