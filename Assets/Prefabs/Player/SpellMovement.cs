using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMovement : MonoBehaviour
{
    public PlayersMagic magicScript = null;

    public Vector3 p0Pos;
    public Vector3 p1Pos;
    public Vector3 p2Pos;

    public Transform startPoint;
    public Transform endPoint;
    public Transform heightPoint;

    private float accumulatedTime = 0f;
    public float iterationDuration = 3.0f;

    private Vector3[] splinePosArray = new Vector3[100];

    public float gizmoSplineSphereRadius = 0.3f;

    private void Start()
    {

        if (!magicScript)
            magicScript = GetComponentsInParent<PlayersMagic>()?[0];

        if (magicScript)
        {
            p0Pos = magicScript.startPosition.transform.position;
            p1Pos = magicScript.maxHeightPosition.transform.position;
            p2Pos = magicScript.endPosition.transform.position;
        }
        else
        {
            startPoint.position = p0Pos;
            heightPoint.position = p1Pos;
            endPoint.position = p2Pos;
        }

        accumulatedTime = 0.0f;
        
        
    }


    private void Update()
    {
        
        if (magicScript)
        {
            p0Pos = magicScript.startPosition.transform.position;
            p1Pos = magicScript.maxHeightPosition.transform.position;
            p2Pos = magicScript.endPosition.transform.position;
        }
        else
        {
            startPoint.position = p0Pos;
            heightPoint.position = p1Pos;
            endPoint.position = p2Pos;
        }
        
        //accumulatedTime += Time.deltaTime;
        //float t = TimeFunction(accumulatedTime);

        

        float timeStep = (float)1 / 100;
        float t = 0.0f;

        for (int i = 1; i < 100; ++i)
        {
            t += timeStep;
            
            splinePosArray[i] = Vector3.Lerp(Vector3.Lerp(p0Pos, p1Pos, t), Vector3.Lerp(p1Pos, p2Pos, t), t);
        }

        // move the object
        accumulatedTime += Time.deltaTime;

        float transitionT = TimeFunction(accumulatedTime);

        if (transitionT < 1.0f)
        {
            transform.position = Vector3.Lerp(Vector3.Lerp(p0Pos, p1Pos, transitionT), Vector3.Lerp(p1Pos, p2Pos, transitionT), transitionT);
        }
    }


    private float TimeFunction(float currentTime)
    {
        if (iterationDuration > 0.0f)
        {
            return currentTime / iterationDuration;
        }

        return 0.0f;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < 100; ++i)
        {
            Gizmos.DrawSphere(splinePosArray[i], gizmoSplineSphereRadius);
        }
    }
}
