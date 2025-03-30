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
    public GameObject timerScreen;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pauseTimerText;
    public Button[] levelButtons;
    private static int unlockedLevel = 0;   
    public GameObject pauseMenuUI;
    public GameObject confirmPanel;
    private bool isPaused = false;

    [Header("Health System")]
    public Image[] starImages;
    public Sprite fullStar;
    public Sprite emptyStar;

    public int totalStars;
    public static int sharedStars = 3;

    public float timeLimit = 180f;
    private float timer;

    private int score;
    private int remainingBuildings = 0;
    private bool isGameActive = true;


    void Start()
    {
        scoreText.text = score.ToString();
        timer = timeLimit;
        totalStars = sharedStars;
        UpdateStarUI();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
        {
            gameOverScreen.SetActive(false);
            gameWinScreen.SetActive(false);
            levelScreen.SetActive(false);
            pauseMenuUI.SetActive(false);
            confirmPanel.SetActive(false);
            timerScreen.SetActive(true);
            titleScreen.SetActive(true);

            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = (i <= unlockedLevel);
            }
        }
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }

            string formattedTime = FormatTime(timer);

            if (timerText != null)
                timerText.text = formattedTime;

            if (pauseTimerText != null)
                pauseTimerText.text = formattedTime;

            if (isGameActive && !isPaused)
            {
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    GameOver();
                }
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
        timerScreen.SetActive(false);
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
        Time.timeScale = 1f;
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
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
            timerScreen.SetActive(false);
            GainStarAfterSuccess();

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level"))
            {
                string numberPart = sceneName.Replace("Level", "");
                if (int.TryParse(numberPart, out int currentLevel))
                {
                    int nextLevel = currentLevel + 1;

                    unlockedLevel = nextLevel;
                }
            }
        }
    }

    void UpdateStarUI()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = (i < totalStars) ? fullStar : emptyStar;
        }
    }

    public void LoseStarAndRestartLevel()
    {
        totalStars = Mathf.Max(0, totalStars - 1);
        sharedStars = totalStars;
        UpdateStarUI();

        if (totalStars <= 0)
        {
            GameOver();
        }
    }

    public void GainStarAfterSuccess()
    {
        totalStars = Mathf.Min(starImages.Length, totalStars + 1);
        sharedStars = totalStars;
        UpdateStarUI();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        timerScreen.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        confirmPanel.SetActive(false);
        timerScreen.SetActive(true);
    }

    public void ConfirmBackToMenu()
    {
        pauseMenuUI.SetActive(false);
        confirmPanel.SetActive(true);
    }

    public void CancelBackToMenu()
    {
        pauseMenuUI.SetActive(true);
        confirmPanel.SetActive(false);
    }
}
