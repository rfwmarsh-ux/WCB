using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// HUD display for gang reputation and status
/// </summary>
public class GangStatusHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gangReputationText;
    [SerializeField] private TextMeshProUGUI gangStanceText;
    [SerializeField] private TextMeshProUGUI gangMemberCountText;
    [SerializeField] private Image gangReputationBar;
    [SerializeField] private MotorcycleGang motorcycleGang;

    private int lastReputation = int.MinValue;
    private MotorcycleGang.GangStance lastStance = MotorcycleGang.GangStance.Neutral;

    private void Start()
    {
        if (motorcycleGang == null)
            motorcycleGang = FindObjectOfType<MotorcycleGang>();
    }

    private void Update()
    {
        if (motorcycleGang == null) return;

        UpdateGangDisplay();
    }

    private void UpdateGangDisplay()
    {
        int currentRep = motorcycleGang.GetPlayerReputation();
        MotorcycleGang.GangStance currentStance = motorcycleGang.CurrentStance;

        if (currentRep != lastReputation || currentStance != lastStance)
        {
            lastReputation = currentRep;
            lastStance = currentStance;

            if (gangReputationText != null)
            {
                gangReputationText.text = $"Gang Rep: {currentRep}";
                gangReputationText.color = GetReputationColor(currentRep);
            }

            if (gangStanceText != null)
            {
                gangStanceText.text = $"Status: {currentStance}";
                gangStanceText.color = GetStanceColor(currentStance);
            }

            if (gangReputationBar != null)
            {
                gangReputationBar.fillAmount = (currentRep + 100f) / 200f; // Convert -100 to 100 range to 0-1
                gangReputationBar.color = GetStanceColor(currentStance);
            }
        }

        if (gangMemberCountText != null)
        {
            int memberCount = motorcycleGang.GetMemberCount();
            gangMemberCountText.text = $"{motorcycleGang.GetGangName()}: {memberCount}/{15}";
            gangMemberCountText.color = currentStance == MotorcycleGang.GangStance.Hostile ? Color.red : Color.white;
        }
    }

    private Color GetReputationColor(int reputation)
    {
        return reputation switch
        {
            < -50 => Color.red,
            < 0 => new Color(1f, 0.5f, 0f),    // Orange
            < 50 => Color.yellow,
            >= 50 => Color.green,
            _ => Color.white
        };
    }

    private Color GetStanceColor(MotorcycleGang.GangStance stance)
    {
        return stance switch
        {
            MotorcycleGang.GangStance.Hostile => Color.red,
            MotorcycleGang.GangStance.Neutral => Color.yellow,
            MotorcycleGang.GangStance.Friendly => new Color(0f, 1f, 0.5f),
            MotorcycleGang.GangStance.Allied => Color.green,
            _ => Color.white
        };
    }
}
