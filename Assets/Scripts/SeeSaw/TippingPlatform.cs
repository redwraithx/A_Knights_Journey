using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TippingPlatform : MonoBehaviour
{
    public GameObject player = null;
    public Rigidbody rb = null;
    public float movingSpeed = 50f;
    private float gravity = 9.81f;
    public bool isBeingStandOn = false;
    private Vector3 newOrientation = Vector3.zero;

    private void Start()
    {
        
    }


    private void FixedUpdate()
    {
        if (isBeingStandOn)
        {
            newOrientation = new Vector3(player.transform.position.x - rb.transform.position.x, 0f, 0f);
            newOrientation.Normalize();
        }
        else
        {
            newOrientation = Vector3.zero;
        }

        float angle = Vector3.Angle(newOrientation, transform.up);
        
        Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, newOrientation);
        var moveStep = (movingSpeed + gravity) * Time.fixedDeltaTime;
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, deltaRotation, moveStep);
        
        rb.MoveRotation(newRotation);

    }
}
