using UnityEngine;

/// <summary>
/// This upgrade awards dosh if manager vibe is good at the end of the day for 7 days straight (gets weaker with each purchase)
/// </summary>
public class UpgradeWeeklyBonus : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;
    public int upgradeLevel = 0;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.isWeeklyBonusActive = true;
        if (upgradeLevel == 0)
        {
            upgradeLevel++;
        }
    }

    public void DeactivateUpgrade()
    {
        statsManager.bonusDoshPerWeek = 10f;
        statsManager.isWeeklyBonusActive = false;
    }

    public void IncreaseUpgradeLevel()
    {
        upgradeLevel++;
        // Starts with a big increase, but the increase gets smaller with each purchase
        statsManager.bonusDoshPerWeek += 10f / upgradeLevel;
    }
}