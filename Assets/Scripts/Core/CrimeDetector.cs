using UnityEngine;

/// <summary>
/// Tracks and detects crimes committed by the player
/// </summary>
public class CrimeDetector : MonoBehaviour
{
    [SerializeField] private PoliceSystem policeSystem;
    [SerializeField] private Vehicle playerVehicle;

    private Vehicle currentlyDrivenVehicle;
    private bool hasReportedTheft = false;
    private bool hasReportedHitAndRun = false;

    private void Start()
    {
        if (policeSystem == null)
            policeSystem = FindObjectOfType<PoliceSystem>();
    }

    private void Update()
    {
        DetectVehicleTheft();
    }

    private void DetectVehicleTheft()
    {
        // Check if player is in a stolen vehicle
        PlayerManager playerManager = GameManager.Instance.GetComponent<PlayerManager>();
        if (playerManager != null && playerManager.IsInVehicle)
        {
            Vehicle vehicle = playerManager.GetCurrentVehicle();
            if (vehicle != null && vehicle.IsStolen && !hasReportedTheft)
            {
                ReportCrime(PoliceSystem.CrimeType.VehicleTheft);
                hasReportedTheft = true;
            }
        }
    }

    public void ReportAssault()
    {
        ReportCrime(PoliceSystem.CrimeType.Assault);
    }

    public void ReportMurder()
    {
        ReportCrime(PoliceSystem.CrimeType.Murder);
    }

    public void ReportRobbery()
    {
        ReportCrime(PoliceSystem.CrimeType.Robbery);
    }

    public void ReportHitAndRun()
    {
        if (!hasReportedHitAndRun)
        {
            ReportCrime(PoliceSystem.CrimeType.HitAndRun);
            hasReportedHitAndRun = true;
        }
    }

    public void ReportRecklessDriving()
    {
        ReportCrime(PoliceSystem.CrimeType.RecklessDriving);
    }

    public void ReportBreakingAndEntering()
    {
        ReportCrime(PoliceSystem.CrimeType.BreakingAndEntering);
    }

    private void ReportCrime(PoliceSystem.CrimeType crime)
    {
        if (policeSystem != null)
        {
            policeSystem.CommitCrime(crime);
        }
    }

    public void ResetCrimeReports()
    {
        hasReportedTheft = false;
        hasReportedHitAndRun = false;
    }
}
