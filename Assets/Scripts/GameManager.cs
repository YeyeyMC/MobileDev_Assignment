using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gameplay")] 
    [SerializeField] private int totalCollectibles = 5;
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private Transform ball;
    [SerializeField] private GyroStuff ballController;
    [SerializeField] private Transform ballSpawnPoint;

    [Header("UI Panels")] 
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject endPanel;

    [Header("UI Text")] 
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private TextMeshProUGUI magnetText;

    public bool IsPlaying { get; private set; }
    public Transform Ball => ball;

    private int score;
    private float timeLeft;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        timeLeft = gameTime;
        score = 0;

        Time.timeScale = 0f;
        IsPlaying = false;

        startPanel.SetActive(true);
        hudPanel.SetActive(false);
        endPanel.SetActive(false);

        UpdateUI();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(false);
        }

        UpdateUI();
    }

    public void StartGame()
    {
        score = 0;
        timeLeft = gameTime;
        IsPlaying = true;
        Time.timeScale = 1f;

        startPanel.SetActive(false);
        hudPanel.SetActive(true);
        endPanel.SetActive(false);

        if (ballController != null && ballSpawnPoint != null)
        {
            ballController.ResetBall(ballSpawnPoint.position);
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (!IsPlaying) return;

        score += amount;

        if (score >= totalCollectibles) EndGame(true);

        UpdateUI();
    }

    public void SetMagnetUI(bool active, float charge01)
    {
        if (magnetText == null) return;

        string state = active ? "ON" : "OFF";
        magnetText.text = $"Touch Magnet: {state} | Charge: {Mathf.RoundToInt(charge01 * 100f)}%";
    }

    private void EndGame(bool won)
    {
        IsPlaying = false;
        Time.timeScale = 0f;

        hudPanel.SetActive(false);
        endPanel.SetActive(true);

        if (endText != null) endText.text = won ? "You Win!" : "Time Up!";
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}/{totalCollectibles}";
        if (timerText != null) timerText.text = $"Time: {Mathf.CeilToInt(timeLeft)}";
    }
}