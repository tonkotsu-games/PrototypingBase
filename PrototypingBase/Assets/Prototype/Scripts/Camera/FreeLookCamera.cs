using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [SerializeField]
    private Transform playerTarget;
    //[SerializeField]
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

    private float rotationX;
    private float rotationY;

    private Vector3 offSet;
    private Vector3 offSetNew;
    private Vector3 offSetOld;
    private Vector3 lookAt;


    private bool lockOn = false;

    private void Start()
    {
        offSetNew = new Vector3(0, offSetY, offSetZ);
        cam = Camera.main.transform;
        offSetOld = new Vector3(0,0,0);
        pivot = cam.parent;
    }

    private void Update()
    {
        CameraMove();
        CameraRotation();
    }

    private void FixedUpdate()
    {
        offSetNew = new Vector3(0, offSetY, offSetZ);

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
        if (Input.GetAxisRaw("CameraHorizontal") > deadZone ||
            Input.GetAxisRaw("CameraHorizontal") < -deadZone ||
            Input.GetAxisRaw("CameraVertical") > deadZone ||
            Input.GetAxisRaw("CameraVertical") < -deadZone)
        {

            rotationY -= Input.GetAxisRaw("CameraVertical") * senitivity;
            rotationY = Mathf.Clamp(rotationY, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(rotationY, 0, 0);

            if(lockOn)
            {
                lookAt = enemyTarget.position;
            }

            rotationX += Input.GetAxisRaw("CameraHorizontal") * senitivity;
            transform.rotation = Quaternion.Euler(0, rotationX, 0);

        }
    }
}