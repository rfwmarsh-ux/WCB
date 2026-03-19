using UnityEngine;

public class VehicleTheft : MonoBehaviour
{
    [SerializeField] private float stealRange = 5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.IsInVehicle)
            {
                ExitVehicle();
            }
            else
            {
                TryStealVehicle();
            }
        }
    }

    private void TryStealVehicle()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stealRange);
        
        Vehicle nearestVehicle = null;
        float nearestDist = float.MaxValue;
        
        foreach (var hit in hits)
        {
            Vehicle vehicle = hit.GetComponent<Vehicle>();
            if (vehicle != null && !vehicle.IsOccupied)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestVehicle = vehicle;
                }
            }
        }
        
        if (nearestVehicle != null)
        {
            AttemptTheft(nearestVehicle);
        }
        else
        {
            Debug.Log("No vehicle nearby to steal");
        }
    }

    private void AttemptTheft(Vehicle vehicle)
    {
        if (vehicle.VehicleType == VehicleType.PoliceCruiser)
        {
            Debug.Log("You can't steal a police car!");
            GameManager.Instance.UpdateWantedLevel(3);
            return;
        }

        Debug.Log($"Attempting to steal {vehicle.DisplayName}...");
        
        if (VehicleStealingSystem.Instance != null)
        {
            VehicleStealingSystem.Instance.StartStealingVehicle(vehicle);
        }
        else
        {
            if (vehicle.Steal())
            {
                EnterVehicle(vehicle);
            }
        }
    }
    
    private void EnterVehicle(Vehicle vehicle)
    {
        PlayerManager pm = PlayerManager.Instance;
        if (pm != null)
        {
            pm.EnterVehicle(vehicle);
            vehicle.SetDriver(GetComponent<PlayerController>());
            
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            transform.parent = vehicle.transform;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
            
            Debug.Log($"Entered {vehicle.DisplayName}!");
        }
    }
    
    private void ExitVehicle()
    {
        PlayerManager pm = PlayerManager.Instance;
        if (pm != null && pm.currentVehicle != null)
        {
            Vehicle vehicle = pm.currentVehicle;
            
            if (VehicleStealingSystem.Instance != null)
            {
                VehicleStealingSystem.Instance.RemoveVehicleOccupants(vehicle);
            }
            
            gameObject.SetActive(true);
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Rigidbody2D>().simulated = true;
            transform.parent = null;
            transform.position = vehicle.transform.position + new Vector3(3f, 0, 0);
            
            pm.ExitVehicle();
            vehicle.RemoveDriver();
            
            Debug.Log("Exited vehicle!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stealRange);
    }
}