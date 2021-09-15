

using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayersHealthUI : MonoBehaviour
{
    public Slider healthbar = null;

    private void Start()
    {
        if (!healthbar)
            throw new Exception("Error player missing slider for health bar");
        
    }

    public void SetMaxHealthValue(float value)
    {
        healthbar.maxValue = value;
    }

    public void UpdateCurrentHealthBarValue(float value)
    {
        healthbar.value = value;
    }
    
}
