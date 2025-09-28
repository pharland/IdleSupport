using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UpgradeClass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // Add interfaces
{
    [Header("State")]
    public bool isUnlocked = false;
    [SerializeField] private int upgradeLevel = 0;
    public int maxUpgradeLevel = 1;
    public bool isEnabled = false;

    [Header("Cost")]
    [SerializeField] private float upgradeCost = 10f;
    [SerializeField] private string upgradeCostLocked = "???";
    public float upgradeCostMultiplier = 1.5f;

    [Header("Effect")]
    public float upgradeEffectiveness = 1f;

    [Header("Info")]
    [SerializeField] private string upgradeLockedName = "NOT UNLOCKED";
    [SerializeField] private string upgradeUnlockedName = "NAME GOES HERE";
    [SerializeField] private string upgradeTooltipText = "TOOLTIP GOES HERE.";

    [Header("UI References")]
    public GameObject buyButton;
    public GameObject toggleUpgrade;
    [SerializeField] private GameObject upgradeNameText;
    [SerializeField] private GameObject upgradeLevelText;
    [SerializeField] private GameObject upgradeTooltip;

    [Header("Audio")]
    public AudioClip upgradeSound;
    public AudioClip maxLevelUpgradeSound;

    [Header("UI Colors")]
    public Color upgradeLevelBaseColor = Color.black;
    public Color maxLevelColor = Color.yellow;

    private StatsManager statsManager;

    public void Awake()
    {
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();

        if (upgradeTooltip != null)
        {
            upgradeTooltip.SetActive(false);
        }

        if (isUnlocked)
        {
            UnlockUpgrade();
        }
        else
        {
            LockUpgrade();
        }
    }

    // On mouse hover, activate the Tooltip and update its text with the description of this upgrade
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (upgradeTooltip != null)
        {
            upgradeTooltip.SetActive(true);
            GameObject.Find("DynamicText_Tooltip").GetComponent<TextMeshProUGUI>().text = upgradeTooltipText;
            upgradeTooltip.transform.position = Mouse.current.position.ReadValue();
        }
        else
        {
            Debug.Log("Upgrade tooltip GameObject not assigned in inspector or not found in scene.");
        }
    }

    public void Update()
    {
        // Make the tooltip follow the mouse if it's active
        if (upgradeTooltip != null && upgradeTooltip.activeSelf)
        {
            upgradeTooltip.transform.position = Mouse.current.position.ReadValue();
        }
    }

    // Disable Tooltip when not hovering
    public void OnPointerExit(PointerEventData eventData)
    {
        if (upgradeTooltip != null)
        {
            upgradeTooltip.SetActive(false);
        }
    }

    public void UnlockUpgrade()
    {
        isUnlocked = true;

        // Enable the buyButton button and unobfuscate the cost text
        if (buyButton != null)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = upgradeCost.ToString();
            buyButton.GetComponent<Button>().interactable = true;
        }

        // Unobfuscate the upgrade name
        if (upgradeNameText != null)
        {
            upgradeNameText.GetComponent<TextMeshProUGUI>().text = upgradeUnlockedName;
        }

        // Unobfuscate the upgrade level text
        if (upgradeLevelText != null)
        {
            upgradeLevelText.GetComponent<TextMeshProUGUI>().text = "LV: " + upgradeLevel.ToString() + "/" + maxUpgradeLevel;
        }
    }

    public void LockUpgrade()
    {
        isUnlocked = false;

        // Disable the buyButton button and obfuscate the cost text
        if (buyButton != null)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = upgradeCostLocked;
            buyButton.GetComponent<Button>().interactable = false;
        }

        // Disable the toggleUpgrade button
        if (toggleUpgrade != null)
        {
            toggleUpgrade.GetComponent<Toggle>().interactable = false;
        }

        // Obfuscate the upgrade name
        if (upgradeNameText != null)
        {
            upgradeNameText.GetComponent<TextMeshProUGUI>().text = upgradeLockedName;
        }

        // Obfuscate the upgrade level text
        if (upgradeLevelText != null)
        {
            upgradeLevelText.GetComponent<TextMeshProUGUI>().text = "LV: 0/?";
        }
    }

    public void PurchaseUpgrade()
    {
        if (statsManager.totalDosh >= upgradeCost)
        {
            // Deduct the cost from the player's money
            statsManager.totalDosh -= upgradeCost;
            statsManager.UpdateDoshUI();

            // Increase the upgrade level
            upgradeLevel++;

            // Play the appropriate sound


            // Check if we've reached max level
            if (upgradeLevel >= maxUpgradeLevel)
            {
                // Disable the buyButton button and change its text
                if (buyButton != null)
                {
                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "N/A";
                    buyButton.GetComponent<Button>().interactable = false;
                }

                // Change the color of the upgrade name to indicate max level
                if (upgradeNameText != null)
                {
                    upgradeLevelText.GetComponent<TextMeshProUGUI>().text = "LV: MAX";
                    upgradeLevelText.GetComponent<TextMeshProUGUI>().color = maxLevelColor;
                }
            }
            else
            {
                // Update the cost for the next level, rounded up to nearest whole number
                upgradeCost = Mathf.Ceil(upgradeCost * upgradeCostMultiplier);

                // Update the buyButton text to show new cost
                if (buyButton != null)
                {
                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = upgradeCost.ToString();
                }

                // Update the upgrade level text
                if (upgradeLevelText != null)
                {
                    upgradeLevelText.GetComponent<TextMeshProUGUI>().text = "LV: " + upgradeLevel.ToString() + "/" + maxUpgradeLevel;
                    upgradeLevelText.GetComponent<TextMeshProUGUI>().color = upgradeLevelBaseColor;
                }
            }

            ActivateUpgrade();
            toggleUpgrade.GetComponent<Toggle>().isOn = true;
            toggleUpgrade.GetComponent<Toggle>().interactable = true;
        }
        else
        {
            // Play SFX


            Debug.Log("Not enough money to purchase upgrade.");
        }
    }

    public void ActivateUpgrade()
    {
        // Implement the effect of the upgrade here.
        Debug.Log(upgradeUnlockedName + " activated.");
    }

    public void DeactivateUpgrade()
    {
        // Implement the deactivation of the upgrade effect here.
        Debug.Log(upgradeUnlockedName + " deactivated.");
    }

    // Toggle the upgrade effect on or off depending on whether the toggleUpgrade is on or off
    public void ToggleUpgradeEffect()
    {
        if (toggleUpgrade != null && toggleUpgrade.GetComponent<Toggle>().isOn)
        {
            isEnabled = true;
            ActivateUpgrade();
        }
        else
        {
            isEnabled = false;
            DeactivateUpgrade();
        }
    }
}
