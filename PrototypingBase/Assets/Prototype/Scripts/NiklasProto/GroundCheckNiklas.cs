using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckNiklas : MonoBehaviour
{
    private bool isGrounded;
    public bool IsGrounded { get => isGrounded; }

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