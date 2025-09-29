using UnityEngine;

/// <summary>
/// This upgrade reduces the Manager good threshold, making them easier to please
/// </summary>
public class UpgradeGlazeManager : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.managerVibeGoodThreshold -= 0.02f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.managerVibeGoodThreshold = 0.95f;
    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.managerVibeGoodThreshold -= 0.02f;
    }
}