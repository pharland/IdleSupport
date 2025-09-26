using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IncorrectButton : MonoBehaviour
{
    public Color startColour = Color.paleGreen;
    public Color endColour = Color.red;
    public float transitionDuration = 10f; // Duration of the color transition in seconds
    public float delayBeforeStart = 5f; // Delay before starting the transition in seconds

    private Image image; // Reference to the Image component of the button
    private float transitionTimer = 0f; // Timer to track the transition progress
    private bool isTransitioning = true; // Flag to indicate if the transition is in progress

    void Awake()
    {
        image = GetComponent<Image>();
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
        if (image != null && isTransitioning)
        {
            // Wait before starting the transition
            if (transitionTimer < delayBeforeStart)
            {
                transitionTimer += Time.deltaTime;
                return;
            }

            // Start the color transition after 5 seconds
            float transitionElapsed = transitionTimer - 5f;
            float t = Mathf.Clamp01(transitionElapsed / transitionDuration); // Normalized time (0 to 1)
            image.color = Color.Lerp(startColour, endColour, t); // Interpolate between start and end colors

            // Stop transitioning when the duration is reached
            if (t >= 1f)
            {
                isTransitioning = false;
            }

            transitionTimer += Time.deltaTime; // Increment the timer
        }
    }
}
