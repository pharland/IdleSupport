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
    public float totalDosh = 0;

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

    [Header("Emails")]
    [Tooltip("Delay before incorrect buttons start turning red.")]
    public float delayBeforeButtonGoingRed = 5f;

    [Tooltip("% Chance (0-1) to ignore the bad effect of clicking an incorrect response email button.")]
    public float chanceToIngoreIncorrectResponseEffect = 0f;

    [HideInInspector]
    public int emailsSent = 0;
    [HideInInspector]
    public bool isFired = false; 
    public bool isFirstEmailSent = false; // first/tutorial email is sent


    private int emailsSentToday = 0; // Sends 0%CSAT fake email if still 0 at end of day. Resets to 0 at day start. 
    private int daysEmployed = 0;
    private int daysManagerUpset = 0; // incremented at day end if manager vibe is below bad threshold, resets to 0 if vibe is neutral/good at the end of any day
    private float dayTimer = 0f; // ticks every second
    private float averageCSAT = 0f; // updated after each email is sent, affects manager vibe

    private GameObject firedPanel; // UI panel to show when fired

    private void Awake()
    {
        // initialise fired UI panel and set it to inactive by default
        firedPanel = GameObject.Find("Panel_Fired");
        if (firedPanel != null)
        {
            firedPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("FiredPanel not found in scene!");
        }
    }

    void Update()
    {
        if (!isFired && isFirstEmailSent)
        {
            IncrementDay();
        }
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
                UpdateManagerVibe(0f);

                // Play negative SFX here
            }

            // days manager upset
            if (managerVibe < managerVibeBadThreshold)
            {
                daysManagerUpset++;

                // check if fired
                if (daysManagerUpset >= badDaysUntilFired)
                {
                    FirePlayer();
                }
            }
            else
            {
                daysManagerUpset = 0;
            }

            // reset emails sent today
            emailsSentToday = 0;
        }
    }

    /// <summary>
    /// Handle firing the player
    /// </summary>
    public void FirePlayer()
    {
        //enabled FIRED screen ui and pause game
        firedPanel.SetActive(true);
        isFired = true;

        // Play fired SFX here
    }

    /// <summary>
    /// Adds dosh (base * modifier) when an email is sent
    /// </summary>
    public void AddDosh()
    {
        totalDosh += Mathf.Round(baseDoshPerEmail * doshModifier * 100f) / 100f;
        UpdateDoshUI();
    }

    /// <summary>
    /// Adds bonus dosh (bonus * modifier) when email is sent AND manager vibe is good
    /// </summary>
    public void AddBonusDosh()
    {
        totalDosh += Mathf.Round(bonusDoshPerEmail * bonusDoshModifier * 100f) / 100f;
        UpdateDoshUI();
    }

    /// <summary>
    /// Subtracts dosh (base * negative modifier) when an email is sent
    /// </summary>
    public void SubtractDosh()
    {
        totalDosh -= Mathf.Round(baseDoshPerEmail * doshNegativeModifier * 100f) / 100f;
        UpdateDoshUI();
    }

    /// <summary>
    /// Update the dosh display text
    /// </summary>
    public void UpdateDoshUI()
    {
        doshText.text = "$" + totalDosh.ToString("F2");
    }

    // Update manager vibe based on average CSAT
    public void UpdateManagerVibe(float csatScore)
    {
        emailsSentToday++;

        // Update average CSAT and recalculate manager vibe
        averageCSAT = ((averageCSAT * (emailsSent - 1)) + csatScore) / emailsSent;
        if (float.IsNaN(averageCSAT) || averageCSAT < 0f)
        {
            averageCSAT = 0f;
        }
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
