using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentEmailButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;

    private EmailManager email;
    private EmailUIManager uiManager;

    public void Setup(EmailManager email, EmailUIManager manager)
    {
        this.email = email;
        this.uiManager = manager;

        if (label != null)
        {
            // display a friendly title; EmailManager exposes 'emailTitle'
            label.text = string.IsNullOrEmpty(email.emailTitle) ? "Email" : email.emailTitle;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (uiManager != null)
        {
            uiManager.OnTabClicked(email);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClicked);
        }
    }
}