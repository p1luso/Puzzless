using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int moves;
    public int points;
    public int peeks;
    public int difficulty;
    public int TotalScore;

    private int initialPoints;
    private int pointsPerMove;
    private int pointsPerPeek;
    private int timePenalty;

    private const string TotalScoreKey = "TotalScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTotalScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeScore(int difficulty)
    {
        this.difficulty = difficulty;
        moves = 0;
        peeks = 0;

        switch (difficulty)
        {
            case 1: // Easy
                initialPoints = 4000;
                pointsPerMove = 30;
                pointsPerPeek = 50;
                timePenalty = 10;
                break;
            case 2: // Medium
                initialPoints = 7000;
                pointsPerMove = 60;
                pointsPerPeek = 100;
                timePenalty = 30;
                break;
            case 3: // Hard
                initialPoints = 10000;
                pointsPerMove = 75;
                pointsPerPeek = 200;
                timePenalty = 45;
                break;
        }

        points = initialPoints;
    }

    public void RegisterMove()
    {
        moves++;
        points -= pointsPerMove;
    }

    public void RegisterPeek()
    {
        peeks++;
        points -= pointsPerPeek;
    }

    public void RegisterTimePenalty()
    {
        points -= timePenalty;
    }

    public void UpdateTotalScore()
    {
        TotalScore += points; // Add points to TotalScore at the end of the game
        SaveTotalScore();
    }

    private void LoadTotalScore()
    {
        if (PlayerPrefs.HasKey(TotalScoreKey))
        {
            TotalScore = PlayerPrefs.GetInt(TotalScoreKey);
        }
        else
        {
            TotalScore = 0;
        }
    }

    private void SaveTotalScore()
    {
        PlayerPrefs.SetInt(TotalScoreKey, TotalScore);
        PlayerPrefs.Save();
    }

    public int GetTotalScore()
    {
        return TotalScore;
    }
}
