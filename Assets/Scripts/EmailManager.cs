using UnityEngine;
using TMPro;

public class EmailManager : MonoBehaviour
{
    public StatsManager statsManager;

    private EmailUIManager uiManager; // cached reference to the UI manager so we can register/unregister new emails

    [SerializeField] private TextMeshProUGUI correctButtonsClickedText;
    [SerializeField] private TextMeshProUGUI incorrectButtonsClickedText;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    public string emailTitle = "New Email"; // Title shown on the button that selects the Current Email

    public float csatScore = 100f;

    private int buttonsClicked = 0;
    private int correctButtonsClicked = 0;
    private int incorrectButtonsClicked = 0;
    //private int totalButtons;
    private int totalCorrectButtons;
    private int totalIncorrectButtons;
    private bool pauseTimer = false;
    private float emailTimer = 0f;

    void Awake()
    {
        uiManager = Object.FindFirstObjectByType<EmailUIManager>();
    }

    void Start()
    {
        // Set up dynamic references on email creation
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
        totalCorrectButtons = GameObject.FindGameObjectsWithTag("Correct").Length;
        totalIncorrectButtons = GameObject.FindGameObjectsWithTag("Incorrect").Length;

        //totalButtons = totalCorrectButtons + totalIncorrectButtons;
    }

    void OnEnable()
    {
        if (uiManager != null)
        {
            uiManager.RegisterEmail(this);
        }
    }

    void OnDestroy()
    {
        if (uiManager != null)
        {
            uiManager.UnregisterEmail(this);
        }
    }

    // Called when the player clicks a correct button
    public void CorrectButtonClick()
    {
        buttonsClicked++;
        correctButtonsClicked++;

        // Add positive SFX here


        //correctButtonsClickedText.text = "Correct Buttons Clicked = " + correctButtonsClicked.ToString();
    }

    // Called when the player clicks an incorrect button
    public void IncorrectButtonClick()
    {
        buttonsClicked++;
        incorrectButtonsClicked++;
        csatScore -= statsManager.subtractCSATAmount;

        // Add negative SFX here


        //incorrectButtonsClickedText.text = "Incorrect Buttons Clicked = " + incorrectButtonsClicked.ToString();
    }

    // Called when the player clicks the "Send Email" button
    public void SendEmailClicked()
    {
        pauseTimer = true;
        statsManager.emailsSent++;

        // Deduct CSAT based on time taken (10% for every 3 seconds)
        csatScore -= (emailTimer / statsManager.subtractCSATInterval);
        if (csatScore < 0) csatScore = 0;

        Debug.Log("Email Sent Successfully!");
        Debug.Log("Email Timer: " + emailTimer.ToString() + " seconds. CSAT = " + csatScore + "%");

        // Add dosh based on performance (negative + bad csat if incorrect/not all correct buttons clicked)
        if (incorrectButtonsClicked == 0 && buttonsClicked >= totalCorrectButtons)
        {
            // pay base dosh rate
            statsManager.AddDosh();

            // Update Manager vibe


            // Add positive SFX
        }
        else
        {
            // Subtract dosh by base pay rate
            statsManager.SubtractDosh();

            Debug.Log("Email sent with Errors. Payment docked.");

            // Update Manager vibe


            // add negative SFX
        }

        statsManager.UpdateManagerVibe(csatScore);

        // Little popup showing CSAT score for this email
        uiManager.DisplayEmailCSAT(csatScore);

        Destroy(gameObject);
    }

    // EmailUIManager continues to tick the timer each frame while this email is active
    public void TickTimer(float deltaTime)
    {
        if (!pauseTimer)
        {
            emailTimer += deltaTime;
            if (timeTakenText != null)
                timeTakenText.text = "Time taken (sec) = " + Mathf.FloorToInt(emailTimer).ToString();
        }
    }
}
