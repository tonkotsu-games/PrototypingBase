using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [SerializeField]
    private Transform playerTarget;
    [SerializeField]
    private Transform enemyTarget;
    private Transform pivot;
    private Transform cam;

    [Header("Camera Stick Dead Zone")]
    [SerializeField]
    [Range(0, 1)]
    private float deadZone;

    [Header("Offset for the Camera")]
    [Range(0, 20)]
    [SerializeField]
    private float offSetY;
    [Range(-20, 0)]
    [SerializeField]
    private float offSetZ;
    [SerializeField]
    private float followSpeed;

    [Header("Camera movement")]
    [Range(1, 10)]
    [SerializeField]
    private float senitivity;
    [Range(-90, 0)]
    [SerializeField]
    private float minAngle;
    [Range(0, 90)]
    [SerializeField]
    private float maxAngle;

    [Header("Camera LockOn")]
    [Range(0, 1)]
    [SerializeField]
    private float lookAtSpeed;
    [Range(0, 10)]
    [SerializeField]
    private float lockOnOffSetY;

    private float rotationX;
    private float rotationY;

    private Vector3 offSet;
    private Vector3 offSetNew;
    private Vector3 offSetOld;

    [HideInInspector]
    public Vector3 lookAt;

    [HideInInspector]
    public bool lockOn = false;

    private void Start()
    {
        offSetNew = new Vector3(0, offSetY, offSetZ);
        cam = Camera.main.transform;
        offSetOld = new Vector3(0, 0, 0);
        pivot = cam.parent;
    }

    private void Update()
    {
        CameraMove();
        CameraRotation();
        if (Input.GetButtonDown("CameraButton"))
        {
            lockOn = !lockOn;
        }
    }

    private void FixedUpdate()
    {
        if (!lockOn)
        {
            offSetNew = new Vector3(0, offSetY, offSetZ);
        }
        else
        {
            offSet = new Vector3(0, lockOnOffSetY, offSetZ);
        }
        if (offSetOld != offSetNew)
        {
            cam.localPosition = new Vector3(pivot.localPosition.x, pivot.localPosition.y + offSetY, pivot.localPosition.z + offSetZ);
            offSetOld = offSetNew;
        }
    }

    public void CameraMove()
    {
        Vector3 targetPosition = Vector3.Lerp(transform.position, playerTarget.transform.position, followSpeed);
        transform.position = targetPosition;
    }

    private void CameraRotation()
    {
        if ((Input.GetAxisRaw("CameraHorizontal") > deadZone ||
            Input.GetAxisRaw("CameraHorizontal") < -deadZone ||
            Input.GetAxisRaw("CameraVertical") > deadZone ||
            Input.GetAxisRaw("CameraVertical") < -deadZone) && !lockOn)
        {

            rotationY -= Input.GetAxisRaw("CameraVertical") * senitivity;
            rotationY = Mathf.Clamp(rotationY, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(rotationY, 0, 0);

            rotationX += Input.GetAxisRaw("CameraHorizontal") * senitivity;
            transform.rotation = Quaternion.Euler(0, rotationX, 0);
        }
        else if (lockOn && enemyTarget != null)
        {
            lookAt = enemyTarget.position - transform.position;
            lookAt.Normalize();
            //lookAt.y = 0

            if (lookAt == Vector3.zero)
            {
                lookAt = transform.forward;
            }
            Quaternion lookAtRot = Quaternion.LookRotation(lookAt);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRot, lookAtSpeed);
        }
    }
}