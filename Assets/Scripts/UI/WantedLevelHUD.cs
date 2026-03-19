using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// HUD display for police/wanted level information
/// </summary>
public class WantedLevelHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wantedLevelText;
    [SerializeField] private TextMeshProUGUI wantedDescriptionText;
    [SerializeField] private TextMeshProUGUI policeCountText;
    [SerializeField] private Image wantedLevelBar;
    [SerializeField] private PoliceSystem policeSystem;

    private int lastWantedLevel = -1;

    private void Start()
    {
        if (policeSystem == null)
            policeSystem = FindObjectOfType<PoliceSystem>();
    }

    private void Update()
    {
        if (policeSystem == null) return;

        UpdateWantedDisplay();
        UpdatePoliceCount();
    }

    private void UpdateWantedDisplay()
    {
        int currentWanted = policeSystem.CurrentWantedLevel;

        if (currentWanted != lastWantedLevel)
        {
            lastWantedLevel = currentWanted;

            if (wantedLevelText != null)
            {
                wantedLevelText.text = $"Wanted: {currentWanted}/5";
                wantedLevelText.color = GetWantedColor(currentWanted);
            }

            if (wantedDescriptionText != null)
            {
                wantedDescriptionText.text = policeSystem.GetWantedDescription();
                wantedDescriptionText.color = GetWantedColor(currentWanted);
            }

            if (wantedLevelBar != null)
            {
                wantedLevelBar.fillAmount = currentWanted / 5f;
                wantedLevelBar.color = GetWantedColor(currentWanted);
            }
        }
    }

    private void UpdatePoliceCount()
    {
        if (policeCountText != null)
        {
            int policeCount = policeSystem.GetActiveOfficers().Count;
            policeCountText.text = $"Police: {policeCount}";
            policeCountText.color = policeCount > 0 ? Color.red : Color.green;
        }
    }

    private Color GetWantedColor(int level)
    {
        return level switch
        {
            0 => Color.green,
            1 => Color.yellow,
            2 => new Color(1f, 0.5f, 0f),    // Orange
            3 => new Color(1f, 0.3f, 0.3f),  // Light red
            4 => Color.red,
            5 => Color.red,
            _ => Color.white
        };
    }
}
