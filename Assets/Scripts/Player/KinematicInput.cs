
using System;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// players movement will transition to a new file i have been working on but its still in progress

public class KinematicInput : MonoBehaviour
{
    [Header("My References")]
    
    Rigidbody rigidBody;
    public Animator anim;
    public Camera m_MainCamera = null;
    public Transform playersForwardDirectionNode = null;
    
    [Header("Other World Object References")]
    public GameObject currentPlatform = null;
    
    [Header("UI References")]
    // UI References
    public GameObject gameOverWinUI = null;
    public GameObject playerHealthUICOntainer = null;
    
    //[Header("Enemy References")]
    // enemy3 boo
    //public Transform enemyBoo = null;
    
    [Header("Collectable Variables")]
    // win condition variables
    private const int maxTreasures = 4;
    [SerializeField] private int _currentCollectedTreasures = 0;

    [Header("My Speed Variable")]
    // Horizontal movement parameters
    public float speed = 10.0f;
    
    [Header("My Jumping Variables")]
    // Jump and Fall parameters
    public float maxJumpSpeed = 1.5f;
    public float maxDoubleJumpSpeed = 0.75f;
    public float maxFallSpeed = -2.2f;
    public float timeToMaxJumpSpeed = 0.2f;
    public float deccelerationDuration = 0.0f;
    public float maxJumpDuration = 1.2f;
    public float maxDoubleJumpDuration = 0.2f;

    [Header("My Jumping / Falling / Gravity Helpers")]
    // Jump and Fall helpers
    bool jumpStartRequest = false;
    bool jumpRelease = false;
    public bool isMovingUp = false;
    public bool isFalling = false;
    public bool isDoubleJumping = false;
    float currentJumpDuration = 0.0f;
    float gravityAcceleration = -9.8f;
    
    
    [Header("My Falling Variables")]
    // fall damage variables
    public bool willTakeFallDamage = false;
    public float fallDamageHeight = 2.01f;
    public float currentFallingHeight = 0f;
    public int fallDamageValue = 15;
    public int fallDamageApplied = 0;
    
    [Header("My Falling Misc")]
    public bool startFallingDelayTimer = false;
    public float fallDelayTime = 1f;
    public float currentFallDelayTimer = 0f;
    
    [Header("My Knock back distance")]
    public float distanceToKnockback = 0.9f;
    
    [Header("Wall Detection, can i move forward?")]
    // wall collision
    public bool canMoveForward = true;
    
    [Header("Ground / Head / Wall Collision lengths")]
    public float groundSearchLength = 0.6f;
    public float headBumpCheckLength = 0.65f;
    public float checkForWallCollisionForward = 0.5f;
    
    [Header("Hit Enemy's Head Distance")]
    // hit enemy
    private RaycastHit currentHitEnemyHit;
    public float hitEnemyOnHeadDistance = 0.62f;
    
    // distance to object below player
    private RaycastHit distanceToObjectBelowPlayer;

    // ray casts for ground, platforms and walls
    RaycastHit currentGroundHit;

    [Header("Hit Head on Platform BoxCast Size")]
    private RaycastHit currentHeadHit;
    public Vector3 headHitBoxSize = new Vector3(0.5f, 0.2f, 0.5f);
    
    [Header("Wall Detection SphereCast Size")]
    private RaycastHit currentWallHit;
    public float wallHitRadius = 0.4f;
    
    [Header("My LocalForward Vector")]
    public Vector3 localForward = Vector3.zero;
    
    
    // Rotation Parameters
    float angleDifferenceForward = 0.0f;

    // rotating platform
    public bool startAnimatorJump = false;

    // Components and helpers
    
    Vector2 input;
    Vector3 playerSize;

    [Header("GuiStyle")]
    // Debug configuration
    public GUIStyle myGUIStyle;


    private Vector3 previousPosition = Vector3.zero;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Jump = Animator.StringToHash("Jump");

    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerSize = GetComponent<Collider>().bounds.size;
    }

    void Start()
    {
        if (!gameOverWinUI)
            throw new Exception("Error missing gameOverWinUI reference");

        if (!playerHealthUICOntainer)
            throw new Exception("Error missing playerHealthUIContainer reference");

        if (!playersForwardDirectionNode)
            throw new Exception("Error missing players local forward position empty transform");
        
        if(!anim)
            anim = GetComponent<Animator>();

        //if (!enemyBoo)
        //    throw new Exception("Error missing boo reference");
        
        
        jumpStartRequest = false;
        jumpRelease = false;
        isMovingUp = false;
        isFalling = false;
        isDoubleJumping = false;

        currentFallDelayTimer = fallDelayTime;
        canMoveForward = true;
    }

    void Update()
    {


        localForward = (playersForwardDirectionNode.position - transform.position);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        input = new Vector2();
        input.x = horizontal;
        input.y = vertical;

        if (Input.GetButtonDown("Jump"))
        {
            jumpStartRequest = true;
            jumpRelease = false;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpRelease = true;
            jumpStartRequest = false;
        }

        DelayedFallTimer();


        // if (_currentCollectedTreasures >= maxTreasures)
        //     LoadWinningScene();

        // Update the players animations from idle to walk to run
        if (isOnGround() && !HasHitWall())
            anim.SetFloat(MoveSpeed, rigidBody.velocity.magnitude);
        else
            anim.SetFloat(MoveSpeed, 0f);

    }

    void LateUpdate()
    {
        
        previousPosition = transform.position;
    }

    void StartFalling()
    {
        isMovingUp = false;
        isFalling = true;
        currentJumpDuration = 0.0f;
        jumpRelease = false;
    }

    void FixedUpdate()
    {
        //LookAtBoo();

        if (HasHitWall() && currentWallHit.collider != null)
        {
            if (currentWallHit.collider.gameObject.CompareTag($"Wall"))
            {
                if (Vector3.Distance(currentWallHit.point, transform.position) < 0.4f)
                    return;

                canMoveForward = false;
            }
        }
        // else if (HasHitWall() && currentWallHit.collider != null)
        // {
        //     if (currentWallHit.collider.gameObject.CompareTag($"Treasure") && currentWallHit.distance <= 0.2f)
        //     {
        //         _currentCollectedTreasures++;
        //         Destroy(currentWallHit.collider.gameObject, 20f);
        //     }
        // }
        else
            canMoveForward = true;

        // Calculate horizontal movement
        // Vector3 movement = m_MainCamera.transform.right * (input.x * speed * Time.deltaTime);
        // movement += m_MainCamera.transform.forward * (input.y * speed * Time.deltaTime);
        // movement.y = 0.0f;

        Vector3 movement = Vector3.zero;
        
        
        if (input.x > 0.001f || input.x < -0.001f || input.y > 0.001f || input.y < -0.001f)
        {
            movement = m_MainCamera.transform.right * input.x * speed * Time.deltaTime;
            movement += m_MainCamera.transform.forward * input.y * speed * Time.deltaTime;
            movement.y = 0.0f;
            
            
        }
        
        //Debug.Log("my rigidbody positions is: " + rigidBody.position.ToString() + ", Movement is: " + movement);
        
        
        Vector3 targetPosition = rigidBody.position + movement;
        

        // Calculate Vertical movement
        float targetHeight = 0.0f;
        float targetHeadHeight = 0.0f;

        if (!isMovingUp && jumpStartRequest && isOnGround())
        {
            isMovingUp = true;
            jumpStartRequest = false;
            currentJumpDuration = 0.0f;
        }

        // if in the air and coming down
        //if (isFalling)
            // if (HasHitEnemy() && currentHitEnemyHit.collider != null)
            // {
            //     var dist = Vector3.Distance(transform.position, currentHitEnemyHit.transform.position);
            //     
            //     if (currentHitEnemyHit.collider.CompareTag("Enemy") && Mathf.Abs(dist) <= 1f)
            //         currentHitEnemyHit.collider.gameObject.GetComponent<EnemyHealth>().TakeHitFromPlayer(50);
            //     
            // }
    

        if (isMovingUp)
        {
            if (HasHitHead() && currentHeadHit.collider.gameObject.CompareTag($"Platform"))
            {
                willTakeFallDamage = false;
                fallDamageApplied = 0;
                
                StartFalling();
            }
            
            //if (jumpRelease || currentJumpDuration >= maxJumpDuration)
            if (jumpRelease || currentJumpDuration >= maxJumpDuration && !isDoubleJumping || currentJumpDuration >= maxDoubleJumpDuration && isDoubleJumping)
            {
                if (!startFallingDelayTimer && !isDoubleJumping)
                {
                    startFallingDelayTimer = true;

                    startAnimatorJump = false;
                }
                else if (isDoubleJumping)
                {
                    //startAnimatorJump = false;
                    
                    willTakeFallDamage = false;
                    fallDamageApplied = 0;
                    
                    StartFalling();
                }
                    
            }
            else
            {
                float jumpSpeed;

                if (!startAnimatorJump)
                {
                    anim.ResetTrigger(Jump);
                    anim.SetTrigger(Jump);

                    startAnimatorJump = true;
                }
                
                if (!isDoubleJumping)
                    jumpSpeed = maxJumpSpeed;
                else //if(isDoubleJumping)
                    jumpSpeed = maxDoubleJumpSpeed;
                
                float currentYpos = rigidBody.position.y;
                float newVerticalVelocity = jumpSpeed + gravityAcceleration * Time.deltaTime;
                targetHeight =  currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * jumpSpeed * Time.deltaTime * Time.deltaTime);
                
                currentJumpDuration += Time.deltaTime;
            }
        }
        else if (!isOnGround())
        {
            StartFalling();
        }

        if (isFalling)
        {
            if (/* CheckDistanceBelow() && */ distanceToObjectBelowPlayer.collider != null)
            {
                currentFallingHeight = Vector3.Distance(distanceToObjectBelowPlayer.point, transform.position + new Vector3(0f, 0f, 0f));
                
                Vector3 currentFacingY = -transform.up;
                currentFacingY.x = 0.0f;
                currentFacingY.z = 0.0f;
                Debug.DrawLine(rigidBody.position, rigidBody.position + currentFacingY * currentFallingHeight, Color.magenta, 0.0f, false);

                if (currentFallingHeight > fallDamageHeight)
                {
                    float height = (currentFallingHeight < fallDamageHeight ? 0f : currentFallingHeight - 2f); // 2f is player height * 2
                    float damage = fallDamageValue * height;

                    if (damage > fallDamageApplied)
                        fallDamageApplied = Convert.ToInt32(damage);

                    if (currentFallingHeight >= fallDamageHeight)
                    {
                        currentFallingHeight = 0f;
                        willTakeFallDamage = true;
                    }
                }
                else if(!willTakeFallDamage)
                    fallDamageApplied = 0;


            }
            
            if (isOnGround())
            {
                // End of falling state. No more height adjustments required, just snap to the new ground position
                isFalling = false;
                isDoubleJumping = false;
                //targetHeight = currentGroundHit.point.y;// + (0.5f * playerSize.y);
                if (currentGroundHit.collider != null)
                    targetHeight = currentGroundHit.point.y + groundSearchLength;

                if (willTakeFallDamage)
                {
                    GetComponent<PlayersHealth>().TakeDamage(fallDamageApplied);

                    willTakeFallDamage = false;
                    fallDamageApplied = 0;
                    
                    Debug.Log("Take damage from falling to high");
                }
            }
            else if (!isDoubleJumping && Input.GetButtonDown("Jump"))
            {
                isFalling = false;
                isMovingUp = true;
                isDoubleJumping = true;

                willTakeFallDamage = false;
                fallDamageApplied = 0;
                
                startFallingDelayTimer = false;
                currentFallDelayTimer = fallDelayTime;
            }
            else
            {
                float currentYpos = rigidBody.position.y;
                float currentYvelocity = rigidBody.velocity.y;

                float newVerticalVelocity = maxFallSpeed + gravityAcceleration * Time.deltaTime;
                targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * maxFallSpeed * Time.deltaTime * Time.deltaTime);
            }

        }

        if ( targetHeight > Mathf.Epsilon)
        {
            // Only required if we actually need to adjust height
            targetPosition.y = targetHeight;
        }
   
        // Calculate new desired rotation
        Vector3 movementDirection = targetPosition - rigidBody.position;
        movementDirection.y = 0f;
        movementDirection.Normalize();
            
        Vector3 currentFacingXZ = transform.forward;
        currentFacingXZ.y = 0.0f;
        
        

        angleDifferenceForward = Vector3.SignedAngle(movementDirection, currentFacingXZ, Vector3.up);
        Vector3 targetAngularVelocity = Vector3.zero;
        targetAngularVelocity.y = angleDifferenceForward * Mathf.Deg2Rad;

        Quaternion syncRotation = quaternion.identity;
        
        if(movementDirection != Vector3.zero)
            syncRotation = Quaternion.LookRotation(movementDirection);

        Debug.DrawLine(rigidBody.position, rigidBody.position + movementDirection * 2.0f, Color.green, 0.0f, false);
        
        // Forward debugline
        Debug.DrawLine(rigidBody.position, rigidBody.position + currentFacingXZ * 2.0f, Color.cyan, 0.0f, false);
        
        
        

        
        if (canMoveForward)
            rigidBody.MovePosition(targetPosition);
        else
            rigidBody.MovePosition(new Vector3(transform.position.x, targetPosition.y, transform.position.z));
        
        
        if (syncRotation != Quaternion.identity && movement.magnitude > 0.01f)
        {
            // Currently we only update the facing of the character if there's been any movement
            rigidBody.MoveRotation(syncRotation);
        }
        
        // platform adjustments
        if (currentGroundHit.collider != null)
        {
            if (currentGroundHit.collider.gameObject.CompareTag($"SeeSawPlatform"))
                currentPlatform = currentGroundHit.collider.gameObject;
            else if (currentPlatform != null)
            {
                currentPlatform.GetComponent<TippingPlatform>().isBeingStandOn = false;
                currentPlatform = null;
            }
            
            if(currentPlatform != null)
            {
                if (isOnGround())
                    currentPlatform.GetComponent<TippingPlatform>().isBeingStandOn = true;
                else
                    currentPlatform.GetComponent<TippingPlatform>().isBeingStandOn = false;
            }
            
        }

        if (currentGroundHit.collider != null)
        {
            Debug.DrawLine(transform.position, currentGroundHit.point, Color.black);
        }
    }

    // private void LookAtBoo()
    // {
    //     float distance = Vector3.Distance(transform.position, enemyBoo.position);
    //     Vector3 targetDirection = enemyBoo.position - transform.position;
    //
    //     float angle = Vector3.Angle(targetDirection, localForward);
    //
    //     if (Mathf.Abs(distance) > 10)
    //     {
    //         if (angle < 10.0f)
    //             enemyBoo.GetComponent<Vision>().playerSeesBoo = true;
    //         else 
    //             enemyBoo.GetComponent<Vision>().playerSeesBoo = false;
    //     }
    //     else
    //     {
    //         if (angle < 30.0f) 
    //             enemyBoo.GetComponent<Vision>().playerSeesBoo = true;
    //         else
    //             enemyBoo.GetComponent<Vision>().playerSeesBoo = false;
    //     }
    // }
    
    
    private void DelayedFallTimer()
    {
        if (!startFallingDelayTimer)
            return;
        
        currentFallDelayTimer -= Time.deltaTime;

        if (currentFallDelayTimer <= 0f)
        {
            startFallingDelayTimer = false;
            currentFallDelayTimer = fallDelayTime;

            StartFalling();
        }
    }

    private bool isOnGround()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        
        bool results = Physics.Raycast(ray, out currentGroundHit, groundSearchLength);

        return results;
    }
    
    private bool HasHitHead()
    {
        return Physics.BoxCast(transform.position, headHitBoxSize, transform.up, out currentHeadHit, transform.rotation, headBumpCheckLength);
    }
    // private bool HasHitEnemy()
    // {
    //     //Debug.DrawLine(transform.position, transform.position - new Vector3(0f, hitEnemyOnHeadDistance, 0f), Color.red, hitEnemyOnHeadDistance);
    //     //Debug.DrawRay(transform.position, -transform.up, Color.red, hitEnemyOnHeadDistance * 10);
    //     
    //     //return Physics.Raycast(transform.position, -transform.up, out currentHitEnemyHit, hitEnemyOnHeadDistance);
    //     return Physics.BoxCast(transform.position, headHitBoxSize, -transform.up, out currentHitEnemyHit, transform.rotation, headBumpCheckLength);
    // }

    
    
    // private bool CheckDistanceBelow()
    // {
    //     return Physics.Raycast(transform.position + new Vector3(0f, 0.1f, 0f), -transform.up, out distanceToObjectBelowPlayer); 
    // }

    private bool HasHitWall()
    {
        //Vector3 currentFacingXZ = transform.InverseTransformDirection(transform.forward);
        //currentFacingXZ.y = 0.0f;

        //Debug.DrawLine(transform.position, transform.position + currentFacingXZ, Color.blue, 0.01f);
        //return Physics.SphereCast(transform.position, wallHitRadius, transform.position + currentFacingXZ * 2f, out currentWallHit,checkForWallCollisionForward);
        
        return Physics.SphereCast(new Vector3(transform.position.x, transform.position.y - 0.65f, transform.position.z), wallHitRadius, localForward, out currentWallHit, checkForWallCollisionForward);
    }

    // void OnGUI()
    // {
    //     // Add here any debug text that might be helpful for you
    //     GUI.Label(new Rect(10, 10, 100, 20), "Angle " + angleDifferenceForward.ToString(), myGUIStyle);
    // }

    private void OnDrawGizmos()
    {
        // Debug Draw last ground collision, helps visualize errors when landing from a jump
        if (currentGroundHit.collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(currentGroundHit.point, 0.25f);
        }
        

        if (currentHeadHit.collider != null)
        {
            if (currentHeadHit.collider.gameObject.CompareTag($"Platform"))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(currentHeadHit.point, new Vector3(0.6f, 0.2f, 0.6f));
            }
        }
        
        if (currentWallHit.collider != null)
        {
            //Debug.Log($"head hit: {currentWallHit.collider.name}");

            if (currentWallHit.collider.gameObject.CompareTag($"Wall") || currentWallHit.collider.gameObject.CompareTag($"Treasure"))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(new Vector3(currentWallHit.point.x, currentWallHit.point.y + 0.6f, currentWallHit.point.z), 0.11f);
            }
        }
        

    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // Debug-draw all contact points and normals, helps visualize collisions when the physics of the RigidBody are enabled (when is NOT Kinematic)
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     // get collectables
    //     if (other.CompareTag("Treasure"))
    //     {
    //         _currentCollectedTreasures++;
    //         
    //         Destroy(other.gameObject);
    //     }
    // }

    private void LoadWinningScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // disable player health ui
        playerHealthUICOntainer.SetActive(false);
        
        // LOAD YOU WIN UI
        gameOverWinUI.SetActive(true);
        
        // DISABLE PLAYER
        gameObject.SetActive(false);
        
    }


    public void KnockPlayerBack()
    {
        // Vector3 movement = Vector3.forward * knockBackForce * Time.deltaTime;
        // movement += Vector3.forward * knockBackForce * Time.deltaTime;
        // movement.y = 0.0f;
        //
        // Vector3 targetPosition = rigidBody.position + movement;
        //
        // rigidBody.MovePosition(targetPosition);
        

    }

    
    
    
}
