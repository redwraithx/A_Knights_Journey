
using System;
using EnemyStates;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
 
     public float targetHeight = 1.7f;
     public float distance = 5.0f;
     public float offsetFromWall = 0.1f;
 
     public float maxDistance = 20;
     public float minDistance = .6f;
     public float speedDistance = 5;
 
     public float xSpeed = 200.0f;
     public float ySpeed = 200.0f;
 
     public int yMinLimit = -40;
     public int yMaxLimit = 80;
 
     public int zoomRate = 40;
 
     public float rotationDampening = 3.0f;
     public float zoomDampening = 5.0f;
 
     public LayerMask collisionLayers = -1;
 
     private float xDeg = 0.0f;
     private float yDeg = 0.0f;
     private float currentDistance;
     private float desiredDistance;
     private float correctedDistance;
 
     void Start ()
     {
         Vector3 angles = transform.eulerAngles;
         xDeg = angles.x;
         yDeg = angles.y;
 
         currentDistance = distance;
         desiredDistance = distance;
         correctedDistance = distance;
 
         // Make the rigid body not change rotation
         if (gameObject.GetComponent<Rigidbody>())
             gameObject.GetComponent<Rigidbody>().freezeRotation = true;
     }

     private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible = true;
         }
         else if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
         {
             Cursor.lockState = CursorLockMode.Locked;
             Cursor.visible = false;
         }
     }


     void LateUpdate ()
     {
         Vector3 vTargetOffset;
 
         // Don't do anything if target is not defined
         if (!target)
             return;
 
         xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
         yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
 
 
         // calculate the desired distance
         desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance) * speedDistance;
         desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
 
         yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
 
         // set camera rotation
         Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
         correctedDistance = desiredDistance;
 
         // calculate desired camera position
         vTargetOffset = new Vector3 (0, -targetHeight, 0);
         Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
 
         // check for collision using the true target's desired registration point as set by user using height
         RaycastHit collisionHit;
         Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y, target.position.z) - vTargetOffset;
 
         // if there was a collision, correct the camera position and calculate the corrected distance
         bool isCorrected = false;
         if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers.value))
         {
             // calculate the distance from the original estimated position to the collision location,
             // subtracting out a safety "offset" distance from the object we hit.  The offset will help
             // keep the camera from being right on top of the surface we hit, which usually shows up as
             // the surface geometry getting partially clipped by the camera's front clipping plane.
             correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
             isCorrected = true;
         }
 
         // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
         currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;
 
         // keep within legal limits
         currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
 
         // recalculate position based on the new currentDistance
         position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);
 
         transform.rotation = rotation;
         transform.position = position;
     }
 
     private static float ClampAngle (float angle, float min, float max)
     {
         if (angle < -360)
             angle += 360;
         if (angle > 360)
             angle -= 360;
         return Mathf.Clamp (angle, min, max);
     }
     
     
     
}
