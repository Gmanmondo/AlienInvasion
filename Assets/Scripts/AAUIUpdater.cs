using TMPro;
using UnityEngine;

public class AAUIUpdater : MonoBehaviour
{
    public static AAUIUpdater Instance;
    public TMP_Text aaCowLossText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AAUpdateCowLossDisplay(int cowLoss)
    {
        aaCowLossText.text = $"Cows Lost: {cowLoss}";
    }
}