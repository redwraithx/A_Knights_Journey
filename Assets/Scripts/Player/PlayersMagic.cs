using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayersMagic : MonoBehaviour
{
    public int healingAmountMin;
    public int healingAmountMax;
    
    public float castDelayTimer;
    public bool isCastDelayTimerRunning = false;
    public bool isResettingTimer = false;

    public PlayerAttributes playerAttributes;
    public PlayersHealth playersHealth;
    public Animator anim;
    private static readonly int CastHeal = Animator.StringToHash("CastHeal");

    public GameObject healingSpellEffectPrefab = null;
    
    
    [Space, Header("Positions for Magic spell transition")]
    // cast magic points
    // hand
    public GameObject startPosition = null;
    // feet
    public GameObject endPosition = null;
    // above head
    public GameObject maxHeightPosition = null;
    public float magicLifeTimeLength = 1.5f;
    

    private void Start()
    {
        if (!playerAttributes)
            playerAttributes = GetComponent<PlayerAttributes>();

        if (!playersHealth)
            playersHealth = GetComponent<PlayersHealth>();

        if (!anim)
            anim = GetComponent<Animator>();

        if (!healingSpellEffectPrefab)
            healingSpellEffectPrefab = GameManager.Instance.playersHealingSpellReference;
        
        UpdateRecastTimer();
    }


    private void Update()
    {
        if (playersHealth.CurrentHealth <= 0f)
            return;
        
        
        if (!isCastDelayTimerRunning)
        {
            if (Input.GetMouseButtonDown(1) &&  !isResettingTimer)
            {
                isResettingTimer = false;
                
                anim.SetTrigger(CastHeal);
                
                // heal player
                playersHealth.HealPlayer(UnityEngine.Random.Range(healingAmountMin, healingAmountMax));
                
                // start recast timer
                isCastDelayTimerRunning = true;
            }
        }

        if (isCastDelayTimerRunning)
        {
            castDelayTimer -= Time.deltaTime;

            if (castDelayTimer <= 0f)
            {
                isCastDelayTimerRunning = false;
                
                UpdateRecastTimer();
                
                // show icon for heal is ready
                
            }
        }
    }


    public void UpdateRecastTimer()
    {
        castDelayTimer = playerAttributes.GetRecastTimeForHealingSpell();

        isResettingTimer = false;
    }

    public void SpawnHealEffect()
    {
        GameObject spell = Instantiate(healingSpellEffectPrefab, startPosition.transform.position, quaternion.identity);
        
        spell.transform.SetParent(startPosition.transform);
        
        Destroy(spell, magicLifeTimeLength);
    }
}
