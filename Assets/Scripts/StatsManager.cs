using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text field displaying the number of days employed.")]
    [SerializeField] private TextMeshProUGUI daysEmployedText;

    [Tooltip("Text field displaying the total dosh earned.")]
    [SerializeField] private TextMeshProUGUI doshText;

    [Header("Day Progression")]
    [Tooltip("How many seconds in real time is a day in game.")]
    [SerializeField] private float secondsPerDay;


    [Header("Manager Vibe")]
    [Tooltip("Current manager vibe, affects dosh earned/lost. Range of 0 to 1")]
    public float managerVibe = 0.8f;

    [Tooltip("Slider representing manager vibe.")]
    [SerializeField] private Slider managerVibeSlider;

    [Tooltip("Slider bar changing colour based on manager vibe.")]
    [SerializeField] private Image managerVibeBar;

    [Tooltip("Anything above this is good.")]
    public float managerVibeGoodThreshold = 0.95f;

    [Tooltip("Anything below this is bad.")]
    public float managerVibeBadThreshold = 0.8f;

    [Tooltip("If manager vibe is below the bad threshold at this number of days in a row, the player is fired.")]
    public int badDaysUntilFired = 7;

    [Header("Dosh Settings")]
    [Tooltip("Base amount of dosh to earn per email sent.")]
    public float baseDoshPerEmail = 2f;

    [Tooltip("Modifies base dosh earned.")]
    public float doshModifier = 1f;

    [Tooltip("Base amount of dosh to earn per email sent when Manager vibe is good.")]
    public float bonusDoshPerEmail = 2f;

    [Tooltip("Modifies bonus dosh earned.")]
    public float bonusDoshModifier = 1f;

    [Tooltip("Deduct this amount of dosh per email sent with errors.")]
    public float subtractDoshAmount = 2f;

    [Tooltip("Modifies dosh lost.")]
    public float doshNegativeModifier = 1f;

    [Header("CSAT Settings")]
    [Tooltip("Deduct this amount of CSAT for every interval.")]
    public float subtractCSATAmount = 10f;

    [Tooltip("Deduct CSAT at this interval (seconds).")]
    public float subtractCSATInterval = 3f;


    [HideInInspector]
    public int emailsSent = 0;

    private int emailsSentToday = 0; // Sends 0%CSAT fake email if still 0 at end of day. Resets to 0 at day start. 
    private int daysEmployed = 0;
    private int daysManagerUpset = 0; // incremented at day end if manager vibe is below bad threshold, resets to 0 if vibe is neutral/good at the end of any day
    private float totalDosh = 0;
    private float dayTimer = 0f; // ticks every second
    private float averageCSAT = 0f; // updated after each email is sent, affects manager vibe

    void Update()
    {
        IncrementDay();
    }

    // Increment days based on the timer
    private void IncrementDay()
    {
        dayTimer += Time.deltaTime;
        if (dayTimer >= secondsPerDay)
        {
            // days employed
            daysEmployed++;
            daysEmployedText.text = daysEmployed.ToString();
            dayTimer = 0f;

            // csat penalty if no emails sent today
            if (emailsSentToday == 0)
            {
                UpdateManagerVibe(0f); // fake 0% CSAT email

                // Play negative SFX here
            }
            emailsSentToday = 0;

            // days manager upset
            if (managerVibe < managerVibeBadThreshold)
            {
                daysManagerUpset++;
            }
            else
            {
                daysManagerUpset = 0;
            }

            if (daysManagerUpset >= badDaysUntilFired)
            {
                Debug.Log("Fired!");
            }
        }
    }

    /// <summary>
    /// Adds dosh (base * modifier) when an email is sent
    /// </summary>
    public void AddDosh()
    {
        totalDosh += Mathf.Round(baseDoshPerEmail * doshModifier * 100f) / 100f;
        doshText.text = "$" + totalDosh.ToString();
    }

    /// <summary>
    /// Adds bonus dosh (bonus * modifier) when email is sent AND manager vibe is good
    /// </summary>
    public void AddBonusDosh()
    {
        totalDosh += Mathf.Round(bonusDoshPerEmail * bonusDoshModifier * 100f) / 100f;
        doshText.text = "$" + totalDosh.ToString();
    }

    /// <summary>
    /// Subtracts dosh (base * negative modifier) when an email is sent
    /// </summary>
    public void SubtractDosh()
    {
        totalDosh -= Mathf.Round(baseDoshPerEmail * doshNegativeModifier * 100f) / 100f;
        doshText.text = "$" + totalDosh.ToString();
    }

    // Update manager vibe based on average CSAT
    public void UpdateManagerVibe(float csatScore)
    {
        emailsSentToday++;

        // Update average CSAT and recalculate manager vibe
        averageCSAT = ((averageCSAT * (emailsSent - 1)) + csatScore) / emailsSent;
        managerVibe = Mathf.Clamp01(averageCSAT / 100f);

        // Update slider bar and colour based on vibe thresholds
        if (managerVibeSlider != null)
        {
            managerVibeSlider.value = managerVibe;

            if (managerVibe >= managerVibeGoodThreshold)
            {
                managerVibeBar.color = Color.green;
                AddBonusDosh();
            }
            else if (managerVibe >= managerVibeBadThreshold)
            {
                managerVibeBar.color = Color.yellow;
            }
            else
            {
                managerVibeBar.color = Color.red;
            }
        }
    }
}
