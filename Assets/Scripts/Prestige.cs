using UnityEngine;
using UnityEngine.UI;

public class Prestige : MonoBehaviour
{
    StatsManager statsManager;

    public GameObject quitButton;
    public TMPro.TextMeshProUGUI estimatedPayoutText;
    public int daysRequiredToQuit = 1;
    public float payoutPerDay = 10f;

    private void Start()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
        quitButton.GetComponent<Button>().interactable = false;
    }

    private void Update()
    {
        if (statsManager.daysEmployed >= daysRequiredToQuit)
        {
            quitButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            quitButton.GetComponent<Button>().interactable = false;
        }

        estimatedPayoutText.text = "$" + (statsManager.daysEmployed * payoutPerDay).ToString("F2");
    }

    public void PrestigeGame()
    {
        statsManager.PrestigePayout(payoutPerDay);
        statsManager.NewJob();
    }
}
