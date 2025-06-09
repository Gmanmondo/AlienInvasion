using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AAFinalScoreDisplay : MonoBehaviour
{
    public Transform aaLeaderboardContainer;
    public GameObject aaPlayerScoreEntryPrefab;
    public float aaRevealDelay = 1f;

    void Start()
    {
        StartCoroutine(AARevealScores());
    }

    IEnumerator AARevealScores()
    {
        // 1. Get players and sort them from most to least cows lost
        List<AAPlayerManager.PlayerData> players = new List<AAPlayerManager.PlayerData>(AAPlayerManager.Instance.players);

        players.Sort((a, b) => b.score.CompareTo(a.score)); // Highest score (worst) first

        for (int i = 0; i < players.Count; i++)
        {
            AAPlayerManager.PlayerData player = players[i];

            // 2. Instantiate UI entry
            GameObject entry = Instantiate(aaPlayerScoreEntryPrefab, aaLeaderboardContainer);
            entry.transform.SetSiblingIndex(0);
            TMP_Text entryText = entry.GetComponentInChildren<TMP_Text>();
            if (entryText != null)
                entryText.text = $"{players.Count - i}. {player.playerName} - Cows Lost: {player.score}";

            // Optional: animate fade-in or scale pop here
            yield return new WaitForSeconds(aaRevealDelay);
        }
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("AlienPlayerSelect");
    }
}