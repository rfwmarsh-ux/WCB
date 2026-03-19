using UnityEngine;

/// <summary>
/// Handles vehicle interaction with roadworks obstacles
/// </summary>
public class RoadworksInteraction : MonoBehaviour
{
    [SerializeField] private float slowdownDuration = 2f;
    [SerializeField] private Vehicle vehicleComponent;

    private bool isInRoadworks = false;
    private float speedReductionFactor = 1f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vehicleComponent = GetComponent<Vehicle>();
    }

    private void FixedUpdate()
    {
        if (isInRoadworks && speedReductionFactor < 1f)
        {
            // Apply speed reduction
            if (rb != null)
            {
                rb.velocity *= speedReductionFactor;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Roadworks roadwork = collision.gameObject.GetComponent<Roadworks>();
            if (roadwork != null && roadwork.IsActive())
            {
                EnterRoadworks(roadwork);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Roadworks roadwork = collision.gameObject.GetComponent<Roadworks>();
            if (roadwork != null && roadwork.IsActive())
            {
                // Continue slowing while in roadworks
                isInRoadworks = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ExitRoadworks();
        }
    }

    private void EnterRoadworks(Roadworks roadwork)
    {
        isInRoadworks = true;
        speedReductionFactor = roadwork.GetSlowdownFactor();

        if (vehicleComponent != null)
        {
            Debug.Log($"{vehicleComponent.DisplayName} hit roadworks at {roadwork.GetZoneName()}");
        }
    }

    private void ExitRoadworks()
    {
        isInRoadworks = false;
        speedReductionFactor = 1f;
    }

    public bool IsInRoadworks() => isInRoadworks;
    public float GetCurrentSpeedReduction() => speedReductionFactor;
}
