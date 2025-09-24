using System.Collections.Generic;
using UnityEngine;

public class EmailUIManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Parent transform where email tab buttons will be instantiated")]
    public Transform buttonListParent;

    [Tooltip("Prefab for a single email tab button (must have EmailTabButton component)")]
    public GameObject buttonPrefab;

    [Header("Email Spawning")]
    [Tooltip("Prefab for current email")]
    public GameObject currentEmailPrefab;

    [Tooltip("Prefab transform for where to display the email")]
    public Transform emailContainerParent;

    [Tooltip("How quickly should new emails spawn (in seconds)")]
    [SerializeField] private float emailSpawnInterval = 10f;

    private float emailSpawnTimer = 0f;

    private readonly Dictionary<EmailManager, GameObject> map = new(); // map an EmailManager to its instantiated button GameObject
    private readonly List<EmailManager> allEmails = new(); // keep track of all registered emails

    private EmailManager activeEmail;

    // Register a new email with the UI manager, called by EmailManager in Start()
    public void RegisterEmail(EmailManager email)
    {
        if (email == null || buttonPrefab == null || buttonListParent == null) return;
        if (map.ContainsKey(email)) return;

        GameObject go = Instantiate(buttonPrefab, buttonListParent, false);

        if (go.TryGetComponent(out CurrentEmailButton tab))
        {
            // Setup the button to reference this email and the UI manager
            tab.Setup(email, this);
            map[email] = go;

            // Add to the list of all emails if not already present
            if (!allEmails.Contains(email))
            {
                allEmails.Add(email);
            }

            // If the email is already active, make sure it's the only active one
            if (email.gameObject.activeSelf)
            {
                ShowEmail(email);
            }

            // If this is the first registered email and none are active, optionally show it
            if (activeEmail == null && email.gameObject.activeSelf == false)
            {
                // do nothing — keep email hidden until user clicks a tab
            }
        }
        else
        {
            Debug.LogError("buttonPrefab must have an EmailTabButton component.");
            Destroy(go);
            return;
        }
    }

    public void UnregisterEmail(EmailManager email)
    {
        if (email == null) return;

        // Remove and destroy the associated button
        if (map.TryGetValue(email, out GameObject go))
        {
            Destroy(go);
            map.Remove(email);
            allEmails.Remove(email);
        }

        // If the email being removed is currently active, clear activeEmail reference
        if (activeEmail == email)
        {
            activeEmail = null;

            // optionally show the next available email if any
            foreach (KeyValuePair<EmailManager, GameObject> kv in map)
            {
                ShowEmail(kv.Key);
                break;
            }
        }
    }

    // Called by CurrentEmailButton when a tab is clicked
    public void OnTabClicked(EmailManager email)
    {
        ShowEmail(email);
    }

    // Make the requested email active and deactivate all others
    public void ShowEmail(EmailManager emailToShow)
    {
        foreach (KeyValuePair<EmailManager, GameObject> kv in map)
        {
            EmailManager email = kv.Key;
            if (email == null) continue;
            bool shouldBeActive = (email == emailToShow);
            email.gameObject.SetActive(shouldBeActive);
        }

        activeEmail = emailToShow;
    }

    void Update()
    {
        // Automatic email spawning
        emailSpawnTimer += Time.deltaTime;
        if (emailSpawnTimer >= emailSpawnInterval)
        {
            SpawnNewEmail("New Email");
            emailSpawnTimer = 0f;
        }

        // Tick all email timers, even if their GameObjects are inactive
        foreach (EmailManager email in allEmails)
        {
            if (email != null)
                email.TickTimer(Time.deltaTime);
        }
    }

    public EmailManager SpawnNewEmail(string emailTitle = "New Email")
    {
        // Instantiate a new email prefab and return its EmailManager component
        if (currentEmailPrefab != null && emailContainerParent != null)
        {
            // Instantiate the email prefab as a child of the Email_List parent
            GameObject newEmail = Instantiate(currentEmailPrefab, emailContainerParent);

            // Set the email title and activate the email GameObject
            if (newEmail.TryGetComponent(out EmailManager emailManager))
            {
                emailManager.emailTitle = emailTitle;
                newEmail.SetActive(true);
            }
            else
            {
                Debug.LogError("Spawned prefab does not have an EmailManager component!");
            }

            return emailManager;
        }

        Debug.LogError("CurrentEmailPrefab or EmailContainerParent not set!");
        return null;
    }
}