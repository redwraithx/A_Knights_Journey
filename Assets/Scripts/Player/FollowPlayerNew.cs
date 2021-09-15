
using UnityEngine;

public class FollowPlayerNew : MonoBehaviour
{
    public Transform PlayerTransform = null;
    private Vector3 _cameraOffset = Vector3.zero;
    private Vector3 _currentOrientationFromPlayer;

    private Vector2 rightStickInput = Vector2.zero;
    
    public float cameraRotationSpeed = 90.0f;
    public float originalAngle = 10.0f;
    public float distanceFromPlayer = 10.0f;

    public Quaternion currentRotation = Quaternion.identity;

    public float eulerPitch = 0.0f;
    public float eulerYaw = -100.0f;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //_cameraOffset = transform.position - PlayerTransform.position;

        currentRotation.eulerAngles += new Vector3(eulerPitch, eulerYaw, 0.0f);
        _currentOrientationFromPlayer = currentRotation * Vector3.forward;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!PlayerTransform)
        {
            PlayerTransform = GameManager.Instance.playerReference.transform;
            
            Debug.Log("PlayerTranform ref in followcamera was null");

            return;
        }
        
        // Vector3 relativePos = PlayerTransform.position - transform.position;
        // transform.rotation = Quaternion.LookRotation(relativePos);

        float horizontal = Input.GetAxis("RightStickHorizontal");

        float vertical = Input.GetAxis("RightStickVertical");

        rightStickInput = new Vector2(horizontal, vertical);
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

    }

    //// LateUpdate is called after Update methods
    //void LateUpdate()
    //{
    //    Vector3 newPos = PlayerTransform.position + _cameraOffset;
    //    transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    //}

    private void LateUpdate()
    {
        eulerYaw += rightStickInput.x * cameraRotationSpeed * Time.deltaTime;
        eulerPitch += rightStickInput.y * cameraRotationSpeed * Time.deltaTime;

        currentRotation.eulerAngles = new Vector3(eulerPitch, eulerYaw, 0.0f);
        _currentOrientationFromPlayer = currentRotation * Vector3.forward;

        _cameraOffset = PlayerTransform.position + (_currentOrientationFromPlayer * distanceFromPlayer);
        transform.position = _cameraOffset;

        
        
        Vector3 relativePos = PlayerTransform.position - transform.position;
        
        transform.rotation = Quaternion.LookRotation(relativePos);
        
        
    }
}
