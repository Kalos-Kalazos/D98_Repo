using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_MainMenu : MonoBehaviour
{
    private void Start()
    {
        Script_AudioManager.Instance.PlayMusic(0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OptionsGame()
    {
        SceneManager.LoadScene("OptionsGame");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
