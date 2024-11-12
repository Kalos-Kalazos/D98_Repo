using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject powersMenu;
    private void Start()
    {
        Script_AudioManager.Instance.PlayMusic(0);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void OptionsGame()
    {
        SceneManager.LoadScene("OptionsScene");
    }
    public void ActiveOptions()
    {
        powersMenu.SetActive(true);
    }
    public void DeactiveOptions()
    {
        powersMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
