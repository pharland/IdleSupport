using UnityEngine;

/// <summary>
/// This upgrade decreases the interval between email spawns
/// </summary>
public class UpgradeTechnicalSkill : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.delayBeforeButtonGoingRed -= 1f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.delayBeforeButtonGoingRed = 5f;
    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.delayBeforeButtonGoingRed -= 1f;
    }
}