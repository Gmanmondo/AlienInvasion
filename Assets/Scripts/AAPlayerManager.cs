using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AAPlayerManager : MonoBehaviour
{
    public static AAPlayerManager Instance;

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int score;
    }

    public List<PlayerData> players = new List<PlayerData>();
    private int currentPlayerIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayers(List<string> names)
    {
        players.Clear();
        foreach (var name in names)
        {
            players.Add(new PlayerData { playerName = name, score = 0 });
        }
    }

    public PlayerData GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public void EndTurn(int playerScore)
    {
        players[currentPlayerIndex].score = playerScore;
        currentPlayerIndex++;
        
        Debug.Log("TURN ENDED → Next up: " + (currentPlayerIndex < players.Count
            ? players[currentPlayerIndex].playerName
            : "All players done"));

        if (currentPlayerIndex >= players.Count)
        {
            // All players finished
            SceneManager.LoadScene("AlienScoreDisplay");
        }
        else
        {
            // Load next round for the next player
            SceneManager.LoadScene("AlienGameplay");
        }
    }
}