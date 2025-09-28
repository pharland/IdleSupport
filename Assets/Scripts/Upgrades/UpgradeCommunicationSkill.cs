using UnityEngine;

public class UpgradeCommunicationSkill : MonoBehaviour, IUpgradeBehaviour
{
    public void ActivateUpgrade()
    {
        Debug.Log("Enabled Communication Skill...");
        // Your effect logic here
    }

    public void DeactivateUpgrade()
    {
        Debug.Log("Disabled Communication Skill...");
        // Your effect logic here
    }
    public void IncreaseUpgradeLevel()
    {
        Debug.Log("Communication Skill Up!");
        // Your effect logic here
    }
}