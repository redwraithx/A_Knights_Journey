using System.Runtime.CompilerServices;
using EnemyStates;
using UnityEngine;


namespace EnemyStates
{

    public enum CurrentState
    {
        Idle,
        ChasePlayer,
        ArrivedAtPlayer,
        
        Attack, 
        AttackCoolDown,
        
        SearchPlayersLastKnownPOS,
        
        Searching,
        Patrolling,
        Arrived,
        Fleeing,
        BackToOrigin,
        ResetToOriginPOS,
        ResetToOriginDelay,
        Dead
    }
}




public class AIHelpers
{
    static private float maxWanderDuration = 2.0f;
    static private float wanderCounter = 0.0f;
    static private System.Random r = new System.Random();

    public class MovementResult
    {
        public Vector3 newPosition = Vector3.zero;
        public Vector3 newOrientation = Vector3.zero;
    }

    public class InputParameters
    {
        public InputParameters(Transform current, Transform target, float updateDelta, float speed)
        {
            currentTransform = current;
            targetTransform = target;
            currentUpdateDuration = updateDelta;
            maxSpeed = speed;
        }
        public InputParameters(Transform current, Vector3 target, float updateDelta, float speed)
        {
            currentTransform = current;
            targetPosition = target;
            currentUpdateDuration = updateDelta;
            maxSpeed = speed;
        }

        public InputParameters(InputParameters o)
        {
            currentTransform = o.currentTransform;
            targetTransform = o.targetTransform;
            currentUpdateDuration = o.currentUpdateDuration;
            maxSpeed = o.maxSpeed;
        }

        public InputParameters()
        {
            currentUpdateDuration = 0.0f;
            maxSpeed = 1.0f;
        }

        public Transform currentTransform;
        public Transform targetTransform;
        public Vector3 targetPosition;
        public float currentUpdateDuration;
        public float maxSpeed;
    }

    public enum MovementBehaviors
    { 
        Idle,
        SeekKinematic,
        FleeKinematic,
        WanderKinematic,
        SearchKinematic,
        BackToOriginPosKinematic,
        ArcherKinematic,
        Dead
    }

    //private static bool IsThisEnemyAlive(InputParameters inputData) => ((inputData.currentTransform.gameObject.GetComponent<EnemyAttributes>().GetCurrentState() == CurrentState.Dead) ? true : false);
    private static bool IsThisEnemyAlive(InputParameters inputData) => inputData.currentTransform.gameObject.GetComponent<EnemyAttributes>().GetIsDeadStatus();


    internal static void SeekKinematic(InputParameters inputData, ref MovementResult result)
    {
        Debug.Log("seek kinematic call");
        // TODO: Implement logic to write the new desired position that moves closer to the target into result.newPosition

        Vector3 directionToMove = Vector3.zero;
        Vector3 newPos = Vector3.zero;;
        
        //float changeOverTime = 0f;


        var backupResults = result;
        
        
        
        if (inputData.targetTransform)
        {
            directionToMove = (inputData.targetTransform.position - inputData.currentTransform.position);
            directionToMove.Normalize();
            
            var changeOverTime = inputData.maxSpeed * inputData.currentUpdateDuration;

            //newPos = Vector3.Lerp(inputData.currentTransform.position, inputData.targetTransform.position, changeOverTime);
            
            //result.newPosition = newPos;

            result.newPosition = inputData.currentTransform.position + (directionToMove * inputData.maxSpeed * inputData.currentUpdateDuration);
        }
        else
        {
            directionToMove = (inputData.targetPosition - inputData.currentTransform.position);
            directionToMove.Normalize();
            
            var changeOverTime = inputData.maxSpeed * inputData.currentUpdateDuration;

            //newPos = Vector3.Lerp(inputData.currentTransform.position, inputData.targetPosition, changeOverTime);
            
            //result.newPosition = newPos;
            
            result.newPosition = inputData.currentTransform.position + (directionToMove * inputData.maxSpeed * inputData.currentUpdateDuration);
        }


        

        if (newPos == Vector3.zero)
            result = backupResults;
        else
            result.newPosition = newPos;
        
        
        result.newPosition.y = 0.92f;
    }

    private static void ApplyAvoidance(ref Vector3 direction, ref InputParameters inputData)
    {
        float moveForce = 5f;
        float minimumAvoidanceDistance = 5f;
        
        int layerMask = 1 << 8;

        RaycastHit avoidanceHit;

        if (Physics.Raycast(inputData.currentTransform.position, inputData.currentTransform.forward, out avoidanceHit, minimumAvoidanceDistance, layerMask)) ;
        {
            var hitNormal = avoidanceHit.normal;
            
            Debug.Log("hitzNormal: ");
            hitNormal.y = 0.0f; // MAY NEED TO UPDATE THIS WITH A HEIGHT ADJUSTMENT VALUE

            direction = inputData.currentTransform.forward + hitNormal * moveForce;
        }
        
    }

    internal static void FleeKinematic(InputParameters inputData, ref MovementResult result)
    {
        if (!IsThisEnemyAlive(inputData))
            return;
        
        Debug.Log("flee kine matic call");
        
        Vector3 directionToMove = (inputData.currentTransform.position - inputData.targetTransform.position);
        directionToMove.Normalize();
        
        //float changeOverTime = inputData.maxSpeed * inputData.currentUpdateDuration;

        //result.newPosition = inputData.currentTransform.position + (directionToMove * changeOverTime);
        
        result.newPosition = inputData.currentTransform.position + (directionToMove * (inputData.maxSpeed * inputData.currentUpdateDuration));
    }

    internal static void WanderKinematic(InputParameters inputData, ref MovementResult result)
    {
        if (!IsThisEnemyAlive(inputData))
            return;


        Debug.Log("wander kinematic call");
        
        SeekKinematic(inputData, ref result);
    }

    internal static void SearchKinematic(InputParameters inputData, ref MovementResult result)
    {
        Debug.Log("going to search position");
        
        SeekKinematic(inputData, ref result);
    }
    
    
    internal static void BackToOriginPosKinematic(InputParameters inputData, ref MovementResult result)
    {
        if (!IsThisEnemyAlive(inputData))
            return;
        
        
        SeekKinematic(inputData, ref result);
    }

    internal static void ArcherKinematic(InputParameters inputData, ref MovementResult result)
    {
        if (!IsThisEnemyAlive(inputData))
            return;
        

        
        Debug.Log("the archer is unable to move but needs to run a state anyway");
    }
    
    
}