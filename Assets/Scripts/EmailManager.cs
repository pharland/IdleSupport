using UnityEditor.UIElements;
using UnityEngine;

public class EmailManager : MonoBehaviour
{
    public StatsManager statsManager;

    [SerializeField] private TMPro.TextMeshProUGUI correctButtonsClickedText;
    [SerializeField] private TMPro.TextMeshProUGUI incorrectButtonsClickedText;
    [SerializeField] private TMPro.TextMeshProUGUI timeTakenText;

    private int buttonsClicked = 0;
    private int correctButtonsClicked = 0;
    private int incorrectButtonsClicked = 0;
    private int totalButtons;
    private int totalCorrectButtons;
    private int totalIncorrectButtons;

    private float emailTimer = 0f;

    public float csatScore = 100f;

    void Start()
    {
        statsManager = GameObject.Find("Stats").GetComponent<StatsManager>();
        totalCorrectButtons = GameObject.FindGameObjectsWithTag("Correct").Length;
        totalIncorrectButtons = GameObject.FindGameObjectsWithTag("Incorrect").Length;
        //totalButtons = totalCorrectButtons + totalIncorrectButtons;
    }

    void Update()
    {
        // Increment email timer and round down to nearest second
        emailTimer += Time.deltaTime;
        timeTakenText.text = "Time taken (sec) = " + Mathf.FloorToInt(emailTimer).ToString();
    }

    public void CorrectButtonClick()
    {
        buttonsClicked++;
        correctButtonsClicked++;
        correctButtonsClickedText.text = "Correct Buttons Clicked = " + correctButtonsClicked.ToString();
    }
    public void IncorrectButtonClick()
    {
        buttonsClicked++;
        incorrectButtonsClicked++;
        incorrectButtonsClickedText.text = "Incorrect Buttons Clicked = " + incorrectButtonsClicked.ToString();
        csatScore -= statsManager.subtractCSATAmount;
    }

    public void SendEmailClicked()
    {
        // Deduct CSAT based on time taken (10% for every 3 seconds)
        csatScore = 100 - (statsManager.subtractCSATAmount * (emailTimer / statsManager.subtractCSATInterval));
        if (csatScore < 0) csatScore = 0;

        // Check if all correct buttons were clicked and no incorrect buttons were clicked
        if (incorrectButtonsClicked == 0 && buttonsClicked >= totalCorrectButtons)
        {
            Debug.Log("Email Sent Successfully!");
            Debug.Log("Email Timer: " + emailTimer.ToString() + " seconds. CSAT = " + csatScore + "%");
        }
        else
        {
            Debug.Log("Email Sent with Errors. Payment docked.");
        }
    }
}
