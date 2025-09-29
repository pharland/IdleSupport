using UnityEngine;

/// <summary>
/// This upgrade reduces the Manager bad threshold, making it harder for them to get upset
/// </summary>
public class UpgradeBlackmail : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.managerVibeBadThreshold -= 0.05f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.managerVibeBadThreshold = 0.8f;
    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.managerVibeBadThreshold -= 0.05f;
    }
}