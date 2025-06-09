using UnityEngine;

public class AACowAbductionManager : MonoBehaviour
{
    public static AACowAbductionManager Instance;

    public int aaCowsLostThisTurn = 0;
    public float aaAbductionRatePerSecond = 1f;

    private float aaAbductionBuffer = 0f; // NEW: floating point buffer

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AAResetCowLoss()
    {
        aaCowsLostThisTurn = 0;
        aaAbductionBuffer = 0f;
        AAUIUpdater.Instance?.AAUpdateCowLossDisplay(aaCowsLostThisTurn);
    }

    public void AAReportAbduction(float deltaTime)
    {
        aaAbductionBuffer += aaAbductionRatePerSecond * deltaTime;

        int wholeCows = Mathf.FloorToInt(aaAbductionBuffer);
        if (wholeCows > 0)
        {
            aaCowsLostThisTurn += wholeCows;
            aaAbductionBuffer -= wholeCows;

            Debug.Log($"Abducted {wholeCows} cows. Total: {aaCowsLostThisTurn}");
            AAUIUpdater.Instance?.AAUpdateCowLossDisplay(aaCowsLostThisTurn);
        }
    }
}