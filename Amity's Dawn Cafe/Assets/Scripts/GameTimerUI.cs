using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Update()
    {
        float t = GameTimer.Instance.timeRemaining;
        timerText.text = $"Time  Left:  {t: 0}";
    }
}
