using UnityEngine;
using UnityEngine.InputSystem;

public class EmailManager : MonoBehaviour
{
    public StatsManager statsManager;

    private EmailUIManager uiManager; // cached reference to the UI manager so we can register/unregister new emails

    [SerializeField] private TMPro.TextMeshProUGUI correctButtonsClickedText;
    [SerializeField] private TMPro.TextMeshProUGUI incorrectButtonsClickedText;
    [SerializeField] private TMPro.TextMeshProUGUI timeTakenText;
    public string emailTitle = "New Email"; // Title shown on the button that selects the Current Email

    public float csatScore = 100f;

    private int buttonsClicked = 0;
    private int correctButtonsClicked = 0;
    private int incorrectButtonsClicked = 0;
    private int totalButtons;
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

    public void CorrectButtonClick()
    {
        buttonsClicked++;
        correctButtonsClicked++;
        // Add positive SFX here

        //correctButtonsClickedText.text = "Correct Buttons Clicked = " + correctButtonsClicked.ToString();
    }
    public void IncorrectButtonClick()
    {
        buttonsClicked++;
        incorrectButtonsClicked++;
        csatScore -= statsManager.subtractCSATAmount;
        // Add negative SFX here

        //incorrectButtonsClickedText.text = "Incorrect Buttons Clicked = " + incorrectButtonsClicked.ToString();
    }

    public void SendEmailClicked()
    {
        pauseTimer = true;

        // Deduct CSAT based on time taken (10% for every 3 seconds)
        csatScore = 100 - (statsManager.subtractCSATAmount * (emailTimer / statsManager.subtractCSATInterval));
        if (csatScore < 0) csatScore = 0;

        // Check if all correct buttons were clicked and no incorrect buttons were clicked
        if (incorrectButtonsClicked == 0 && buttonsClicked >= totalCorrectButtons)
        {
            statsManager.AddDosh(2);

            // Update Manager vibe

            // Replace with call to function in EmailUiManager to show popup based on CSAT
            Debug.Log("Email Sent Successfully!"); 
            Debug.Log("Email Timer: " + emailTimer.ToString() + " seconds. CSAT = " + csatScore + "%");

            // Add positive SFX
        }
        else
        {
            statsManager.AddDosh(-2);

            // Update Manager vibe

            // Replace with call to function in EmailUiManager to show bad popup
            Debug.Log("Email Sent with Errors. Payment docked.");

            // add negative SFX
        }

        uiManager.DisplayEmailCSAT(csatScore);

        Destroy(gameObject);
    }

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
