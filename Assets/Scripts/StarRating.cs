using UnityEngine;
using UnityEngine.UI;

public class StarRating : MonoBehaviour
{
    public Slider starSlider;  // Assign the slider
    public int maxScore = 100; // Maximum score is 100, adjust as needed

    public void UpdateStarRating(int currentScore)
    {
        // Calculate the slider value between 0 and 5
        float starValue = (float)currentScore / maxScore * 5;
        starSlider.value = starValue;
    }
}
