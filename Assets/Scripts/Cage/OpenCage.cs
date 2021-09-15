using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenCage : MonoBehaviour
{
    public bool isDoorOpen = false;

    public void PlayerOpensGame()
    {
        SceneManager.LoadScene(2); // change to load to win level
    }
}
