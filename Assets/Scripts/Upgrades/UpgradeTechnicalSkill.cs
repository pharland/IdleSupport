using UnityEngine;

public class UpgradeTechnicalSkill : MonoBehaviour, IUpgradeBehaviour
{
    public void ActivateUpgrade()
    {
        Debug.Log("Enabled Technical Skill...");
        // Your effect logic here
    }

    public void DeactivateUpgrade()
    {
        Debug.Log("Disabled Technical Skill...");
        // Your effect logic here
    }
    public void IncreaseUpgradeLevel()
    {
        Debug.Log("Technical Skill Up!");
        // Your effect logic here
    }
}