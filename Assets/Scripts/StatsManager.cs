using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI daysEmployedText;

    private int daysEmployed = 0;
    private float dayTimer = 0f;
    [SerializeField] private float secondsPerDay = 3f;

    void Update()
    {
        // Increment days employed based on the timer
        dayTimer += Time.deltaTime;
        if (dayTimer >= secondsPerDay)
        {
            daysEmployed++;
            daysEmployedText.text = daysEmployed.ToString();
            dayTimer = 0f;
        }
    }
}
