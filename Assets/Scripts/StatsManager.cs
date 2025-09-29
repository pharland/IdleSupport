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

    [Tooltip("Number of days in a row the manager vibe has been below the bad threshold.")]
    public int daysManagerUpset = 0;

    [Tooltip("Number of days in a row the manager vibe has been above the good threshold.")]
    public int daysManagerHappy = 0;


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

    [Tooltip("If true, weekly bonus dosh is active.")]
    public bool isWeeklyBonusActive = false;

    [Tooltip("Base amount of dosh earned if Manager Vibe is good every day for a 7 days straight.")]
    public float bonusDoshPerWeek = 20f;

    [Tooltip("Modifies weekly bonus dosh earned.")]
    public float bonusDoshPerWeekModifier = 1f;

    [Tooltip("Deduct this amount of dosh per email sent with errors.")]
    public float subtractDoshAmount = 2f;

    [Tooltip("Modifies dosh lost.")]
    public float doshNegativeModifier = 1f;

    [Tooltip("Passive dosh earned per second from upgrades.")]
    public float passiveDoshPerSecond = 0f;

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

    [Tooltip("If true, timers (CSAT decrease, button going red) are paused.")]
    public bool timersPaused = false;

    [HideInInspector]
    public int emailsSent = 0;
    [HideInInspector]
    public bool isFired = false; 
    public bool isFirstEmailSent = false; // first/tutorial email is sent


    private int emailsSentToday = 0; // Sends 0%CSAT fake email if still 0 at end of day. Resets to 0 at day start. 
    public int daysEmployed = 0;
    private float dayTimer = 0f; // ticks every second
    private float averageCSAT = 0f; // updated after each email is sent, affects manager vibe

    private GameObject firedPanel; // UI panel to show when fired

    EmailUIManager emailUIManager; // Cached reference to EmailUIManager
    private float passiveDoshTimer; // Timer for passive dosh accumulation

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
        if (!isFired && isFirstEmailSent && !timersPaused)
        {
            IncrementDay();
        }

        // passive dosh from upgrades
        if (passiveDoshPerSecond > 0f && !timersPaused)
        {
            passiveDoshTimer += Time.deltaTime;
            if (passiveDoshTimer >= 1f)
            {
                int secondsPassed = Mathf.FloorToInt(passiveDoshTimer);
                float doshToAdd = passiveDoshPerSecond * secondsPassed;
                totalDosh += doshToAdd;
                UpdateDoshUI();
                passiveDoshTimer -= secondsPassed;
            }
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

            // get a list of all GameObjects with the "Upgrade" tag and get their UpgradeClass component
            GameObject[] upgradeObjects = GameObject.FindGameObjectsWithTag("Upgrade");

            // loop through each upgrade GameObject and call UnlockUpgrade() if the required day is met
            foreach (GameObject upgradeObject in upgradeObjects)
            {
                UpgradeClass upgrade = upgradeObject.GetComponent<UpgradeClass>();
                if (upgrade != null && !upgrade.isUnlocked && upgrade.daysToUnlock > 0 && daysEmployed >= upgrade.daysToUnlock)
                {
                    upgrade.UnlockUpgrade();
                }
            }

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

            // days manager happy
            if (isWeeklyBonusActive && managerVibe >= managerVibeGoodThreshold)
            {
                daysManagerHappy++;
                if (daysManagerHappy == 7)
                {
                    AddWeeklyBonusDosh();
                    daysManagerHappy = 0;
                }
            }
            else
            {
                daysManagerHappy = 0;
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
    /// Reset stats and upgrades for a new job
    /// </summary>
    public void NewJob()
    {
        //reset all relevant stats
        isFired = false;
        firedPanel.SetActive(false);
        daysEmployed = 0;
        daysEmployedText.text = daysEmployed.ToString();
        managerVibe = 0.8f;
        if (managerVibeSlider != null)
        {
            managerVibeSlider.value = managerVibe;
            managerVibeBar.color = Color.yellow;
        }
        daysManagerHappy = 0;
        daysManagerUpset = 0;
        averageCSAT = 0f;
        emailsSent = 0;
        emailsSentToday = 0;
        dayTimer = 0f;

        // deactivate all non-permanent upgrades, reset levels to 0 and cost to base cost
        GameObject[] upgradeObjects = GameObject.FindGameObjectsWithTag("Upgrade");
        foreach (GameObject upgradeObject in upgradeObjects)
        {
            UpgradeClass upgrade = upgradeObject.GetComponent<UpgradeClass>();
            if (upgrade != null && !upgrade.isPermanent && upgrade.isUnlocked)
            {
                upgrade.DeactivateUpgrade();
                upgrade.toggleUpgrade.GetComponent<Toggle>().isOn = false;
                upgrade.toggleUpgrade.GetComponent<Toggle>().interactable = false;
                upgrade.upgradeLevel = 0;
                upgrade.nextUpgradeCost = upgrade.baseUpgradeCost;
                upgrade.buyButton.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.nextUpgradeCost.ToString();
                if (upgrade.daysToUnlock > 0)
                {
                    upgrade.LockUpgrade();
                }
                else
                {
                    upgrade.UpdateUpgradeLevelText();
                }
            }
        }

        // Delete all existing emails and spawn the first day email
        emailUIManager = GameObject.Find("EmailListManager").GetComponent<EmailUIManager>();
        if (emailUIManager != null)
        {
            emailUIManager.DeleteAllEmails();
            emailUIManager.SpawnNewEmail("First Day Email", emailUIManager.firstDayEmail);
            isFirstEmailSent = false;
        }
        else
        {
            Debug.LogError("EmailListManager not found in scene!");
        }
    }

    /// <summary>
    /// Award payout based on days employed when player prestiges
    /// </summary>
    /// <param name="payoutPerDay"></param>
    public void PrestigePayout(float payoutPerDay)
    {
        float payout = daysEmployed * payoutPerDay;
        totalDosh += payout;
        UpdateDoshUI();
    }

    /// <summary>
    /// Adds dosh (base * modifier) when an email is sent
    /// </summary>
    public void AddDosh()
    {
        var doshToAdd = Mathf.Round(baseDoshPerEmail * doshModifier * 100f) / 100f;
        totalDosh += doshToAdd;
        UpdateDoshUI();
    }

    /// <summary>
    /// Adds bonus dosh (bonus * modifier) when 7 days straight of good manager vibes
    /// </summary>
    public void AddWeeklyBonusDosh()
    {
        var doshToAdd = Mathf.Round(bonusDoshPerWeek * bonusDoshPerWeekModifier * 100f) / 100f;
        totalDosh += doshToAdd;
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

    public void PauseTimers()
    {
        timersPaused = true;
    }

    public void ResumeTimers()
    {
        timersPaused = false;
    }
}
