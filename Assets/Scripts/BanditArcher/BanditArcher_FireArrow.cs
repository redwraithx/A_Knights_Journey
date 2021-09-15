using System;
using System.Collections;
using TreeEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BanditArcher_FireArrow : MonoBehaviour 
{
    
    //public Transform target;
    public Vector3 shotTargetLocation;
    
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Transform projectile;
    private Transform _myTransform;

    public float shotDelayTime = 1.5f;
    
    
    public LayerMask arrowHitLayerMask;
    public RaycastHit arrowHit;
    public float arrowRaycastLength = 2f;

    public int arrowDamage = 10;
    public float knockBackForcePerArrow = 0.1f;

    public bool isArrowFlying = true;


    void Awake()
    {
        _myTransform = transform;
    }
 
    public void Start()
    {
        Debug.Log("spawning object");
        //Instantiate(Resources.Load("PlayersLastKnownPosition_TrackingPoint"), new Vector3(shotTargetLocation.x, shotTargetLocation.y + 1f, shotTargetLocation.z), Quaternion.identity);
        
        StartCoroutine(SimulateProjectile());
    }


    private void Update()
    {
        // raycast to check for hit
        // this will let you deal damage
        // or to remove the arrow from the scene
        
    }
    
    
    
    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * arrowRaycastLength, Color.magenta);
        
        // raycast for player or valid hit
        if (HasHitObject() && arrowHit.collider != null)
        {
            if (arrowHit.collider.CompareTag("Player"))
            {
                Debug.Log("hit Player");

                arrowHit.collider.GetComponent<PlayersHealth>()
                    .TakeDamageFromAttack(arrowDamage, Vector3.zero, 0f); //arrowHit.transform.position - transform.forward, knockBackForcePerArrow);
                
                Destroy(gameObject);
                
            }
            else if ( arrowHit.collider.CompareTag("Ground") || arrowHit.collider.CompareTag("Platform") || arrowHit.collider.CompareTag("Wall") )
            {
                Debug.Log("Hit a ground or platform or a wall");
                
                Destroy(gameObject);
            }
        }
        
        
    }

    public bool HasHitObject()
    {
        if (!isArrowFlying)
            return false;
        
        Ray ray = new Ray(transform.position, transform.forward);

        return Physics.SphereCast(transform.position, 0.5f, transform.forward - transform.position, out arrowHit, arrowRaycastLength,
            arrowHitLayerMask); //arrowHit, arrowRaycastLength, arrowHitLayerMask);
    }
    

    IEnumerator SimulateProjectile()
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(shotDelayTime);
       
        // Move projectile to the position of throwing object + add some offset if needed.
        projectile.position = _myTransform.position + new Vector3(0, 0.0f, 0);
       
        // Calculate distance to target
        //float target_Distance = Vector3.Distance(projectile.position, target.position);
        float target_Distance = Vector3.Distance(projectile.position, shotTargetLocation);
 
        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
 
        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
 
        // Calculate flight time.
        float flightDuration = target_Distance / Vx;
   
        // Rotate projectile to face the target.
        //projectile.rotation = Quaternion.LookRotation(target.position - projectile.position);
        projectile.rotation = Quaternion.LookRotation(shotTargetLocation - projectile.position);
       
        float elapse_time = 0;
 
        while (elapse_time < flightDuration)
        {
            projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
           
            elapse_time += Time.deltaTime;
 
            yield return null;
            
            
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log("removing rigidbody component off arrow");
        Destroy(GetComponent<Rigidbody>());
        
        Debug.Log("removing collider components off arrow");
        foreach(var col in GetComponents<Collider>())
            Destroy(col);

        yield return new WaitForSeconds(10f);
        
        Debug.Log("destorying this arrow");
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("collision with: " + other.gameObject.name);
            
            Debug.Log("removing rigidbody component off arrow on collision");
            Destroy(GetComponent<Rigidbody>());
        
            Debug.Log("removing collider components off arrow on collision");
            foreach(var col in GetComponents<Collider>())
                Destroy(col);
        }
    }
}
