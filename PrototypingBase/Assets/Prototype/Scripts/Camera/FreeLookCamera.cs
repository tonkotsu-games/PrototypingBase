﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{

    [SerializeField] private GameObject playerTarget;
    [SerializeField] private GameObject enemyTarget;
    private GameObject lookAt;

    [Header("Camera Stick Dead Zone")]
    [SerializeField]
    [Range(0, 1)]
    private float deadZone;
    [Header("Offset for the Camera")]
    [Range(0,20)]
    [SerializeField]
    private float offSetY;
    [Range(-20,20)]
    [SerializeField]
    private float offSetZ;
    [Range(0,20)]
    [SerializeField]
    private float highSet;
    [SerializeField]
    private float turnSpeed;


    Quaternion rotationX;
    Quaternion rotationY;

    Vector3 offSet;
    Vector3 offSetNew;
    Vector3 offSetOld;


    private void Start()
    {
        offSet = new Vector3(0, offSetY, offSetZ);
        offSetNew = offSet;
        offSetOld = offSet;
        lookAt = playerTarget;
    }

    private void Update()
    {
        CameraMove();
    }

    private void FixedUpdate()
    {
        offSetNew = new Vector3(0, offSetY, offSetZ);

        if (offSetOld != offSetNew)
        {
            offSet = offSetNew;
            offSetOld = offSetNew;
        }
        if(lookAt == playerTarget)
        {
            lookAt.transform.position = new Vector3(playerTarget.transform.position.x, playerTarget.transform.position.y, playerTarget.transform.position.z);
        }
    }
    public void CameraMove()
    {
        if (lookAt == playerTarget)
        {
            if (Input.GetAxisRaw("CameraHorizontal") > deadZone ||
               Input.GetAxisRaw("CameraHorizontal") < -deadZone)
            {
                rotationX = Quaternion.AngleAxis(Input.GetAxisRaw("CameraHorizontal") * turnSpeed, Vector3.up);
                //rotationY = Quaternion.AngleAxis(inputPackage.CameraVertical, Vector3.right);
                offSet = rotationX * offSet;
            }
                transform.position = playerTarget.transform.position + offSet;
            
        }
        else
        {
            transform.position = (playerTarget.transform.position + offSet) - enemyTarget.transform.position;
        }
        transform.LookAt(lookAt.transform);
    }
    public void ChangeState()
    {
        if(lookAt == playerTarget)
        {
            lookAt = enemyTarget;
        }
        else
        {
            lookAt = playerTarget;
        }
    }
}
