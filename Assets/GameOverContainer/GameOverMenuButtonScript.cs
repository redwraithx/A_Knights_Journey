using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuButtonScript : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
