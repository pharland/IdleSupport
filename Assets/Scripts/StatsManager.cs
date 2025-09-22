using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI daysEmployedText;
    [SerializeField] private TMPro.TextMeshProUGUI doshText;
    
    private int daysEmployed = 0;
    private float dayTimer = 0f; // ticks every second
    [SerializeField] private float secondsPerDay = 10f; // how many seconds in real time is a day in game

    private int totalDosh = 0;

    //public float maxTimeForPerfectCSAT = 5f;
    public int subtractCSATAmount = 10; //-10% CSAT per interval
    public float subtractCSATInterval = 3f; //seconds

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

    public void AddDosh(int amount)
    {
        totalDosh += amount;
        doshText.text = "$" + totalDosh.ToString();
    }
}
