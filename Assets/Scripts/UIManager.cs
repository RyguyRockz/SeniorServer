using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI guestLeaveText; // Text UI for guest leaving
    public TextMeshProUGUI guestEatText; // Text UI for guest eating
    public TextMeshProUGUI guestIncorrectFoodText; // Text UI for incorrect food

    private void Start()
    {
        // Make sure the text elements are not active at the start
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
        textElement.text = message; // Set the message
        textElement.gameObject.SetActive(true); // Activate the text element

        yield return new WaitForSeconds(duration); // Wait for the specified duration

        textElement.gameObject.SetActive(false); // Deactivate the text element
    }
}