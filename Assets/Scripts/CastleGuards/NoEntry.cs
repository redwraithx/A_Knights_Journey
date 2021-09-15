

using System;
using UnityEngine;
using TMPro;


public class NoEntry : MonoBehaviour
{
    [Header("Current Dialog & Timers state")]
    public bool isTimerRunning = false;
    public bool canSpeakToPlayer = true;
    public bool isSpeakingToPlayer = false;
    public bool canRestartDialog = false;
    
    [Header("Dialog displayed Time/Timer")]
    public float waitTime = 8f;
    public float currentTimer = 0f;

    [Header("Dialog Reset Timer")]
    public float coolDownTime = 4f;
    public float currentCoolDownTimer = 0f;
    public bool isDialogCoolDownTimerRunning = false;

    [Header("NPC UI References")]
    public GameObject dialogUIContainer = null;
    public TMP_Text dialogTextUI = null;

    [Header("Player Reference & Rotation Status")] 
    public GameObject targetObject = null;
    public bool isRotatingTowardsPosition = false;

    [Header("Container that holds all the different dialogs this NPC can say randomly")]
    public string[] dialog;
    
    // remember origin direction
    [Header("This Characters Components")] public SphereCollider sphereCollider = null;
    public Transform localForward = null;
    public Vector3 localForwardRotationPoint = Vector3.zero;
    


    private void Awake()
    {
        localForwardRotationPoint = transform.position + localForward.position;
        
        
    }

    private void Start()
    {
        if (!sphereCollider)
            throw new Exception("Error, sphereCollider reference is null");
            
        if (!localForward)
            throw new Exception("Error, localForward object is null");
        
        if (!dialogUIContainer)
            throw new Exception("Error, dialog UI Container is missing!");

        if (!dialogTextUI)
            throw new Exception("Error, dialog UI Text Mesh Pro is missing");

        if (dialog.Length <= 0)
            throw new Exception("Error, there are 0 dialog's for the castle guard");
        
        
        ResetBothTimers();

        canSpeakToPlayer = false;
        isSpeakingToPlayer = false;
        
        canRestartDialog = false;
    }


    private void Update()
    {
        // Time for how long to display the timer
        if (isTimerRunning)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0f)
            {
                isSpeakingToPlayer = false;
                canSpeakToPlayer = false;
                
                dialogUIContainer.SetActive(false);
                
                ResetTimerValues();

                isDialogCoolDownTimerRunning = true;

                isRotatingTowardsPosition = true;

            }
        }

        // Timer for how long to wait before allowing NPC to speak again while player is within range
        if (isDialogCoolDownTimerRunning)
        {
            currentCoolDownTimer -= Time.deltaTime;

            if (currentCoolDownTimer <= 0f)
            {
                Debug.Log("cool down timer running");
                
                ResetBothTimers();
                
                isSpeakingToPlayer = false;
                canSpeakToPlayer = false;

                canRestartDialog = targetObject;

                // if (targetObject)
                //     isRotatingTowardsPosition = true;
                if (targetObject)
                    IsLookingAtTargetObject(targetObject.transform.position);
                else
                    IsLookingAtTargetObject(localForward.position);

            }
        }


        if (canSpeakToPlayer && isSpeakingToPlayer)
        {
            isSpeakingToPlayer = false;

            RotateTowardsPoint(targetObject.transform.position);
         
            var index = GetRandomDialog();

            dialogTextUI.text = dialog[index];
            
            dialogUIContainer.SetActive(true);

            //isDialogCoolDownTimerRunning = true;
            isTimerRunning = true;
            
        }


        if (isRotatingTowardsPosition)
        {
            Debug.Log("castle guard is rotating towards object: " + (targetObject ? targetObject.name : "localPosition"));
            
            if (targetObject)
            {
                RotateTowardsPoint(targetObject.transform.position);
                
                IsLookingAtTargetObject(targetObject.transform.position);
            }
            else
            {
                RotateTowardsPoint(localForward.position);
                
                IsLookingAtTargetObject(localForward.position);
            }
        }
    }

    private void ResetBothTimers()
    {
        currentCoolDownTimer = coolDownTime;
        isDialogCoolDownTimerRunning = false;
        
        // reset timer for guard 
        ResetTimerValues();
    }

    private int GetRandomDialog() => UnityEngine.Random.Range(0, dialog.Length);


    private void ResetTimerValues()
    {
        currentTimer = waitTime;
        isTimerRunning = false;
    }

    private void RotateTowardsPoint(Vector3 lookAtPoint)
    {
        var lookingDirection = lookAtPoint - transform.position;
        lookingDirection.y = 0f;

        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    }
    
    private bool IsLookingAtTargetObject(Vector3 targetPosition)
    {
        bool _isTargetInFrontOfNPC = false;
        
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 targetDirection = targetPosition - transform.position;
    
        float angle = Vector3.Angle(targetDirection, localForward.position);

        float isPlayerWithinNpcToSpheresEdge = (sphereCollider.radius * 0.5f);
        
        if (Mathf.Abs(distance) > isPlayerWithinNpcToSpheresEdge)
        {
            if (angle < 10.0f)
                _isTargetInFrontOfNPC = true;
            // else 
            //     isTargetInFrontOfNPC = false;
        }
        else
        {
            if (angle < 20.0f) 
                _isTargetInFrontOfNPC = true;
            // else
            //     isTargetInFrontOfNPC = false;
        }

        if (_isTargetInFrontOfNPC)
            isRotatingTowardsPosition = false;

        return _isTargetInFrontOfNPC;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        if (!targetObject)
            targetObject = other.gameObject;
     
        if (canSpeakToPlayer || isDialogCoolDownTimerRunning || isTimerRunning)
            return;

        canSpeakToPlayer = true;
        isSpeakingToPlayer = true;
    }


    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        
        if (canRestartDialog)
        {
            canRestartDialog = false;
            
            canSpeakToPlayer = true;
            isSpeakingToPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        if (targetObject)
            targetObject = null;
        
      

        isRotatingTowardsPosition = true;
    }
}


