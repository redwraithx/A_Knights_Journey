using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditArcher_ShootController : MonoBehaviour
{
    //this is the time to beat the game... make it to the end of the map in this time
    public float mTotalGameTime = 30.0f;
    public float mTriggerPenaltyTime = 5.0f;

    //this is for tweaking difficulty
    public float mCalculateSpeedTime = 1.0f;

    public float mStrafeSpeed = 10.0f;

    //in the case we want velocity defined from current position to destination
    public GameObject targetDestination = null;

    //in the case we know the distance and want velocity based off that
    public float targetDistance = 1.0f;

    private bool _mIsRunning = false;
    private Rigidbody _mRb = null;
    private Vector3 _mLinearVelocity = Vector3.zero;
    private Vector3 _mStrafeVelocity = Vector3.zero;


	// Use this for initialization
	void Start () {
        _mRb = GetComponent<Rigidbody>();

    }

	void DisplayDestination()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //based on velocity and time, place cube at the end position
        //calculate destinaton
        cube.GetComponent<Collider>().enabled = false;
        Vector3 endPosition = this.transform.position + (_mLinearVelocity * mTotalGameTime);
        cube.transform.position = endPosition;
    }

    float CalculateSpeed(float time, float distance)
    {
        if(time < Mathf.Epsilon)
        {
            return 0.0f;
        }

        return distance/time;
    }

    Vector3 CalculateVelocity(float totalTime, Vector3 destination)
    {
        if(totalTime < Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        Vector3 deltaDistance = destination - transform.position;
        return deltaDistance/totalTime;
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_mIsRunning)
            {
                _mIsRunning = true;
                if (targetDestination)
                {
                    _mLinearVelocity = CalculateVelocity( mCalculateSpeedTime , targetDestination.transform.position);
                }
                else
                {
                    //DisplayDestination();
                    _mLinearVelocity = transform.forward * CalculateSpeed(mCalculateSpeedTime, targetDistance);
                }
            }
        }
        
        if(_mIsRunning)
        {
            _mStrafeVelocity = transform.right * (Input.GetAxis("Horizontal") * mStrafeSpeed);
        }

	}

    void FixedUpdate()
    {
        if(_mIsRunning && mTotalGameTime > Mathf.Epsilon)
        {
            //adjust y velocity to current rigid body y velocity
            _mLinearVelocity.y = _mRb.velocity.y;
            _mRb.velocity = _mLinearVelocity + _mStrafeVelocity;
            mTotalGameTime -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        mTotalGameTime -= mTriggerPenaltyTime;
    }
}
