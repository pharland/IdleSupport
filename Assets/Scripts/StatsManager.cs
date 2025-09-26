using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI daysEmployedText;
    [SerializeField] private TMPro.TextMeshProUGUI doshText;

    [SerializeField] private float secondsPerDay; // how many seconds in real time is a day in game
    private int daysEmployed = 0;
    private float dayTimer = 0f; // ticks every second
    private float totalDosh = 0;

    public float baseDoshPerEmail = 2; // base amount of dosh to earn per email sent
    public float subtractDoshAmount = 2; // deduct this amount of dosh per email sent with errors
    public float doshModifier = 1f; // multiplier for dosh earned (e.g high CSAT)
    public float doshNegativeModifier = 1f; // multiplier for dosh lost (e.g incorrect button clicked or low CSAT)
    public int subtractCSATAmount = 10; // deduct this amount of CSAT for every interval
    public float subtractCSATInterval = 3f; // deduct CSAT at this interval (seconds)

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

    // Call this method to add dosh (base * modifier) when an email is sent
    public void AddDosh()
    {
        totalDosh += Mathf.Round(baseDoshPerEmail * doshModifier * 100f) / 100f;
        doshText.text = "$" + totalDosh.ToString();
    }

    // Call this method to subtract dosh (base * negative modifier) when an email is sent
    public void SubtractDosh()
    {
        totalDosh -= Mathf.Round(baseDoshPerEmail * doshNegativeModifier * 100f) / 100f;
        doshText.text = "$" + totalDosh.ToString();
    }
}
