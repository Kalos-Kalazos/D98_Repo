using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    public GameObject[] endLevelUI;
    [SerializeField]
    Script_GameManager gameManager;

    void Start()
    {

        for (int i = 0; i < endLevelUI.Length; i++)
        {
            endLevelUI[i].SetActive(false);            
        }
    }

    public void CompleteLevel(int i)
    {
        endLevelUI[i].SetActive(true);
        StartCoroutine(NextScene());
        Time.timeScale = 0f;
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1f;

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Tutorial")))
        {
            endLevelUI[0].SetActive(false);
        }
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Level1")))
        {
            endLevelUI[1].SetActive(false);
        }
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            endLevelUI[2].SetActive(false);
        }
    }
    IEnumerator NextScene()
    {
        yield return new WaitForSecondsRealtime(5f);

        LoadNextScene();
    }
}
