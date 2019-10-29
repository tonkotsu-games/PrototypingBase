using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingUpdate
{
    public void Heading(Vector3 head, Transform transform, FreeLookCamera cam)
    {
        if (!cam.lockOn)
        {
            if (head == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(head);
            }
        }
        else if (cam.lockOn)
        {
            cam.lookAt.y = 0;
            transform.rotation = Quaternion.LookRotation(cam.lookAt);
        }
    }
}