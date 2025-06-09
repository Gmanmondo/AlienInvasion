using TMPro;
using UnityEngine;

public class AAPlayerTurnUI : MonoBehaviour
{
    public TMP_Text aaPlayerNameText;

    void Start()
    {
        var player = AAPlayerManager.Instance.GetCurrentPlayer();

        if (player != null)
        {
            aaPlayerNameText.text = $"{player.playerName}'s Turn!";
        }
        else
        {
            aaPlayerNameText.text = "Unknown Player";
        }
    }
}