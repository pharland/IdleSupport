using UnityEngine;

/// <summary>
/// This upgrade increases dosh received after every successful email
/// </summary>
public class UpgradeBonusPay : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.doshModifier += 0.3f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.doshModifier = 1f;
    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.doshModifier += 0.3f;
    }
}