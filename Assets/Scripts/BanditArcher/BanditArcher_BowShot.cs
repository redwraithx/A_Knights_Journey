using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditArcher_BowShot : MonoBehaviour
{
    public GameObject arrowPrefab = null;
    public Transform arrowSpawnPoint = null;

    public BanditArcher_Vision myVision = null;
    public EnemyAttributes enemyAttributes = null;


    public float distancePastTargetToHit = 10f;
    
    private void Start()
    {
        if (!myVision)
            myVision = GetComponent<BanditArcher_Vision>();

        if (!enemyAttributes)
            enemyAttributes = GetComponent<EnemyAttributes>();

        if (!arrowSpawnPoint)
            throw new Exception("Error, arrowSpawnPoint is missing");
        
        if (!arrowPrefab)
            throw new Exception("Error, BanditArcher_BowShot component is missing the arrow prefab reference.");
        
        
    }







    public void ShootArrow()
    {
        if (myVision.rayToPlayer.collider != null)
        {
            if (myVision.rayToPlayer.collider.CompareTag("Player"))
            {
                
        
                GameObject arrowGO = Instantiate(arrowPrefab, arrowSpawnPoint);
                arrowGO.SetActive(false);
                var archer = arrowGO.GetComponent<BanditArcher_FireArrow>();
                archer.arrowDamage = enemyAttributes.GetWeaponDamage();
                archer.shotDelayTime = enemyAttributes.GetAttackSpeed();

                var direction = (myVision.player.position - transform.position);
                direction.Normalize();
                var distance = Vector3.Distance(transform.position, myVision.rayToPlayer.transform.position);

                Debug.Log("arrow distance to player = "+ distance);

                distance += 12f;
                
                var newLoc = transform.position + (direction * distance);

                newLoc.y = 0.0f;
                
                archer.shotTargetLocation = newLoc;
                
                arrowGO.transform.parent = null;
                
                //Vector3 shotLocation = myVision.targetObject.transform.position + Vector3.up + (Vector3.forward * 12);
                
                // Vector3 dir = myVision.rayToPlayer.point - transform.position;
                //
                // var newShotLoc = myVision.rayToPlayer.transform.position - transform.position;
                //
                // var addedShotRange = dir * 3f;
                //
                // Vector3 shotLocation = dir;    // Vector3 dir = myVision.rayToPlayer.point - transform.position;
                //
                // var newShotLoc = myVision.rayToPlayer.transform.position - transform.position;
                //
                // var addedShotRange = dir * 3f;
                //
                // Vector3 shotLocation = dir;
                
                
                
                // var newShotLoc = myVision.rayToPlayer.point + (dir * distancePastTargetToHit);
                //
                //
                // newShotLoc.x += -0.35f;
                // newShotLoc.y = 0f;
                //
                // var forward = new Vector3(0, 0, 0);
                //
                // Vector3 shotLocation = newShotLoc + Vector3.up + forward;
                // shotLocation.y = 0f;

                
                
                //arrowGO.transform.LookAt(myVision.rayToPlayer.collider.transform, transform.up);

                
                arrowGO.SetActive(true);
        
        
            }
        }
        

    }
}
