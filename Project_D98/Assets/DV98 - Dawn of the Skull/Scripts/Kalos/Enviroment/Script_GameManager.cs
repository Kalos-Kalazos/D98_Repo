using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Script_GameManager : MonoBehaviour
{
    [Header ("=== Game Manager Settings ===")]
    public Slider healthSlider;           
    public Slider boostSlider;
    public Slider heatSlider;
    public Slider bossHealthSlider;
    private Script_Spaceship player;
    private Script_Boss bossControl;
    public Script_Spawn_enemy spawner;
    public int deadMinionCount;

    public static Script_GameManager MainManager;

    public static int actualScene;

    public void Restart()
    {
        if (actualScene == 1) SceneManager.LoadScene("Scene_Tutorial");

        if (actualScene == 2) SceneManager.LoadScene("Scene_Level1");

        if (actualScene == 3) SceneManager.LoadScene("Scene_LevelBoss");
    }

    public void Win()
    {
        if (actualScene == 3) SceneManager.LoadScene("Scene_Tutorial");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartSpawn()
    {
        spawner.startSpawn = true;
    }

    public void NextLvl()
    {
        if (actualScene == 1) SceneManager.LoadScene("Scene_Level1");

        if (actualScene == 2) SceneManager.LoadScene("Scene_LevelBoss");

        if (actualScene == 3) SceneManager.LoadScene("Scene_Victory");
    }

    public void Achieved()
    {
        if (actualScene == 1 && deadMinionCount > 0) LvlCompleted();

        if (actualScene == 2 && deadMinionCount >= 20) LvlCompleted();
    }

    public void LvlCompleted()
    {
        ActualSceneID();
        SceneManager.LoadScene("Scene_NextLvl");
    }

    void ActualSceneID()
    {
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Tutorial")))
        {
            actualScene = 1;
        }

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Level1")))
        {
            actualScene = 2;
        }

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            actualScene = 3;
        }
    }

    void Start()
    {
        deadMinionCount = 0;

        ActualSceneID();
        player = FindObjectOfType<Script_Spaceship>();
        spawner = FindObjectOfType<Script_Spawn_enemy>();

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            bossControl = FindObjectOfType<Script_Boss>();
        }

        if (player != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.currentHealth;

            boostSlider.maxValue = player.maxBoost;
            boostSlider.value = player.currentBoost;

            heatSlider.maxValue = player.maxHeat;
            heatSlider.value = player.currentHeat;

            player.OnHealthChanged += UpdateHealthBar;
            player.OnBoostChanged += UpdateBoostBar;
            player.OnHeatChanged += UpdateHeatBar;
        }

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            bossHealthSlider.maxValue = bossControl.maxSkullHealth;
            bossHealthSlider.value = bossControl.skullHealth;
            bossControl.OnBossHealthChanged += UpdateBossHealth;
        }
    }

    private void Awake()
    {
        MainManager = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.currentHealth <= 0)
            {
                SceneManager.LoadScene("Scene_Lose");
            }
        }
    }

    #region Cambios de Slider de Vida, Boost y Heat

    //Health bar update
    public void UpdateHealthBar(int currentHealth)
    {
        StartCoroutine(HealthChange(currentHealth));
    }
    IEnumerator HealthChange(int targetHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.5f; 
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetHealth, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = targetHealth;
    }

    //Boost bar update
    public void UpdateBoostBar(float currentBoost)
    {
        StartCoroutine(BoostChange(currentBoost));
    }
    IEnumerator BoostChange(float targetBoost)
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        float startValue = boostSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            boostSlider.value = Mathf.Lerp(startValue, targetBoost, elapsedTime / duration);
            yield return null;
        }

        boostSlider.value = targetBoost;
    }

    //Heat bar update
    public void UpdateHeatBar(float currentHeat)
    {
        StartCoroutine(HeatChange(currentHeat));
    }
    IEnumerator HeatChange(float targetHeat)
    {
        float elapsedTime = 0f;
        float duration = 0.1f;
        float startValue = heatSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            heatSlider.value = Mathf.Lerp(startValue, targetHeat, elapsedTime / duration);
            yield return null;
        }

        heatSlider.value = targetHeat;
    }

    //Boss Heath bar update
    public void UpdateBossHealth(float currentHeat)
    {
        StartCoroutine(BossHealthChange(currentHeat));
    }
    IEnumerator BossHealthChange(float targetBossHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.1f;
        float startValue = bossHealthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bossHealthSlider.value = Mathf.Lerp(startValue, targetBossHealth, elapsedTime / duration);
            yield return null;
        }

        heatSlider.value = targetBossHealth;
    }

    #endregion
}
