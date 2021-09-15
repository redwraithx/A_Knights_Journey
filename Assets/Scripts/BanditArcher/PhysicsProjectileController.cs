

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectileController : MonoBehaviour {

    // 𝑣_1 = 𝑣_0 +𝐴𝑐𝑐𝑒𝑙 ∗ ∆𝑡
    // 𝑝_1 = 𝑝_0 +𝑣_0∗∆𝑡+0.5∗𝐴𝑐𝑐𝑒𝑙∗∆𝑡^2
    //〖𝑣′〗_1 = ((𝑝_1 +𝑣_0 ))/∆𝑡

    public float m_totalTime = 1.0f;
    //in degrees per second
    public float turnSpeed = 90.0f;
    public float rayDistance = 20.0f;
    public GameObject m_verticalMarker = null;
    public GameObject m_horizontalMarker = null;

    public GameObject m_projectile = null;
    public Vector3 m_initialVelocity = Vector3.zero;
    public Slider slider = null;

    private GameObject m_target = null;

    private Rigidbody m_rb = null;
    private float m_startTime = 0.0f;

    [SerializeField] private List<Vector3> gizmoPos = new List<Vector3>();
    [SerializeField] private List<GameObject> shotPathList = new List<GameObject>();

    private bool freezeProjection = false;
    private float freezeDuration = 0.0f;
    private float freezeCurrTime = 0.0f;
    
    public GameObject shotPathPrefabObject = null;

	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();


        if (!shotPathPrefabObject)
            throw new System.Exception("Error missing Shot Path Prefab GameObject");


        for (int i = 0; i < 20000; i++)
        {
            GameObject shotPathGO = Instantiate(shotPathPrefabObject, transform.position, Quaternion.identity);
            shotPathList.Add(shotPathGO);
            
        }
        
        //in case we start and are looking at a target
        UpdateRaycast();
	}
	
    public Vector3 GetDisplacement(Vector3 initialVelocity, Vector3 acceleration, float time)
    {
        return initialVelocity*time +(0.5f * acceleration * time * time);
    }

    Vector3 GetSquaredVector(Vector3 vecToSquare)
    {
        return new Vector3(vecToSquare.x * vecToSquare.x, vecToSquare.y * vecToSquare.y, vecToSquare.z * vecToSquare.z);
    }

    Vector3 GetSquareRootedVector(Vector3 vecToSquareRoot)
    {
        float length = Mathf.Sqrt(vecToSquareRoot.magnitude);
        //normalize will make this vector a length of 1, multiplying by sqrt length gives us sqrt
        return vecToSquareRoot.normalized * length;
    }

    public Vector3 GetFinalVelocity(Vector3 displacement, Vector3 initialVelocity, Vector3 acceleration)
    {
        //calculate final velocity
        //vf^2 = vi^2 + 2ad
        //don't forget to square root your vf^2
        //magnitude gives the length, and that is what we want from displacement
        Vector3 finalVelocitySq = GetSquaredVector(initialVelocity) + (2.0f * acceleration * displacement.magnitude);
        Debug.Log("FinalVelocity: " + GetSquareRootedVector(finalVelocitySq));
        return GetSquareRootedVector(finalVelocitySq);
    }

    public float GetFlightTime(Vector3 displacement, Vector3 initialVelocity, Vector3 acceleration)
    {

        //a = (Vf - Vi)/t
        //t = displacement/((Vf + Vi)/2)
        Vector3 finalVelocity = GetFinalVelocity(displacement, initialVelocity, acceleration);
        float time = (displacement.magnitude) / ((finalVelocity.magnitude + initialVelocity.magnitude) / 2.0f);
        Debug.Log("Time: " + time);
        return time;
    }

    void UpdateRaycast()
    {
        if(m_target != null)
        {
            //restore old colour of object
            m_target.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        m_target = null;
        Ray ray = new Ray();
        ray.direction = transform.forward * rayDistance;
        ray.origin = transform.position;
        RaycastHit hitInfo = new RaycastHit();
        //take forward vector, raycast data set radius
        if(Physics.Raycast(ray, out hitInfo))
        {
            if(hitInfo.collider.gameObject.tag.Contains("Target"))
            {
                //hit a target, make it green
                m_target = hitInfo.collider.gameObject;
                m_target.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    Vector3 GetProjectileInitialVelocity()
    {
        //can use any height, and can base it off upward aim
        Vector3 verticalDisplacement = transform.position - m_verticalMarker.transform.position;
        verticalDisplacement.x = 0.0f;
        verticalDisplacement.z = 0.0f;
        Vector3 finalVelocity = GetFinalVelocity(verticalDisplacement, Vector3.zero, Physics.gravity);
        //since i calculated the final velocity from height to floor, to get to the height i just need to negate this
        Vector3 initialVelocity = finalVelocity * -1.0f;

        float flightTime = GetFlightTime(verticalDisplacement, Vector3.zero, Physics.gravity);
        ////above formula gets 1/2 of the flight time, multiply by 2 to get the total duration
        flightTime *= 2.0f;
        //flatten out the y component, make sure y is 0
        Vector3 horizontalDisplacement = m_target.transform.position - transform.position;
        horizontalDisplacement.y = 0.0f;
        Vector3 horizontalVelocity = (horizontalDisplacement / flightTime);
        initialVelocity += horizontalVelocity;
        return initialVelocity;
    }

    void ShootProjectile()
    {
        // Instantiate the projectile at the position and rotation of this transform
        GameObject projectile = Instantiate(m_projectile, transform.position, transform.rotation);
        projectile.AddComponent<Rigidbody>();

        projectile.GetComponent<Rigidbody>().velocity = m_initialVelocity;

        freezeProjection = true;
        freezeCurrTime = 0.0f;
    }

    void UpdateInitialVelocity()
    {
        float maxSpeed = 20.0f;
        Vector3 projectileDirection = (transform.forward + transform.up);

        if (m_target != null)
        {
            projectileDirection = GetProjectileInitialVelocity();
            float correctSpeed = projectileDirection.magnitude;
            //make this a unit vector, ie size 1
            projectileDirection.Normalize();
            //if we want the half way point of the slider to be the correct amount
            //we set max speed to be correct speed * 2
            maxSpeed = correctSpeed * 2.0f;
        }

        float speed = slider.normalizedValue * maxSpeed;
        m_initialVelocity = projectileDirection * speed;
    }

    // Update is called once per frame
    void Update () {

        Vector3 turnVelocity = Input.GetAxis("Horizontal") * transform.up * Mathf.Deg2Rad * turnSpeed;
        if(turnVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            //only update when we are aiming
            UpdateRaycast();
        }
        //rotate left or right based on input
        m_rb.angularVelocity = turnVelocity;

        UpdateInitialVelocity(); //m_initialVelocity is now accurate

        float x0 = transform.position.x;
        float y0 = transform.position.y;
        float z0 = transform.position.z;

        if (freezeProjection)
        {
            freezeCurrTime += Time.deltaTime;

            if (freezeCurrTime > freezeDuration)
            {
                freezeProjection = false;
            }
        }
        else
        {
            gizmoPos.Clear();

            //𝑡=(2𝑣_𝑦)/𝑔
            float maxGizmoTime = (2.0f * m_initialVelocity.y) / 9.8f;
            freezeDuration = maxGizmoTime;

            //𝑟=(2𝑣_𝑥 𝑣_𝑦)/𝑔
            float maxDistanceX = (2.0f * m_initialVelocity.x * m_initialVelocity.y) / 9.8f;
            float maxDistanceZ = (2.0f * m_initialVelocity.z * m_initialVelocity.y) / 9.8f;
            float timeStep = maxGizmoTime / Mathf.Max(maxDistanceX, maxDistanceZ);
            timeStep = timeStep / 100.0f;

            for (float projTime = 0.0f; projTime < maxGizmoTime; projTime += timeStep)
            {
                // 𝑥(𝑡)=𝑥_0+𝑣_0 𝑡+1/2 𝑔𝑡^2

                //𝑥(𝑡)= 𝑥_0 +𝑣_𝑥 𝑡
                //𝑦(𝑡)= 𝑦_0 + 𝑣_𝑦 𝑡−1 / 2 𝑔𝑡^2
                //𝑧(𝑡)= 𝑧_0 +𝑣_𝑧 𝑡
                Vector3 newPos = Vector3.zero;
                newPos.x = x0 + m_initialVelocity.x * projTime;
                newPos.y = y0 + (m_initialVelocity.y * projTime) - (0.5f * 9.8f * (projTime*projTime));
                newPos.z = z0 + m_initialVelocity.z * projTime;
                gizmoPos.Add(newPos);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootProjectile();
        }

        UpdateShotPath();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //get time difference
        float elapsedTime = (Time.time - m_startTime);
        Debug.Log("Time Elapsed: " + elapsedTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        float elapsedTime = (Time.time - m_startTime);
        Debug.Log("Time Elapsed: " + elapsedTime);
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;

        foreach (Vector3 positions in gizmoPos)
        {
            Gizmos.DrawSphere(positions, 0.01f);
        }
    }

    // this needs  to be more realistic
    void UpdateShotPath()
    {
        int index;
        for (index = 0; index < gizmoPos.Count; index++)
        {
            shotPathList[index].transform.position = gizmoPos[index];
        }

        // resets the position for all objects not currently in the target range back to the canons shot pos;
        for (; index < shotPathList.Count; index++)
        {
            shotPathList[index].transform.position = transform.position;
        }
    }
}
