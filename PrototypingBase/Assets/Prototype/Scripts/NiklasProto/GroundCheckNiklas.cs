using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckNiklas : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }

    private void OnGUI()
    {
        GUILayout.Toggle(isGrounded, "isGrounded");
    }

}
