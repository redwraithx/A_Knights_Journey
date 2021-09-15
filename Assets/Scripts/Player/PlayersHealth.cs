

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersHealth : MonoBehaviour
{
    public int maxHealth = 500;
    public int currentHealth = 0;

    public GameObject gameOverLoseUIContainer = null;
    public GameObject playerHealthBarUIContainer = null;

    private PlayerAttributes _playerAttributes = null;
    
    // // temp
    // private float duration = 1f;
    // private float currentTime = 1f;
    // private bool isTimerRunning = true;
    private bool hasDied = false;
    
    
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            int newValue = value - _playerAttributes.GetDamageProtection();
            
            currentHealth = newValue > 0 ? newValue : 0;
            healthBarReference.UpdateCurrentHealthBarValue(currentHealth);
        }
    }

    public PlayersHealthUI healthBarReference = null;
    
    
    private void Start()
    {
        if (!healthBarReference)
            throw new Exception("Error players health is missing health bar UI slider reference");

        _playerAttributes = GetComponent<PlayerAttributes>();
        
        currentHealth = maxHealth;
        healthBarReference.SetMaxHealthValue(maxHealth);
        healthBarReference.UpdateCurrentHealthBarValue(currentHealth);


        GameManager.Instance.playerReference = gameObject;
    }
    

    
    private void Update()
    {
        if (hasDied)
        {
            gameObject.SetActive(false);
        }
        
        if (currentHealth <= 0 && !hasDied)
        {
            hasDied = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            playerHealthBarUIContainer.SetActive(false);
            gameOverLoseUIContainer.SetActive(true);
        }
        
        
        // if (isTimerRunning)
        // {
        //     currentTime -= Time.deltaTime;
        //
        //     if (currentTime < 0f)
        //     {
        //         currentTime = duration;
        //         
        //         CurrentHealth -= 40;
        //     }
        // }
    }

    public void HealPlayer(int value)
    {
        CurrentHealth += value;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        
    }

    public void TakeDamage(int value) // falldamage
    {
        CurrentHealth -= value;
        
        // check for death!
        if(currentHealth <= 0)
            Debug.Log("Player has died, game over");
    }
    
    
    public void TakeDamageFromAttack(int value, Vector3 knockBackDirection, float knockBackForce) // enemy attack with pushback
    {
        Debug.Log("damage taken: " + value);
        
        CurrentHealth -= value;
        
        // check for death!
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died, game over");
            
            SceneManager.LoadScene(3);

            return;
        }
        
        // push back applied animation
        
        
    }


    public void KnockBackPlayer(Vector3 knockBackDirection, float knockBackForce)
    {
        
    }

    IEnumerator KnockBack(Vector3 direction, float delayTime, int interations)
    {
        int count = 0;

        while (count < interations)
        {
            //Vector3 forcedMove = Camera.main.transform.forward *  
        }

        yield return null;
    }

}
