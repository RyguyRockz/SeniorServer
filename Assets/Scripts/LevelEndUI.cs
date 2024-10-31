using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelEndUI : MonoBehaviour
{
    public TextMeshProUGUI penaltyLogText;

    public void ShowPenaltyLog()
    {
        penaltyLogText.text = ScoreManager.Instance.GetPenaltySummary();
    }
}

