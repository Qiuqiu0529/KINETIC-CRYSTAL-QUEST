using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo;
public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1f;
        
    }
    // Start is called before the first frame update
    public void QuitGame()
    {
        Application.Quit();
    }
    public void PauseMenu()
    {
        
         Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
         Time.timeScale = 1f;
    }
    public void Menu()
    {
        SceneManager.LoadScene("Main");

    }
}