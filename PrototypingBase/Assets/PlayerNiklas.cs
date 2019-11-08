using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNiklas : MonoBehaviour,IDamageAble
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
    int currentHealth;

    [HideInInspector]
    public bool slashRight = false;

    [SerializeField]
    int maxHealth;

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

    [SerializeField]
    float knockbackStrength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckNiklas>();
        dashTimer.Set(1);
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        slashScript = gameObject.GetComponentInChildren<SlashCheckNiklas>();
        slashHitbox = slashScript.gameObject.GetComponent<BoxCollider>();
        currentHealth = maxHealth;
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
            Physics.IgnoreLayerCollision(10, 11, false);
        }
        else
        {
            if (dashTimer.timeCurrent <= 0)
            {
                Physics.IgnoreLayerCollision(10, 11, false);
                inDash = false;
            }
        }
    }

    private void InitiateDash()
    {
        if (!inDash)
        {
            Physics.IgnoreLayerCollision(10, 11, true);
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

    public void EnemyHit(GameObject target)
    {
        target.GetComponent<MockupEnemyController>().DamageAndPush(2,transform.position,knockbackStrength);       
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Player got hit! Current Health: " + currentHealth);
    }

    void OnGui()
    {
        if (currentHealth <= 0)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            style.fontSize = 40;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("YOU DIEDED PLEB!", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

        }
    }
}
