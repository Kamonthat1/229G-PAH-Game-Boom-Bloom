using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;

    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public GameObject levelScreen;
    public TextMeshProUGUI timerText;

    [Header("Health System")]
    public Image[] hearts;         
    public Sprite fullHeart;        
    public Sprite emptyHeart; 
    private int health = 3;

    public float timeLimit = 180f;
    private float timer;

    private int score;
    private int remainingBuildings = 0;
    private bool isGameActive = true;


    void Start()
    {
        scoreText.text = score.ToString();
        timer = timeLimit;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
        {
            gameOverScreen.SetActive(false);
            gameWinScreen.SetActive(false);
            levelScreen.SetActive(false);
            titleScreen.SetActive(true);
        }
        else
        {
            if (titleScreen != null) titleScreen.SetActive(false);
            if (levelScreen != null) levelScreen.SetActive(false);
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            if (gameWinScreen != null) gameWinScreen.SetActive(false);
        }
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu") && isGameActive)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timer / 60f);
                int seconds = Mathf.FloorToInt(timer % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            if (timer <= 0f)
            {
                GameOver();
            }
        }
    }


    public void StartGame()
    {
        titleScreen.SetActive(false);
        levelScreen.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }

    public void GameOver()
    {
        if (!isGameActive) return;

        isGameActive = false;
        StartCoroutine(ShowGameOverDelayed());
    }

    IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        gameOverScreen.SetActive(true);
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void Restart()
    {
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void BackMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level" + levelNumber);
    }

    public void RegisterBuilding()
    {
        remainingBuildings++;
    }

    public void NotifyBuildingDestroyed()
    {
        remainingBuildings--;

        if (remainingBuildings <= 0)
        {
            gameWinScreen.SetActive(true);
        }
    }

    public void LoseHealth()
    {
        health--;
        UpdateHearts();
        if (health <= 0)
        {
            GameOver();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < health) ? fullHeart : emptyHeart;
        }
    }
}
