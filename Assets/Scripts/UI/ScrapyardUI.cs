using UnityEngine;
using TMPro;

/// <summary>
/// UI display for scrapyard interactions
/// </summary>
public class ScrapyardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scrapyardInfoText;
    [SerializeField] private PlayerManager playerManager;
    private Scrapyard currentScrapyard;
    private bool isInScrapyard = false;

    private void Start()
    {
        if (playerManager == null)
            playerManager = GameManager.Instance?.GetComponent<PlayerManager>();
    }

    public void EnterScrapyard(Scrapyard scrapyard)
    {
        currentScrapyard = scrapyard;
        isInScrapyard = true;

        if (scrapyardInfoText != null)
        {
            scrapyardInfoText.text = $"Welcome to {scrapyard.GetScrapyardName()}\n\n";
            scrapyardInfoText.text += $"Vehicle Respray Service\n";
            scrapyardInfoText.text += $"Cost: {scrapyard.GetResprayBaseCost()}$\n\n";
            scrapyardInfoText.text += "Resprays vehicle to a random color and\n";
            scrapyardInfoText.text += "removes your wanted level!\n\n";
            scrapyardInfoText.text += "Press [E] to respray vehicle";
        }
    }

    public void ExitScrapyard()
    {
        isInScrapyard = false;
        currentScrapyard = null;

        if (scrapyardInfoText != null)
            scrapyardInfoText.text = "";
    }

    public void ResprayVehicle()
    {
        if (currentScrapyard == null) return;

        VehicleManager vehicleManager = GameManager.Instance.GetVehicleManager();
        Vehicle playerVehicle = vehicleManager?.GetNearestVehicle(playerManager.transform.position);

        if (playerVehicle != null)
        {
            currentScrapyard.ResprayVehicle(playerVehicle, playerManager, GameManager.Instance);
            Debug.Log("Vehicle resprayed successfully!");
        }
        else
        {
            Debug.Log("No vehicle nearby to respray!");
        }
    }

    public bool IsInScrapyard() => isInScrapyard;
}
