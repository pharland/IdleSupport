using UnityEngine;

/// <summary>
/// This upgrade increases the chances of ignoring the bad stuff when a red email response button is clicked
/// </summary>
public class UpgradeCommunicationSkill : MonoBehaviour, IUpgradeBehaviour
{
    StatsManager statsManager;

    private void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
    }

    public void ActivateUpgrade()
    {
        statsManager.chanceToIngoreIncorrectResponseEffect += 0.1f;
    }

    public void DeactivateUpgrade()
    {
        statsManager.chanceToIngoreIncorrectResponseEffect = 0f;

    }

    public void IncreaseUpgradeLevel()
    {
        statsManager.chanceToIngoreIncorrectResponseEffect += 0.1f;
    }
}