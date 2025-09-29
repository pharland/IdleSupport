using UnityEngine;

/// <summary>
/// This upgrade gives passive Dosh over time
/// </summary>
public class UpgradeCryptoFarm : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    private void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.passiveDoshPerSecond += 1f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.passiveDoshPerSecond = 0f;

    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.passiveDoshPerSecond += 1f;
    }
}