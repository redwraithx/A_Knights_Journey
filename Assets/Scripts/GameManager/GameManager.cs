

using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [SerializeField] internal GameObject playerReference = null;
    [SerializeField] internal GameObject playersHealingSpellReference = null;

    public TMP_Text killCounterText;
    private int killCounter = 0;
    public TMP_Text powerUpCounterText;
    private int powerUpCounter = 0;

    private bool isBossDead = false;
    
    private void Awake()
    {
        if(instance)
           DestroyImmediate(this);

        instance = this;
        
    }

    private void Start()
    {
        if (!killCounterText)
            throw new Exception("Error, game manager missing kill counter text reference");

        killCounterText.text = "Enemy Kills: " + killCounter.ToString();
    }


    private void Update()
    {
        if (isBossDead)
        {
            //SceneManager.LoadScene()
        }
    }

    // track kills
    public void EnemyKilled()
    {
        killCounter++;

        killCounterText.text = "Enemy Kills: " + killCounter.ToString();
    }
    
    public void PowerUpsCollected()
    {
        powerUpCounter++;

        powerUpCounterText.text = "Power Ups Collected: " + powerUpCounter.ToString();
    }
}
