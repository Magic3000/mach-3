using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dateText;
    internal const string PLAYER_PREFS_KEY = "scoreboard";

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    void Start()
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            //Debug.Log("Leaderboard not found, creating new one");
            //PlayerPrefs.SetString(PLAYER_PREFS_KEY, JsonConvert.SerializeObject(new Scoreboard() { scores = new List<Score>() }));
            var scorePref = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
            if (string.IsNullOrEmpty(scorePref))
            {
                scoreText.text = "Score\nEmpty";
                dateText.text = "Date\nEmpty";
                return;
            }
            Debug.Log(scorePref);
            var scoreboard = JsonConvert.DeserializeObject<Scoreboard>(scorePref);
            scoreboard.scores = scoreboard.scores.OrderByDescending(x => x.score).ToList();
            scoreText.text = $"Score\n{string.Join("\n", scoreboard.scores.Select(x => x.score))}";
            dateText.text = $"Date\n{string.Join("\n", scoreboard.scores.Select(x => x.date.ToString("dd/MM/yyyy HH:mm")))}";
        }
        else
            Debug.LogError("PLAYER_PREFS_KEY not found");
    }

    internal static void SaveScore(int _score)
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            var scoreboard = JsonConvert.DeserializeObject<Scoreboard>(PlayerPrefs.GetString(PLAYER_PREFS_KEY));
            scoreboard.scores.Add(new Score() { score = _score, date = DateTime.Now });
            PlayerPrefs.SetString(PLAYER_PREFS_KEY, JsonConvert.SerializeObject(scoreboard));
            PlayerPrefs.Save();
        }
        else
        {
            var scoreboard = new Scoreboard();
            Debug.Log(DateTime.Now);
            scoreboard.scores = new List<Score>
            {
                new Score() { score = _score, date = DateTime.Now }
            };
            PlayerPrefs.SetString(PLAYER_PREFS_KEY, JsonConvert.SerializeObject(scoreboard));
            PlayerPrefs.Save();
        }
        // PlayerPrefs.SetInt(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), score);
        // PlayerPrefs.Save();
    }

    public class Score
    {
        public int score = default;
        public DateTime date = default;
    }
    public class Scoreboard
    {
        public List<Score> scores = default;
    }
}
