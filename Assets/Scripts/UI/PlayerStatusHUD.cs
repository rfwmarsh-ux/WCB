using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// HUD display for player health and armour
/// </summary>
public class PlayerStatusHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armourText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image armourBar;
    [SerializeField] private PlayerManager playerManager;

    private float lastHealth = float.MaxValue;
    private float lastArmour = float.MaxValue;

    private void Start()
    {
        if (playerManager == null)
            playerManager = GameManager.Instance?.GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (playerManager == null) return;

        UpdateHealthDisplay();
        UpdateArmourDisplay();
    }

    private void UpdateHealthDisplay()
    {
        float currentHealth = playerManager.GetHealth();
        float maxHealth = playerManager.GetMaxHealth();

        if (currentHealth != lastHealth)
        {
            lastHealth = currentHealth;

            if (healthText != null)
            {
                healthText.text = $"Health: {currentHealth:F0}/{maxHealth:F0}";
                healthText.color = GetHealthColor(currentHealth, maxHealth);
            }

            if (healthBar != null)
            {
                healthBar.fillAmount = currentHealth / maxHealth;
                healthBar.color = GetHealthColor(currentHealth, maxHealth);
            }
        }
    }

    private void UpdateArmourDisplay()
    {
        float currentArmour = playerManager.GetArmour();
        float maxArmour = playerManager.GetMaxArmour();

        if (currentArmour != lastArmour)
        {
            lastArmour = currentArmour;

            if (armourText != null)
            {
                armourText.text = $"Armour: {currentArmour:F0}/{maxArmour:F0}";
                armourText.color = currentArmour > 0 ? new Color(0.2f, 0.2f, 0.8f) : Color.gray;
            }

            if (armourBar != null)
            {
                armourBar.fillAmount = currentArmour / maxArmour;
                armourBar.color = new Color(0.2f, 0.2f, 0.8f);
            }
        }
    }

    private Color GetHealthColor(float health, float maxHealth)
    {
        float healthPercent = health / maxHealth;

        return healthPercent switch
        {
            > 0.7f => Color.green,
            > 0.4f => Color.yellow,
            > 0.2f => new Color(1f, 0.5f, 0f),
            _ => Color.red
        };
    }
}
