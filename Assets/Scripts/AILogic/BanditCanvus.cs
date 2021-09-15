
using System;
using TMPro;
using UnityEngine;

public class BanditCanvus : MonoBehaviour
{
    public EnemyAttributes enemyAttributes = null;

    public TMP_Text stateText = null;
    public bool canShowDebugText = true;

    public GameObject TextContainer = null;
    
    public GameObject playerRef;
    
    private void Start()
    {
        if (!TextContainer)
            throw new Exception($"Error! Enemy {gameObject.name} is missing its Debug Text container reference");
        
        enemyAttributes = GetComponentInParent<EnemyAttributes>();

        //stateText = GetComponentsInChildren<TMP_Text>();
        
        
        UpdatePlayerReference();
    }

    private void Update()
    {
        if (!playerRef)
            UpdatePlayerReference();

        if (!playerRef)
            return;
        
        if (canShowDebugText)
        {
            TextContainer.SetActive(true);
            
            stateText.text = enemyAttributes.GetCurrentState().ToString();

            //if (enemyAttributes.GetCurrentRaycastHitToPlayer().collider != null && enemyAttributes.GetCurrentRaycastHitToPlayer().collider.CompareTag("Player"))
            //{
                

                var lookingDirection = transform.position - playerRef.transform.position;
                lookingDirection.y = 0f;

                var rotation = Quaternion.LookRotation(lookingDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
            //}
        }
        else
        {
            TextContainer.SetActive(false);
        }
        
    }

    private void UpdatePlayerReference()
    {
        playerRef = GameManager.Instance.playerReference;
    }
    
}
