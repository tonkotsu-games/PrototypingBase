using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Level_Cinematic : MonoBehaviour
{
    [SerializeField]
    Camera_Playmyani cameraChange;

    public void ChangeBack()
    {
        cameraChange.EndOfAnimation();
    }
}
