using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNiklas : MonoBehaviour, IDamageAble
{

    StateMachine stateMachine = new StateMachine();
    public StateMachine StateMachine { get => stateMachine; }

    [SerializeField]
    AttackStates newAttackSate;
    AttackStates currentAttackState;

    IState currentAttackISate;

    private static GameObject player;
    public static GameObject Player { get => player; }

    Rigidbody playerRigidbody;

    Vector3 moveVector;
    public Vector3 MoveVector { get => moveVector; set => moveVector = value; }

    GroundCheckNiklas groundCheck;
    int currentHealth;

    [HideInInspector]
    public bool slashRight = false;

    [SerializeField]
    int maxHealth;

    [SerializeField]
    float movementSpeed = 1f;
    public float MovementSpeed { get => movementSpeed; }
    [SerializeField]
    float dashDistance = 5f;
    public float DashDistance { get => dashDistance; }
    [SerializeField]
    float dashSpeed = 50f;
    public float DashSpeed { get => dashSpeed; }
    [SerializeField]
    float attackStepDistance = 5f;
    public float AttackStepDistance { get => attackStepDistance; }
    [SerializeField]
    float stepSpeed = 10f;
    public float StepSpeed { get => stepSpeed; }

    [SerializeField]
    float gravity = -60f;
    [SerializeField]
    float knockbackStrength = 5f;

    private bool currentAttackIsNull = false;

    void Start()
    {
        player = gameObject;
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckNiklas>();
        currentHealth = maxHealth;
        stateMachine.ChangeState(new MoveState(player));
        currentAttackState = newAttackSate;
        currentAttackISate = newAttackSate.NewState();
    }

    void Update()
    {
        if (newAttackSate != currentAttackState)
        {
            currentAttackState = newAttackSate;
            currentAttackISate = newAttackSate.NewState();
            currentAttackIsNull = false;
        }
        if(currentAttackISate == null)
        {
            currentAttackIsNull = true;
        }

        if (Input.GetButtonDown("Dash") &&
            stateMachine.CurrentState != currentAttackISate &&
            stateMachine.CurrentState.ToString() != "DashState")
        {
            stateMachine.ChangeState(new DashState(player));
        }
        else if (Input.GetButtonDown("Slash") &&
                 stateMachine.CurrentState != currentAttackISate &&
                 stateMachine.CurrentState.ToString() != "DashState")
        {
            if (currentAttackISate != null)
            {
                stateMachine.ChangeState(currentAttackISate);
            }
        }


    }

    private void FixedUpdate()
    {
        if (groundCheck.IsGrounded )
        {
            stateMachine.ExecuteStateUpdate();
        }
        else
        {
            playerRigidbody.AddForce(0, gravity, 0);
        }

    }

    public void EnemyHit(GameObject target)
    {
        target.GetComponent<MockupEnemyController>().DamageAndPush(2, transform.position, knockbackStrength);
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Player got hit! Current Health: " + currentHealth);
    }

    private void OnGUI()
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
        if(currentAttackIsNull)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            style.fontSize = 40;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("!Warning current attack state is null!", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
        GUI.TextField(new Rect(0, 20, 100, 20), stateMachine.CurrentState.ToString());
    }
}

public enum AttackStates
{
    FastSlash
}

public static class TakenStateChanger
{

    public static IState NewState(this AttackStates stateType)
    {
        switch (stateType)
        {
            case AttackStates.FastSlash:
                return new FastSlashState(PlayerNiklas.Player);
            default:
                return null;
        }
    }
}