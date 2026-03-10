using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleScoreTrack : MonoBehaviour
{
    public static int currentActive;
    public static int score;

    public TextMeshProUGUI currentActiveText;
    public TextMeshProUGUI scoreText;

    private void LateUpdate()
    {
        currentActiveText.text = $"Active: {currentActive}";
        scoreText.text = $"Score: {score}";
    }
}
