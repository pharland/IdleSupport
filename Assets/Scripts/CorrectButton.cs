using UnityEngine;
using UnityEngine.UI;

public class CorrectButton : MonoBehaviour
{
    StatsManager statsManager;

    public Color startColour = new(0.85f, 0.85f, 0.85f, 1f); // light grey
    public Color endColour = Color.paleGreen;
    public float transitionDuration = 10f; // Duration of the color transition in seconds

    private Image image; // Reference to the Image component of the button
    private float transitionTimer = 0f; // Timer to track the transition progress
    private bool isTransitioning = true; // Flag to indicate if the transition is in progress

    void Awake()
    {
        image = GetComponent<Image>();
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    void Start()
    {
        if (image != null)
        {
            image.color = startColour;
        }
    }

    void Update()
    {
        // Handle color transition with 5 seconds delay before starting
        if (image != null && isTransitioning && !statsManager.timersPaused)
        {
            // Wait before starting the transition
            if (transitionTimer < statsManager.delayBeforeButtonGoingRed)
            {
                transitionTimer += Time.deltaTime;
                return;
            }

            // Start the color transition after a delay
            float transitionElapsed = transitionTimer - statsManager.delayBeforeButtonGoingRed;
            float timer = Mathf.Clamp01(transitionElapsed / transitionDuration); // Normalized time (0 to 1)
            image.color = Color.Lerp(startColour, endColour, timer); // Interpolate between start and end colors

            // Stop transitioning when the duration is reached
            if (timer >= 1f)
            {
                isTransitioning = false;
            }

            transitionTimer += Time.deltaTime; // Increment the timer
        }
    }
}
