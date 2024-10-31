using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI guestLeaveText;
    public TextMeshProUGUI guestEatText;
    public TextMeshProUGUI guestIncorrectFoodText;


    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        guestLeaveText.gameObject.SetActive(false);
        guestEatText.gameObject.SetActive(false);
        guestIncorrectFoodText.gameObject.SetActive(false);
       
    }

    public void ShowText(TextMeshProUGUI textElement, string message, float duration)
    {
        StartCoroutine(DisplayText(textElement, message, duration));
    }

    private IEnumerator DisplayText(TextMeshProUGUI textElement, string message, float duration)
    {
        textElement.text = message;
        textElement.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        textElement.gameObject.SetActive(false);
    }

    public TextMeshProUGUI penaltyLogText; // Text element to display penalty summary

    public void ShowPenaltyLog()
    {
        penaltyLogText.text = ScoreManager.Instance.GetPenaltySummary();
    }
}
    